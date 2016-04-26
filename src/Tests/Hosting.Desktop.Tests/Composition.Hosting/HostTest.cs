namespace More.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="Host"/>.
    /// </summary>
    public class HostTest
    {
        public class App : Application
        {
        }

        public class StubObject
        {
            public StubObject( [Configuration.Setting] string message )
            {
                Message = message;
            }

            public string Message { get; set; }

            [Configuration.Setting]
            public int RetryCount { get; set; }
        }

        [Fact( DisplayName = "host should return object composed with settings" )]
        public void HostShouldReturnObjectComposedWithSettings()
        {
            // arrange
            var settings = new Dictionary<string, object>()
            {
                { "More.Composition.Hosting.HostTest+StubObject:message", "Test" },
                { "More.Composition.Hosting.HostTest+StubObject:RetryCount", 42 }
            };
            var app = new App();
            var expected = new StubObject( "Test" ) { RetryCount = 42 };
            StubObject actual;

            app.Startup += ( s, e ) => app.Shutdown();

            using ( var host = new Host( key => settings[key] ) )
            {
                host.Configure( ( c, b ) => b.ForType<StubObject>().Export() );
                host.Run( app );

                // act
                actual = host.GetRequiredService<StubObject>();
            }

            // assert
            Assert.Equal( expected.Message, actual.Message );
            Assert.Equal( expected.RetryCount, actual.RetryCount );
        }
    }
}
