namespace More.VisualStudio.Editors
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Designer.Interfaces;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.InteropServices;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Represents the base implementation for a code generator.
    /// </summary>
    [ComVisible( true )]
    public abstract class CodeGenerator : IVsSingleFileGenerator, IObjectWithSite
    {
        private const int NoInterface = -2147467262;
        private readonly Lazy<string> defaultExtension;
        private object site = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        protected CodeGenerator()
        {
            this.defaultExtension = new Lazy<string>( this.GetDefaultExtension );
        }

        /// <summary>
        /// Gets the default extension used for generated files.
        /// </summary>
        /// <value>The default extension used for generated files. The default value
        /// is ".g" plus the file extension of code files for the current language.</value>
        public virtual string DefaultExtension
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.defaultExtension.Value;
            }
        }

        /// <summary>
        /// Generates the code content using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="">context</see> used to generate content.</param>
        /// <returns>A <see cref="Stream">stream</see> containing the generated content.</returns>
        protected abstract Stream Generate( CodeGeneratorContext context );

        private string GetDefaultExtension()
        {
            Contract.Ensures( Contract.Result<string>() != null );

            if ( this.site == null )
                return string.Empty;

            using ( var serviceProvider = new VisualStudioServiceProvider( (IOleServiceProvider) this.site ) )
            {
                IVSMDCodeDomProvider provider;
                CodeDomProvider codeProvider;

                if ( serviceProvider.TryGetService( out provider ) && ( codeProvider = provider.CodeDomProvider as CodeDomProvider ) != null )
                    return ".g." + codeProvider.FileExtension;
            }

            return string.Empty;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Exposed as the DefaultExtension property." )]
        int IVsSingleFileGenerator.DefaultExtension( out string pbstrDefaultExtension )
        {
            var ext = this.DefaultExtension;

            if ( ext.Length > 0 && ext[0] != '.' )
                ext = "." + ext;

            pbstrDefaultExtension = ext;
            return 0;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Debug assertions are used. VS will never pass null here." )]
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive. Disposed on the finally block. The CodeGeneratorContext will never fail. If it did, there's nothing to dispose anyway." )]
        int IVsSingleFileGenerator.Generate( string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress )
        {
            Debug.Assert( rgbOutputFileContents != null, "rgbOutputFileContents is null" );
            Debug.Assert( rgbOutputFileContents.Length > 0, "rgbOutputFileContents is zero-length" );

            var serviceProvider = this.site == null ? ServiceProvider.Current : new VisualStudioServiceProvider( (IOleServiceProvider) this.site );
            var context = new CodeGeneratorContext( wszInputFilePath, bstrInputFileContents, wszDefaultNamespace, new VsProgressAdapter( pGenerateProgress ), serviceProvider );

            try
            {
                // generate content as stream
                using ( var stream = this.Generate( context ) )
                {
                    var temp = stream as MemoryStream;
                    var size = (int) stream.Length;
                    byte[] buffer;

                    if ( temp == null )
                    {
                        buffer = new byte[size];
                        stream.Read( buffer, 0, size );
                    }
                    else
                    {
                        buffer = temp.ToArray();
                    }

                    rgbOutputFileContents[0] = Marshal.AllocCoTaskMem( size );
                    Marshal.Copy( buffer, 0, rgbOutputFileContents[0], size );
                    pcbOutput = (uint) size;
                }
            }
            catch ( Exception ex )
            {
                rgbOutputFileContents[0] = IntPtr.Zero;
                pcbOutput = 0U;
                context.Progress.ReportError( SR.GenerateError.FormatDefault( ex.Message ) );
                throw;
            }
            finally
            {
                var disposable = serviceProvider as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }

            return 0;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Only used or called by VS." )]
        void IObjectWithSite.GetSite( ref Guid riid, out IntPtr ppvSite )
        {
            if ( this.site == null )
                Marshal.ThrowExceptionForHR( NoInterface );

            var iunknown = Marshal.GetIUnknownForObject( site );
            var hr = Marshal.QueryInterface( iunknown, ref riid, out ppvSite );

            Marshal.Release( iunknown );
            ErrorHandler.ThrowOnFailure( hr );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Only used or called by VS." )]
        void IObjectWithSite.SetSite( object pUnkSite )
        {
            this.site = pUnkSite;
        }
    }
}
