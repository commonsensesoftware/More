namespace More.VisualStudio.Editors
{
    using Microsoft.VisualStudio.Designer.Interfaces;
    using Microsoft.VisualStudio.OLE.Interop.Fakes;
    using Microsoft.VisualStudio.Shell.Interop;
    using Moq;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Xunit;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;

    /// <summary>
    /// Represents the base implementation for a <see cref="CodeGenerator">code generator</see> unit test.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="CodeGenerator">code generator</see> to test.</typeparam>
    public abstract class CodeGeneratorUnitTest<T> where T : CodeGenerator, new()
    {
        private const int NoInterface = -2147467262;
        private readonly Dictionary<Guid, Lazy<object>> services = new Dictionary<Guid, Lazy<object>>();
        private readonly StubIServiceProvider serviceProvider = new StubIServiceProvider();
        private string fileExtension = "cs";

        protected CodeGeneratorUnitTest()
        {
            serviceProvider.QueryServiceGuidRefGuidRefIntPtrOut = QueryService;
            RegisterService( CreateCodeDomProvider );
        }

        private int QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject )
        {
            ppvObject = IntPtr.Zero;

            Lazy<object> service;

            if ( !services.TryGetValue( guidService, out service ) )
                return NoInterface;

            ppvObject = Marshal.GetIUnknownForObject( service.Value );
            return 0;
        }

        private IVSMDCodeDomProvider CreateCodeDomProvider()
        {
            var codeProvider = new Mock<CodeDomProvider>();
            var provider = new Mock<IVSMDCodeDomProvider>();
            var ext = LanguageFileExtension;

            // the CodeDomProvider does not include the leading period; strip it as necessary
            if ( ext[0] == '.' )
                ext = ext.Substring( 1 );

            codeProvider.SetupGet( cp => cp.FileExtension ).Returns( ext );
            provider.SetupGet( p => p.CodeDomProvider ).Returns( codeProvider.Object );

            return provider.Object;
        }

        protected string LanguageFileExtension
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( fileExtension ) );
                return fileExtension;
            }
            set
            {
                Contract.Requires( !string.IsNullOrEmpty( value ) );
                fileExtension = value;
            }
        }

        protected void RegisterService<TService>( TService service )
        {
            Contract.Requires( service != null );
            services[typeof( TService ).GUID] = new Lazy<object>( () => service );
        }

        protected void RegisterService<TService>( Func<TService> activator )
        {
            Contract.Requires( activator != null );
            var ctor = activator;
            services[typeof( TService ).GUID] = new Lazy<object>( () => ctor() );
        }

        protected T CreateCodeGenerator()
        {
            var instance = new T();
            IObjectWithSite target = instance;
            target.SetSite( serviceProvider );
            return instance;
        }

        protected int Generate( string filePath, string fileContent, string defaultNamespace, out string generatedContent )
        {
            var progress = new Mock<IVsGeneratorProgress>().Object;
            return Generate( filePath, fileContent, defaultNamespace, progress, out generatedContent );
        }

        protected int Generate( string filePath, string fileContent, string defaultNamespace, IVsGeneratorProgress progress, out string generatedContent )
        {
            generatedContent = null;

            IVsSingleFileGenerator generator = CreateCodeGenerator();
            var output = new IntPtr[1];
            var outputSize = 0U;
            var result = generator.Generate( filePath, fileContent, defaultNamespace, output, out outputSize, progress );

            if ( result == 0 )
            {
                // if successful, ensure the pointer is not null and then free the allocated memory
                Assert.NotEqual( IntPtr.Zero, output[0] );
                generatedContent = Marshal.PtrToStringAnsi( output[0], (int) outputSize );
                Marshal.FreeCoTaskMem( output[0] );
            }

            return result;
        }

        [Fact]
        public virtual void DefaultExtensionPropertyShouldReturnExpectedValue()
        {
            var expected = LanguageFileExtension;

            if ( expected[0] == '.' )
                expected = ".g" + expected;
            else
                expected = ".g." + expected;

            var generator = CreateCodeGenerator();
            var actual = generator.DefaultExtension;

            Assert.Equal( expected, actual );
        }

        [Fact]
        public virtual void DefaultExtensionMethodShouldReturnExpectedValue()
        {
            var expected = LanguageFileExtension;

            if ( expected[0] == '.' )
                expected = ".g" + expected;
            else
                expected = ".g." + expected;

            IVsSingleFileGenerator generator = CreateCodeGenerator();
            string actual;
            var result = generator.DefaultExtension( out actual );

            Assert.Equal( 0, result );
            Assert.Equal( expected, actual );
        }
    }
}
