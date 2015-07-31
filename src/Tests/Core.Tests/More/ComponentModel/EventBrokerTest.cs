namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.ComponentModel;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="EventBroker"/>.
    /// </summary>
    public class EventBrokerTest : IDisposable
    {
        private sealed class SyncOnlyContext : SynchronizationContext
        {
            public override void Post( SendOrPostCallback d, object state )
            {
                Send( d, state );
            }
        }

        private bool disposed;
        private SynchronizationContext currentContext;

        ~EventBrokerTest()
        {
            Dispose( false );
        }

        public EventBrokerTest()
        {
            currentContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext( new SyncOnlyContext() );
        }

        private void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( disposing )
                SynchronizationContext.SetSynchronizationContext( currentContext );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        [Fact( DisplayName = "publish should signal subscriber" )]
        public void PublishShouldSignalSubscriber()
        {
            // arrange
            var target = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            handler.Setup( f => f( It.IsAny<string>(), It.IsAny<object>(), It.IsAny<EventArgs>() ) );

            // act
            target.Subscribe<EventArgs>( "Test", handler.Object, new SyncOnlyContext() );
            target.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }

        [Fact( DisplayName = "publish should signal subscriber" )]
        public void PublishShouldSignalSubscriberUsingDefaultSynchronizationContext()
        {
            // arrange
            var target = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            handler.Setup( f => f( It.IsAny<string>(), It.IsAny<object>(), It.IsAny<EventArgs>() ) );

            // act
            target.Subscribe<EventArgs>( "Test", handler.Object );
            target.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }

        [Fact( DisplayName = "publish should signal subscriber with covariant handler" )]
        public void PublishShouldSignalSubscriberWithCovariantEventHandler()
        {
            // arrange
            var target = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();
            var eventArgs = new PropertyChangedEventArgs( null );

            // act
            target.Subscribe<EventArgs>( "Test", handler.Object, new SyncOnlyContext() );
            target.Publish( "Test", null, eventArgs );

            // assert
            handler.Verify( f => f( "Test", null, eventArgs ), Times.Once() );
        }

        [Fact( DisplayName = "publish should not signal subscriber with contravariant handler" )]
        public void PublishShouldNotSignalSubscriberWithContravariantEventHandler()
        {
            // arrange
            var target = new EventBroker();
            var handler = new Mock<Action<string, object, PropertyChangedEventArgs>>();

            // act
            target.Subscribe<PropertyChangedEventArgs>( "Test", handler.Object, new SyncOnlyContext() );
            target.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, It.IsAny<PropertyChangedEventArgs>() ), Times.Never() );
        }

        [Fact( DisplayName = "publish should not signal subscriber after unsubscribe" )]
        public void PublishShouldNotSignalSubscriberAfterUnsubscribe()
        {
            // arrange
            var target = new EventBroker();
            var handler = new Mock<Action<string, object, EventArgs>>();

            target.Subscribe<EventArgs>( "Test", handler.Object, new SyncOnlyContext() );
            
            // act
            target.Publish( "Test", null, EventArgs.Empty );
            target.Unsubscribe( "Test", handler.Object );
            target.Publish( "Test", null, EventArgs.Empty );

            // assert
            handler.Verify( f => f( "Test", null, EventArgs.Empty ), Times.Once() );
        }
    }
}
