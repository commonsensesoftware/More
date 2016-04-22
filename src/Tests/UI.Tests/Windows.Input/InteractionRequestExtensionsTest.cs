namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="InteractionRequestExtensions"/>.
    /// </summary>
    public class InteractionRequestExtensionsTest
    {
        [Fact( DisplayName = "alert async should request title and message" )]
        public async Task AlertAsyncShouldReturnTitleAndMessage()
        {
            // arrange
            var expectedTitle = "Test";
            var expectedMessage = "This is a test.";
            var expectedButton = "OK";
            string actualTitle = null;
            string actualMessage = null;
            string actualButton =  null;
            var interactionRequest = new InteractionRequest<Interaction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualMessage = (string) e.Interaction.Content;
                actualButton = e.Interaction.DefaultCommand.Name;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            await interactionRequest.AlertAsync( expectedTitle, expectedMessage );

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedMessage, actualMessage );
            Assert.Equal( expectedButton, actualButton );
        }

        [Fact( DisplayName = "alert async should request title, message, and button" )]
        public async Task AlertAsyncShouldReturnTitleMessageAndButton()
        {
            // arrange
            var expectedTitle = "Test";
            var expectedMessage = "This is a test.";
            var expectedButton = "Accept";
            string actualTitle = null;
            string actualMessage = null;
            string actualButton = null;
            var interactionRequest = new InteractionRequest<Interaction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualMessage = (string) e.Interaction.Content;
                actualButton = e.Interaction.DefaultCommand.Name;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            await interactionRequest.AlertAsync( expectedTitle, expectedMessage, expectedButton );

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedMessage, actualMessage );
            Assert.Equal( expectedButton, actualButton );
        }

        [Fact( DisplayName = "confirm async should request prompt" )]
        public async Task ConfirmAsyncShouldRequestPrompt()
        {
            // arrange
            var expectedTitle = string.Empty;
            var expectedPrompt = "Continue?";
            var expectedAccept = "OK";
            var expectedCancel = "Cancel";
            string actualTitle = null;
            string actualPrompt = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<Interaction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualPrompt = (string) e.Interaction.Content;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            await interactionRequest.ConfirmAsync( expectedPrompt );

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "confirm async should request prompt and title" )]
        public async Task ConfirmAsyncShouldRequestPromptAndTitle()
        {
            // arrange
            var expectedTitle = "Question";
            var expectedPrompt = "Continue?";
            var expectedAccept = "OK";
            var expectedCancel = "Cancel";
            string actualTitle = null;
            string actualPrompt = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<Interaction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualPrompt = (string) e.Interaction.Content;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            await interactionRequest.ConfirmAsync( expectedPrompt, expectedTitle );

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "confirm async should request title, prompt, and buttons" )]
        public async Task ConfirmAsyncShouldRequestTitlePromptAndButtons()
        {
            // arrange
            var expectedTitle = "Question";
            var expectedPrompt = "Are you sure?";
            var expectedAccept = "Yes";
            var expectedCancel = "No";
            string actualTitle = null;
            string actualPrompt = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<Interaction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualPrompt = (string) e.Interaction.Content;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            await interactionRequest.ConfirmAsync( expectedPrompt, expectedTitle, expectedAccept, expectedCancel );

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }
    }
}
