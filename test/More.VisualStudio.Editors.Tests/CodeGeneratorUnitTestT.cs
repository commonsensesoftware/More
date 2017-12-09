namespace More.VisualStudio.Editors
{
    using FluentAssertions;
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
    using static System.IntPtr;
    using IObjectWithSite = Microsoft.VisualStudio.OLE.Interop.IObjectWithSite;

    /// <summary>
    /// Represents the base implementation for a <see cref="CodeGenerator">code generator</see> unit test.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="CodeGenerator">code generator</see> to test.</typeparam>
    public abstract class CodeGeneratorUnitTest<T> where T : CodeGenerator, new()
    {
        const int NoInterface = -2147467262;
        readonly Dictionary<Guid, Lazy<object>> services = new Dictionary<Guid, Lazy<object>>();
        readonly StubIServiceProvider serviceProvider = new StubIServiceProvider();
        string fileExtension = "cs";

        protected CodeGeneratorUnitTest()
        {
            serviceProvider.QueryServiceGuidRefGuidRefIntPtrOut = QueryService;
            RegisterService( CreateCodeDomProvider );
        }

        int QueryService( ref Guid guidService, ref Guid riid, out IntPtr ppvObject )
        {
            ppvObject = Zero;

            if ( !services.TryGetValue( guidService, out var service ) )
            {
                return NoInterface;
            }

            ppvObject = Marshal.GetIUnknownForObject( service.Value );
            return 0;
        }

        IVSMDCodeDomProvider CreateCodeDomProvider()
        {
            var codeProvider = new Mock<CodeDomProvider>();
            var provider = new Mock<IVSMDCodeDomProvider>();
            var ext = LanguageFileExtension;

            // note: the CodeDomProvider does not include the leading period; strip it as necessary
            if ( ext[0] == '.' )
            {
                ext = ext.Substring( 1 );
            }

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
            IVsSingleFileGenerator generator = CreateCodeGenerator();
            var output = new IntPtr[1];
            var result = generator.Generate( filePath, fileContent, defaultNamespace, output, out var outputSize, progress );

            if ( result == 0 )
            {
                // if successful, ensure the pointer is not null and then free the allocated memory
                output[0].Should().NotBe( Zero );
                generatedContent = Marshal.PtrToStringAnsi( output[0], (int) outputSize );
                Marshal.FreeCoTaskMem( output[0] );
            }
            else
            {
                generatedContent = null;
            }

            return result;
        }

        [Fact]
        public virtual void default_extension_property_should_return_expected_value()
        {
            // arrange
            var expected = LanguageFileExtension;

            if ( expected[0] == '.' )
            {
                expected = ".g" + expected;
            }
            else
            {
                expected = ".g." + expected;
            }

            // act
            var generator = CreateCodeGenerator();

            // assert
            generator.DefaultExtension.Should().Be( expected );
        }

        [Fact]
        public virtual void default_extension_method_should_return_expected_value()
        {
            // arrange
            var expected = LanguageFileExtension;

            if ( expected[0] == '.' )
            {
                expected = ".g" + expected;
            }
            else
            {
                expected = ".g." + expected;
            }

            IVsSingleFileGenerator generator = CreateCodeGenerator();

            // act
            var result = generator.DefaultExtension( out var defaultExtension );

            // assert
            result.Should().Be( 0 );
            defaultExtension.Should().Be( expected );
        }
    }
}