namespace More
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="WeakEventManager{THandler,TArgs}"/>.
    /// </summary>
    public class WeakEventManagerT1T2Test
    {
        [Fact( DisplayName = "weak delegate manager should not allow non-delegate" )]
        public void ConstructorShouldThrowExceptionIfHandlerTypeParameterIsNotADelegate()
        {
            Assert.Throws<InvalidOperationException>( () => new WeakEventManager<string, EventArgs>() );
        }

        [Fact( DisplayName = "remove handler should remove managed delegate" )]
        public void RemoveHandlerShouldRemoveAddedHandler()
        {
            var target = new WeakEventManager<EventHandler, EventArgs>();
            EventHandler handler = DefaultAction.None;
            target.AddHandler( handler );
            Assert.True( target.RemoveHandler( handler ) );
        }

        [Fact( DisplayName = "remove handler should return false for unmanaged delegate" )]
        public void RemoveHandlerShouldNotRemoveUnaddedHandler()
        {
            var target = new WeakEventManager<EventHandler, EventArgs>();
            EventHandler handler1 = ( s, e ) =>
            {
            };
            EventHandler handler2 = ( s, e ) =>
            {
            };
            target.AddHandler( handler1 );
            Assert.False( target.RemoveHandler( handler2 ) );
        }

        [Fact( DisplayName = "raise event should invoke managed handler" )]
        public void RaiseEventShouldInvokeAddedHandler()
        {   var raised = false;
            EventHandler handler = ( s, e ) => raised = true;
            var target = new WeakEventManager<EventHandler, EventArgs>();
            target.AddHandler( handler );
            target.RaiseEvent( this, EventArgs.Empty );
            Assert.True( raised );
        }
    }
}
