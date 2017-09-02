namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class SelectFolderInteractionRequestTest
    {
        [Fact]
        public void request_should_set_continuation()
        {
            // arrange
            var expected = "SelectFolder";
            var interaction = new SelectFolderInteraction();
            var interactionRequest = new SelectFolderInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var continuationData = (string) interaction.ContinuationData["Continuation"];

            // assert
            continuationData.Should().Be( expected );
        }
    }
}