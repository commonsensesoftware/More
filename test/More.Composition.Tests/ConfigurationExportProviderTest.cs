namespace More.Composition
{
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ConfigurationExportProvider"/>.
    /// </summary>
    public class ConfigurationExportProviderTest
    {
        public class ComposedObject
        {
            public ComposedObject( [Setting( "Setting1" )] string setting )
            {
                Setting = setting;
            }

            public string Setting { get; }
        }

        [Fact( DisplayName = "get export descriptors should return expected promises" )]
        public void GetExportDescriptorsShouldReturnExpectedPromises()
        {
            // arrange
            var expected = "Test";
            var settings = new Dictionary<string, object>() { { "Setting1", expected } };
            var exportProvider = new ConfigurationExportProvider( ( key, type ) => settings[key] );
            var conventions = new ConventionBuilder();
            var configuration = new ContainerConfiguration();
            string actual = null;

            conventions.ForType<ComposedObject>().Export();
            configuration.WithPart<ComposedObject>( conventions );
            configuration.WithProvider( exportProvider );

            using ( var container = configuration.CreateContainer() )
            {
                // act
                actual = container.GetExport<ComposedObject>().Setting;
            }

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
