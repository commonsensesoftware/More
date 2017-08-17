namespace More.Composition
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using Xunit;

    public class ConfigurationExportProviderTest
    {
        [Fact]
        public void get_export_descriptors_should_return_expected_promises()
        {
            // arrange
            var expected = "Test";
            var settings = new Dictionary<string, object>() { { "Setting1", expected } };
            var exportProvider = new ConfigurationExportProvider( ( key, type ) => settings[key] );
            var conventions = new ConventionBuilder();
            var configuration = new ContainerConfiguration();
            var setting = default( string );

            conventions.ForType<ComposedObject>().Export();
            configuration.WithPart<ComposedObject>( conventions );
            configuration.WithProvider( exportProvider );

            using ( var container = configuration.CreateContainer() )
            {
                // act
                setting = container.GetExport<ComposedObject>().Setting;
            }

            // assert
            setting.Should().Be( expected );
        }

        public class ComposedObject
        {
            public ComposedObject( [Setting( "Setting1" )] string setting ) => Setting = setting;

            public string Setting { get; }
        }
    }
}