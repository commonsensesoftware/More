namespace More.Windows.Input
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ContinuableInteractionRequest{TInteraction,TArg}"/>.
    /// </summary>
    public class ContinuableInteractionRequestT1T2Test
    {
        [Fact( DisplayName = "request should set continuation id" )]
        public void RequestShouldSetContinuationId()
        {
            // arrange
            var expected = 0L;
            var interaction = new Interaction();
            var interactionRequest = new ContinuableInteractionRequest<Interaction, object>( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actual = (long) interaction.ContinuationData["ContinuationId"];

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new continuable request should set id" )]
        public void ConstructorShouldSetId()
        {
            // arrange
            var expected = "Test";
            var request = new ContinuableInteractionRequest<Interaction, object>( expected, DefaultAction.None );

            // act
            var actual = request.Id;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new continuable request should set id and register with continuation manager" )]
        public void ConstructorShouldSetIdAndRegisterWithContinuationManager()
        {
            // arrange
            var expectedId = "Test";
            var expectedContinuationId = 42L;
            var continuationManager = new Mock<IContinuationManager>();

            continuationManager.Setup( cm => cm.Register( It.IsAny<Action<object>>() ) ).Returns( expectedContinuationId );

            var interaction = new Interaction();
            var interactionRequest = new ContinuableInteractionRequest<Interaction, object>( expectedId, continuationManager.Object, DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actualId = interactionRequest.Id;
            var actualContinuationId = (long) interaction.ContinuationData["ContinuationId"];

            // assert
            Assert.Equal( expectedId, actualId );
            Assert.Equal( expectedContinuationId, actualContinuationId );
        }
    }
}
