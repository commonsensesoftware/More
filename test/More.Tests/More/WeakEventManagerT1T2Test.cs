namespace More
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class WeakEventManagerT1T2Test
    {
        [Fact]
        public void weak_delegate_manager_should_not_allow_nonX2Ddelegate()
        {
            // arrange

            // act
            Action @new = () => new WeakEventManager<string, EventArgs>();

            // assert
            @new.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void remove_handler_should_remove_managed_delegate()
        {
            // arrange
            var manager = new WeakEventManager<EventHandler, EventArgs>();
            EventHandler handler = DefaultAction.None;

            manager.AddHandler( handler );

            // act
            var removed = manager.RemoveHandler( handler );

            // assert
            removed.Should().BeTrue();
        }

        [Fact]
        public void remove_handler_should_return_false_for_unmanaged_delegate()
        {
            // arrange
            var manager = new WeakEventManager<EventHandler, EventArgs>();
            EventHandler handler1 = ( s, e ) => { };
            EventHandler handler2 = ( s, e ) => { };

            manager.AddHandler( handler1 );

            // act
            var removed = manager.RemoveHandler( handler2 );

            // assert
            removed.Should().BeFalse();
        }

        [Fact]
        public void raise_event_should_invoke_managed_handler()
        {
            // arrange
            var manager = new WeakEventManager<EventHandler, EventArgs>();
            var raised = false;
            EventHandler handler = ( s, e ) => raised = true;

            manager.AddHandler( handler );

            // act
            manager.RaiseEvent( this, EventArgs.Empty );

            // assert
            raised.Should().BeTrue();
        }
    }
}