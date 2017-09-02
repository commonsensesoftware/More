namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using Xunit;

    public class ContinuableInteractionRequestT1T2Test
    {
        [Fact]
        public void request_should_set_continuation_id()
        {
            // arrange
            var interaction = new Interaction();
            var interactionRequest = new ContinuableInteractionRequest<Interaction, object>( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );

            // assert
            interaction.ContinuationData["ContinuationId"].Should().Be( 0L );
        }

        [Fact]
        public void new_continuable_request_should_set_id()
        {
            // arrange
            var id = "Test";

            // act
            var request = new ContinuableInteractionRequest<Interaction, object>( id, DefaultAction.None );

            // assert
            request.Id.Should().Be( id );
        }

        [Fact]
        public void new_continuable_request_should_set_id_and_register_with_continuation_manager()
        {
            // arrange
            var id = "Test";
            var continuationData = 42L;
            var continuationManager = new Mock<IContinuationManager>();
            var interaction = new Interaction();

            continuationManager.Setup( cm => cm.Register( It.IsAny<Action<object>>() ) ).Returns( continuationData );

            var interactionRequest = new ContinuableInteractionRequest<Interaction, object>( id, continuationManager.Object, DefaultAction.None );

            // act
            interactionRequest.Request( interaction );

            // assert
            interactionRequest.Id.Should().Be( id );
            interaction.ContinuationData["ContinuationId"].Should().Be( continuationData );
        }
    }
}