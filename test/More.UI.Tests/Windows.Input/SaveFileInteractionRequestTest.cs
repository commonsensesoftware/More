namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SaveFileInteractionRequest"/>.
    /// </summary>
    public class SaveFileInteractionRequestTest
    {
        [Fact( DisplayName = "request should set continuation" )]
        public void RequestShouldSetContinuation()
        {
            // arrange
            var expected = "SaveFile";
            var interaction = new SaveFileInteraction();
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actual = (string) interaction.ContinuationData["Continuation"];

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
