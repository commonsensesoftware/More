namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="OpenFileInteractionRequest"/>.
    /// </summary>
    public class OpenFileInteractionRequestTest
    {
        [Fact( DisplayName = "request should set continuation" )]
        public void RequestShouldSetContinuation()
        {
            // arrange
            var expected = "OpenFile";
            var interaction = new OpenFileInteraction();
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actual = (string) interaction.ContinuationData["Continuation"];

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
