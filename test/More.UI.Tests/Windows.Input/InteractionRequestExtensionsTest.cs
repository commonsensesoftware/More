namespace More.Windows.Input
{
    using IO;
    using Moq;
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
            var response = await interactionRequest.ConfirmAsync( expectedPrompt );

            // assert
            Assert.True( response );
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
                e.Interaction.ExecuteCancelCommand();
            };

            // act
            var response = await interactionRequest.ConfirmAsync( expectedPrompt, expectedTitle );

            // assert
            Assert.False( response );
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
            var response = await interactionRequest.ConfirmAsync( expectedPrompt, expectedTitle, expectedAccept, expectedCancel );

            // assert
            Assert.True( response );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request single file async should request open files with file filter" )]
        public async Task RequestSingleFileAsyncShouldRequestOpenFilesWithFileFilter()
        {
            // arrange
            var expectedTitle = "Open File";
            var expectedAccept = "Open";
            var expectedCancel = "Cancel";
            var expectedFileFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileFilter = null;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileFilter = interaction.FileTypeFilter;
                interaction.Files.Add( expectedFile );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFile = await interactionRequest.RequestSingleFileAsync( expectedFileFilter );

            // assert
            Assert.Equal( expectedFile, actualFile );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileFilter.Single().Name, actualFileFilter.Single().Name );
            Assert.True( expectedFileFilter.Single().Extensions.SequenceEqual( actualFileFilter.Single().Extensions ) );
        }

        [Fact( DisplayName = "request single file async should request open files with title and file filter" )]
        public async Task RequestSingleFileAsyncShouldRequestOpenFilesWithTitleAndFileFilter()
        {
            // arrange
            var expectedTitle = "Open";
            var expectedAccept = "Open";
            var expectedCancel = "Cancel";
            var expectedFileFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileFilter = null;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileFilter = interaction.FileTypeFilter;
                interaction.Files.Add( expectedFile );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFile = await interactionRequest.RequestSingleFileAsync( expectedTitle, expectedFileFilter );

            // assert
            Assert.Equal( expectedFile, actualFile );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileFilter.Single().Name, actualFileFilter.Single().Name );
            Assert.True( expectedFileFilter.Single().Extensions.SequenceEqual( actualFileFilter.Single().Extensions ) );
        }

        [Fact( DisplayName = "request multiple files async should request open files with file filter" )]
        public async Task RequestMultipleFilesAsyncShouldRequestOpenFilesWithFileFilter()
        {
            // arrange
            var expectedTitle = "Open File";
            var expectedAccept = "Open";
            var expectedCancel = "Cancel";
            var expectedFileFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileFilter = null;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileFilter = interaction.FileTypeFilter;
                interaction.Files.Add( expectedFile );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFiles = await interactionRequest.RequestMultipleFilesAsync( expectedFileFilter );

            // assert
            Assert.Equal( expectedFile, actualFiles.Single() );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileFilter.Single().Name, actualFileFilter.Single().Name );
            Assert.True( expectedFileFilter.Single().Extensions.SequenceEqual( actualFileFilter.Single().Extensions ) );
        }

        [Fact( DisplayName = "request multiple files async should request open files with title and file filter" )]
        public async Task RequestMultipleFilesAsyncShouldRequestOpenFilesWithTitleAndFileFilter()
        {
            // arrange
            var expectedTitle = "Open";
            var expectedAccept = "Open";
            var expectedCancel = "Cancel";
            var expectedFileFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileFilter = null;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileFilter = interaction.FileTypeFilter;
                interaction.Files.Add( expectedFile );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFiles = await interactionRequest.RequestMultipleFilesAsync( expectedTitle, expectedFileFilter );

            // assert
            Assert.Equal( expectedFile, actualFiles.Single() );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileFilter.Single().Name, actualFileFilter.Single().Name );
            Assert.True( expectedFileFilter.Single().Extensions.SequenceEqual( actualFileFilter.Single().Extensions ) );
        }

        [Fact( DisplayName = "request async should request open files with title, buttons, and file filter" )]
        public async Task RequestAsyncShouldRequestOpenFilesWithTitleButtonsAndFileFilter()
        {
            // arrange
            var expectedTitle = "Open";
            var expectedAccept = "Select";
            var expectedCancel = "Close";
            var expectedFileFilter = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileFilter = null;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileFilter = interaction.FileTypeFilter;
                interaction.Files.Add( expectedFile );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFiles = await interactionRequest.RequestAsync( expectedTitle, expectedAccept, expectedCancel, expectedFileFilter, false );

            // assert
            Assert.Equal( expectedFile, actualFiles.Single() );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileFilter.Single().Name, actualFileFilter.Single().Name );
            Assert.True( expectedFileFilter.Single().Extensions.SequenceEqual( actualFileFilter.Single().Extensions ) );
        }

        [Fact( DisplayName = "request async should request save files with file choices" )]
        public async Task RequestAsyncShouldRequestSaveFileWithFileChoices()
        {
            // arrange
            var expectedTitle = "Save File";
            var expectedAccept = "Save";
            var expectedCancel = "Cancel";
            var expectedFileChoices = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileChoices = null;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileChoices = interaction.FileTypeChoices;
                interaction.SavedFile = expectedFile;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFile = await interactionRequest.RequestAsync( expectedFileChoices );

            // assert
            Assert.Equal( expectedFile, actualFile );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileChoices.Single().Name, actualFileChoices.Single().Name );
            Assert.True( expectedFileChoices.Single().Extensions.SequenceEqual( actualFileChoices.Single().Extensions ) );
        }

        [Fact( DisplayName = "request async should request save file with title and file choices" )]
        public async Task RequestAsyncShouldRequestSaveFileWithTitleAndFileChoices()
        {
            // arrange
            var expectedTitle = "Save";
            var expectedAccept = "Save";
            var expectedCancel = "Cancel";
            var expectedFileChoices = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileChoices = null;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileChoices = interaction.FileTypeChoices;
                interaction.SavedFile = expectedFile;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFile = await interactionRequest.RequestAsync( expectedTitle, expectedFileChoices );

            // assert
            Assert.Equal( expectedFile, actualFile );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileChoices.Single().Name, actualFileChoices.Single().Name );
            Assert.True( expectedFileChoices.Single().Extensions.SequenceEqual( actualFileChoices.Single().Extensions ) );
        }

        [Fact( DisplayName = "request async should request save file with title, buttons, and file choices" )]
        public async Task RequestAsyncShouldRequestSaveFileWithTitleButtonsAndFileChoices()
        {
            // arrange
            var expectedTitle = "Save";
            var expectedAccept = "Select";
            var expectedCancel = "Close";
            var expectedFileChoices = new[] { new FileType( "Text Files", ".txt" ) };
            var expectedFile = new Mock<IFile>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            IEnumerable<FileType> actualFileChoices = null;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) e.Interaction;

                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                actualFileChoices = interaction.FileTypeChoices;
                interaction.SavedFile = expectedFile;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFile = await interactionRequest.RequestAsync( expectedTitle, expectedAccept, expectedCancel, expectedFileChoices );

            // assert
            Assert.Equal( expectedFile, actualFile );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
            Assert.Equal( expectedFileChoices.Single().Name, actualFileChoices.Single().Name );
            Assert.True( expectedFileChoices.Single().Extensions.SequenceEqual( actualFileChoices.Single().Extensions ) );
        }

        [Fact( DisplayName = "request async should request select folder" )]
        public async Task RequestAsyncShouldRequestSelectFolder()
        {
            // arrange
            var expectedTitle = "Select Folder";
            var expectedAccept = "Select";
            var expectedCancel = "Cancel";
            var expectedFolder = new Mock<IFolder>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                ( (SelectFolderInteraction) e.Interaction ).Folder = expectedFolder;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFolder = await interactionRequest.RequestAsync();

            // assert
            Assert.Equal( expectedFolder, actualFolder );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request select folder with title" )]
        public async Task RequestAsyncShouldRequestSelectFolderWithTitle()
        {
            // arrange
            var expectedTitle = "Pick Folder";
            var expectedAccept = "Select";
            var expectedCancel = "Cancel";
            var expectedFolder = new Mock<IFolder>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                ( (SelectFolderInteraction) e.Interaction ).Folder = expectedFolder;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFolder = await interactionRequest.RequestAsync( expectedTitle );

            // assert
            Assert.Equal( expectedFolder, actualFolder );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request select folder with title and buttons" )]
        public async Task RequestAsyncShouldRequestSelectFolderWithTitleAndButtons()
        {
            // arrange
            var expectedTitle = "Select";
            var expectedAccept = "Pick";
            var expectedCancel = "Close";
            var expectedFolder = new Mock<IFolder>().Object;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                actualTitle = e.Interaction.Title;
                actualAccept = e.Interaction.DefaultCommand.Name;
                actualCancel = e.Interaction.CancelCommand.Name;
                ( (SelectFolderInteraction) e.Interaction ).Folder = expectedFolder;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualFolder = await interactionRequest.RequestAsync( expectedTitle, expectedAccept, expectedCancel );

            // assert
            Assert.Equal( expectedFolder, actualFolder );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request text input with prompt" )]
        public async Task RequestAsyncShouldRequestTextInputWithPrompt()
        {
            // arrange
            var expectedPrompt = "Input Box";
            var expectedTitle = "";
            var expectedAccept = "OK";
            var expectedCancel = "Cancel";
            var expectedResponse = "Test";
            string actualPrompt = null;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<TextInputInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) e.Interaction;

                actualPrompt = (string) interaction.Content;
                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                interaction.Response = expectedResponse;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualResponse = await interactionRequest.RequestAsync( expectedPrompt );

            // assert
            Assert.Equal( expectedResponse, actualResponse );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request text input with prompt and title" )]
        public async Task RequestAsyncShouldRequestTextInputWithPromptAndTitle()
        {
            // arrange
            var expectedPrompt = "Input Box";
            var expectedTitle = "Enter something:";
            var expectedAccept = "OK";
            var expectedCancel = "Cancel";
            var expectedResponse = "Test";
            string actualPrompt = null;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<TextInputInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) e.Interaction;

                actualPrompt = (string) interaction.Content;
                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                interaction.Response = expectedResponse;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualResponse = await interactionRequest.RequestAsync( expectedTitle, expectedPrompt );

            // assert
            Assert.Equal( expectedResponse, actualResponse );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request text input with prompt, title, and default response" )]
        public async Task RequestAsyncShouldRequestTextInputWithPromptTitleAndDefaultResponse()
        {
            // arrange
            var expectedPrompt = "Input Box";
            var expectedTitle = "Enter something:";
            var expectedAccept = "OK";
            var expectedCancel = "Cancel";
            var expectedResponse = "Test";
            string actualPrompt = null;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<TextInputInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) e.Interaction;

                actualPrompt = (string) interaction.Content;
                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                interaction.Response = interaction.DefaultResponse;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualResponse = await interactionRequest.RequestAsync( expectedTitle, expectedPrompt, expectedResponse );

            // assert
            Assert.Equal( expectedResponse, actualResponse );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request text input with prompt, title, default response, and buttons" )]
        public async Task RequestAsyncShouldRequestTextInputWithPromptTitleDefaultResponseAndButtons()
        {
            // arrange
            var expectedPrompt = "Input Box";
            var expectedTitle = "Enter something:";
            var expectedAccept = "Accept";
            var expectedCancel = "Close";
            var expectedResponse = "Test";
            string actualPrompt = null;
            string actualTitle = null;
            string actualAccept = null;
            string actualCancel = null;
            var interactionRequest = new InteractionRequest<TextInputInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) e.Interaction;

                actualPrompt = (string) interaction.Content;
                actualTitle = interaction.Title;
                actualAccept = interaction.DefaultCommand.Name;
                actualCancel = interaction.CancelCommand.Name;
                interaction.Response = interaction.DefaultResponse;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var actualResponse = await interactionRequest.RequestAsync(
                                            expectedTitle,
                                            expectedPrompt, 
                                            expectedResponse,
                                            expectedAccept,
                                            expectedCancel );

            // assert
            Assert.Equal( expectedResponse, actualResponse );
            Assert.Equal( expectedPrompt, actualPrompt );
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedAccept, actualAccept );
            Assert.Equal( expectedCancel, actualCancel );
        }

        [Fact( DisplayName = "request async should request authentication with request uri and callback uri" )]
        public async Task RequestAsyncShouldRequestAuthenticationWithRequestUriAndCallbackUri()
        {
            // arrange
            var expectedRequestUri = new Uri( "http://tempuri.org" );
            var expectedCallbackUri = new Uri( "http://tempuri.org/callback" );
            var expectedResult = new Mock<IWebAuthenticationResult>().Object;
            Uri actualRequestUri = null;
            Uri actualCallbackUri = null;
            var interactionRequest = new InteractionRequest<WebAuthenticateInteraction>();

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (WebAuthenticateInteraction) e.Interaction;

                actualRequestUri = interaction.RequestUri;
                actualCallbackUri = interaction.CallbackUri;
                interaction.DefaultCommand.Execute( expectedResult );
            };

            // act
            var actualResult = await interactionRequest.RequestAsync( expectedRequestUri, expectedCallbackUri );

            // assert
            Assert.Equal( expectedResult, actualResult );
            Assert.Equal( expectedRequestUri, actualRequestUri );
            Assert.Equal( expectedCallbackUri, actualCallbackUri );
        }

        [Fact( DisplayName = "request async should replace user-defined commands" )]
        public async Task RequestAsyncShouldReplaceUserDefinedCommands()
        {
            // arrange
            var interaction = new WebAuthenticateInteraction( new Uri( "http://tempuri.org" ) )
            {
                DefaultCommandIndex = 0,
                Commands =
                {
                    new NamedCommand<object>( "Authenticate", DefaultAction.None )
                }
            };
            var interactionRequest = new InteractionRequest<WebAuthenticateInteraction>();

            interactionRequest.Requested += ( s, e ) =>
                interaction.DefaultCommand.Execute( new Mock<IWebAuthenticationResult>().Object );

            // act
            var result = await interactionRequest.RequestAsync( interaction );

            // assert
            Assert.NotNull( result );
            Assert.Equal( 2, interaction.Commands.Count );
            Assert.Equal( 0, interaction.DefaultCommandIndex );
            Assert.Equal( 1, interaction.CancelCommandIndex );
        }
    }
}
