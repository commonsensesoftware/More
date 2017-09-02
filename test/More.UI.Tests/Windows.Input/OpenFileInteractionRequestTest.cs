namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class OpenFileInteractionRequestTest
    {
        [Fact]
        public void request_should_set_continuation()
        {
            // arrange
            var expected = "OpenFile";
            var interaction = new OpenFileInteraction();
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var continuationData = (string) interaction.ContinuationData["Continuation"];

            // assert
            continuationData.Should().Be( expected );
        }
    }
}