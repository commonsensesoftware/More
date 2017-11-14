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
    using static System.Runtime.InteropServices.Marshal;
    using static System.String;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Represents the base implementation for a code generator.
    /// </summary>
    [ComVisible( true )]
    public abstract class CodeGenerator : IVsSingleFileGenerator, IObjectWithSite
    {
        const int NoInterface = -2147467262;
        readonly Lazy<string> defaultExtension;
        object site = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        protected CodeGenerator() => defaultExtension = new Lazy<string>( GetDefaultExtension );

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
                return defaultExtension.Value;
            }
        }

        /// <summary>
        /// Generates the code content using the provided context.
        /// </summary>
        /// <param name="context">The <see cref="CodeGeneratorContext">context</see> used to generate content.</param>
        /// <returns>A <see cref="Stream">stream</see> containing the generated content.</returns>
        protected abstract Stream Generate( CodeGeneratorContext context );

        string GetDefaultExtension()
        {
            Contract.Ensures( Contract.Result<string>() != null );

            if ( site is IOleServiceProvider oleServiceProvider )
            {
                using ( var serviceProvider = new VisualStudioServiceProvider( oleServiceProvider ) )
                {
                    if ( serviceProvider.TryGetService( out IVSMDCodeDomProvider provider ) && provider.CodeDomProvider is CodeDomProvider codeProvider )
                    {
                        return ".g." + codeProvider.FileExtension;
                    }
                }
            }

            return Empty;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Exposed as the DefaultExtension property." )]
        int IVsSingleFileGenerator.DefaultExtension( out string pbstrDefaultExtension )
        {
            var ext = DefaultExtension;

            if ( ext.Length > 0 && ext[0] != '.' )
            {
                ext = "." + ext;
            }

            pbstrDefaultExtension = ext;
            return 0;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Debug assertions are used. VS will never pass null here." )]
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive. Disposed on the finally block. The CodeGeneratorContext will never fail. If it did, there's nothing to dispose anyway." )]
        int IVsSingleFileGenerator.Generate( string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress )
        {
            Debug.Assert( rgbOutputFileContents != null, "rgbOutputFileContents is null" );
            Debug.Assert( rgbOutputFileContents.Length > 0, "rgbOutputFileContents is zero-length" );

            var oleServiceProvider = site as IOleServiceProvider;
            var serviceProvider = oleServiceProvider == null ? ServiceProvider.Current : new VisualStudioServiceProvider( oleServiceProvider );
            var context = new CodeGeneratorContext( wszInputFilePath, bstrInputFileContents, wszDefaultNamespace, new VsProgressAdapter( pGenerateProgress ), serviceProvider );

            try
            {
                using ( var stream = Generate( context ) )
                {
                    var size = (int) stream.Length;
                    var buffer = default( byte[] );

                    if ( stream is MemoryStream memoryStream )
                    {
                        buffer = memoryStream.ToArray();
                    }
                    else
                    {
                        buffer = new byte[size];
                        stream.Read( buffer, 0, size );
                    }

                    rgbOutputFileContents[0] = AllocCoTaskMem( size );
                    Copy( buffer, 0, rgbOutputFileContents[0], size );
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
                if ( serviceProvider is IDisposable disposable )
                {
                    disposable.Dispose();
                }
            }

            return 0;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Only used or called by VS." )]
        void IObjectWithSite.GetSite( ref Guid riid, out IntPtr ppvSite )
        {
            if ( site == null )
            {
                ThrowExceptionForHR( NoInterface );
            }

            var iunknown = GetIUnknownForObject( site );
            var hr = QueryInterface( iunknown, ref riid, out ppvSite );

            Release( iunknown );
            ErrorHandler.ThrowOnFailure( hr );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Only used or called by VS." )]
        void IObjectWithSite.SetSite( object pUnkSite ) => site = pUnkSite;
    }
}