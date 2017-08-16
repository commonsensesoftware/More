namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.ComponentModel;
    using System.Threading;
    using Xunit;
    using static System.Threading.SynchronizationContext;

    public class EventBrokerTest : IDisposable
    {
        [Fact]
        public void publish_should_signal_subscriber()
        {
            // arrange
            var eventBroker = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            handler.Setup( f => f( It.IsAny<string>(), It.IsAny<object>(), It.IsAny<EventArgs>() ) );

            // act
            eventBroker.Subscribe( "Test", handler.Object, new SyncOnlyContext() );
            eventBroker.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }

        [Fact]
        public void publish_should_signal_subscriber_with_default_synchronization_context()
        {
            // arrange
            var eventBroker = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            handler.Setup( f => f( It.IsAny<string>(), It.IsAny<object>(), It.IsAny<EventArgs>() ) );

            // act
            eventBroker.Subscribe( "Test", handler.Object );
            eventBroker.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }

        [Fact]
        public void publish_should_signal_subscriber_with_covariant_handler()
        {
            // arrange
            var eventBroker = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();
            var eventArgs = new PropertyChangedEventArgs( null );

            // act
            eventBroker.Subscribe( "Test", handler.Object, new SyncOnlyContext() );
            eventBroker.Publish( "Test", null, eventArgs );

            // assert
            handler.Verify( f => f( "Test", null, eventArgs ), Times.Once() );
        }

        [Fact]
        public void publish_should_not_signal_subscriber_with_contravariant_handler()
        {
            // arrange
            var eventBroker = new EventBroker();
            var handler = new Mock<Action<string, object, PropertyChangedEventArgs>>();

            // act
            eventBroker.Subscribe( "Test", handler.Object, new SyncOnlyContext() );
            eventBroker.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, It.IsAny<PropertyChangedEventArgs>() ), Times.Never() );
        }

        [Fact]
        public void publish_should_not_signal_subscriber_after_unsubscribe()
        {
            // arrange
            var eventBroker = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            eventBroker.Subscribe( "Test", handler.Object, new SyncOnlyContext() );

            // act
            eventBroker.Publish( "Test", null, EventArgs.Empty );
            eventBroker.Unsubscribe( "Test", handler.Object );
            eventBroker.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }

        bool disposed;
        SynchronizationContext currentContext = Current;

        ~EventBrokerTest() => Dispose( false );

        public EventBrokerTest() => SetSynchronizationContext( new SyncOnlyContext() );

        void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( disposing )
            {
                SetSynchronizationContext( currentContext );
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        sealed class SyncOnlyContext : SynchronizationContext
        {
            public override void Post( SendOrPostCallback d, object state ) => Send( d, state );
        }
    }
}