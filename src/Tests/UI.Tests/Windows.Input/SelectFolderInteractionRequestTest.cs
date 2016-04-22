namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SelectFolderInteractionRequest"/>.
    /// </summary>
    public class SelectFolderInteractionRequestTest
    {
        [Fact( DisplayName = "request should set continuation" )]
        public void RequestShouldSetContinuation()
        {
            // arrange
            var expected = "SelectFolder";
            var interaction = new SelectFolderInteraction();
            var interactionRequest = new SelectFolderInteractionRequest( DefaultAction.None );

            // act
            interactionRequest.Request( interaction );
            var actual = (string) interaction.ContinuationData["Continuation"];

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
