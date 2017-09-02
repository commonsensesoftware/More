namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class SaveFileInteractionRequestTest
    {
        [Fact]
        public void request_should_set_continuation()
        {
            // arrange
            var expected = "SaveFile";
            var interaction = new SaveFileInteraction();
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var continuationData = (string) interaction.ContinuationData["Continuation"];

            // assert
            continuationData.Should().Be( expected );
        }
    }
}