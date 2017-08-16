namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="WebAuthenticateInteractionRequest"/>.
    /// </summary>
    public class WebAuthenticateInteractionRequestTest
    {
        [Fact( DisplayName = "request should set continuation" )]
        public void RequestShouldSetContinuation()
        {
            // arrange
            var expected = "WebAuthenticate";
            var requestUri = new Uri( "http://tempuri.org" );
            var interaction = new WebAuthenticateInteraction( requestUri );
            var interactionRequest = new WebAuthenticateInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actual = (string) interaction.ContinuationData["Continuation"];

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
