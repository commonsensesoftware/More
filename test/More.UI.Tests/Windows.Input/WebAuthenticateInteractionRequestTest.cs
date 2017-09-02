namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class WebAuthenticateInteractionRequestTest
    {
        [Fact]
        public void request_should_set_continuation()
        {
            // arrange
            var expected = "WebAuthenticate";
            var requestUri = new Uri( "http://tempuri.org" );
            var interaction = new WebAuthenticateInteraction( requestUri );
            var interactionRequest = new WebAuthenticateInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var continuationData = (string) interaction.ContinuationData["Continuation"];

            // assert
            continuationData.Should().Be( expected );
        }
    }
}