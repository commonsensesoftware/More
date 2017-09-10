namespace More.Composition.Hosting
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Xunit;

    public class HostTest
    {
        [Fact]
        public void host_should_return_object_composed_with_settings()
        {
            // arrange
            var settings = new Dictionary<string, object>()
            {
                ["More.Composition.Hosting.HostTest+StubObject:message"] = "Test",
                ["More.Composition.Hosting.HostTest+StubObject:RetryCount"] = 42
            };
            var app = new App();
            var expected = new StubObject( "Test" ) { RetryCount = 42 };
            var stub = default( StubObject );

            app.Startup += ( s, e ) => app.Shutdown();

            using ( var host = new Host( ( key, type ) => settings[key] ) )
            {
                host.Configure( ( c, b ) => b.ForType<StubObject>().Export() );
                host.Run( app );

                // act
                stub = host.GetRequiredService<StubObject>();
            }

            // assert
            stub.ShouldBeEquivalentTo( new { Message = "Test", RetryCount = 42 } );
        }

        public class App : Application { }

        public class StubObject
        {
            public StubObject( [Configuration.Setting] string message ) => Message = message;

            public string Message { get; set; }

            [Configuration.Setting]
            public int RetryCount { get; set; }
        }
    }
}