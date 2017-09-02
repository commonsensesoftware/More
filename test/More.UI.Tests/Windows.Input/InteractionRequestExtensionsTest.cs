namespace More.Windows.Input
{
    using FluentAssertions;
    using IO;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class InteractionRequestExtensionsTest
    {
        [Fact]
        public async Task alert_async_should_request_title_and_message()
        {
            // arrange
            var interactionRequest = new InteractionRequest<Interaction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.ExecuteDefaultCommand();

            // act
            await interactionRequest.AlertAsync( "Test", "This is a test." );

            // assert
            args.Interaction.Should().ShouldBeEquivalentTo(
                new
                {
                    Title = "Test",
                    Content = "This is a test.",
                    DefaultCommand = new { Name = "OK" }
                },
                options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task alert_async_should_request_titleX2C_messageX2C_and_button()
        {
            // arrange
            var interactionRequest = new InteractionRequest<Interaction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.ExecuteDefaultCommand();

            // act
            await interactionRequest.AlertAsync( "Test", "This is a test.", "Accept" );

            // assert
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Test",
                  Content = "This is a test.",
                  DefaultCommand = new { Name = "Accept" }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task confirm_async_should_request_prompt()
        {
            // arrange
            var interactionRequest = new InteractionRequest<Interaction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.ExecuteDefaultCommand();

            // act
            var response = await interactionRequest.ConfirmAsync( "Continue?" );

            // assert
            response.Should().BeTrue();
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "",
                  Content = "Continue?",
                  DefaultCommand = new { Name = "OK" },
                  CancelCommand = new { Name = "Cancel" },
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task confirm_async_should_request_prompt_and_title()
        {
            // arrange
            var interactionRequest = new InteractionRequest<Interaction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.ExecuteCancelCommand();

            // act
            var response = await interactionRequest.ConfirmAsync( "Continue?", "Question" );

            // assert
            response.Should().BeFalse();
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Question",
                  Content = "Continue?",
                  DefaultCommand = new { Name = "OK" },
                  CancelCommand = new { Name = "Cancel" },
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task confirm_async_should_request_titleX2C_promptX2C_and_buttons()
        {
            // arrange
            var interactionRequest = new InteractionRequest<Interaction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.ExecuteDefaultCommand();

            // act
            var response = await interactionRequest.ConfirmAsync( "Are you sure?", "Question", "Yes", "No" );

            // assert
            response.Should().BeTrue();
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Question",
                  Content = "Are you sure?",
                  DefaultCommand = new { Name = "Yes" },
                  CancelCommand = new { Name = "No" },
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_single_file_async_should_request_open_files_with_file_filter()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) ( args = e ).Interaction;
                interaction.Files.Add( file );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var openedFile = await interactionRequest.RequestSingleFileAsync( new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            openedFile.Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Open File",
                  DefaultCommand = new { Name = "Open" },
                  CancelCommand = new { Name = "Cancel" },
                  Multiselect = false,
                  Files = new[] { file },
                  FileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_single_file_async_should_request_open_files_with_title_and_file_filter()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) ( args = e ).Interaction;
                interaction.Files.Add( file );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var openedFile = await interactionRequest.RequestSingleFileAsync( "Open", new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            openedFile.Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Open",
                  DefaultCommand = new { Name = "Open" },
                  CancelCommand = new { Name = "Cancel" },
                  Multiselect = false,
                  Files = new[] { file },
                  FileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_multiple_files_async_should_request_open_files_with_file_filter()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) ( args = e ).Interaction;
                interaction.Files.Add( file );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var openedFiles = await interactionRequest.RequestMultipleFilesAsync( new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            openedFiles.Single().Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Open File",
                  DefaultCommand = new { Name = "Open" },
                  CancelCommand = new { Name = "Cancel" },
                  Multiselect = true,
                  Files = new[] { file },
                  FileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_multiple_files_async_should_request_open_files_with_title_and_file_filter()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) ( args = e ).Interaction;
                interaction.Files.Add( file );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var openedFiles = await interactionRequest.RequestMultipleFilesAsync( "Open", new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            openedFiles.Single().Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Open",
                  DefaultCommand = new { Name = "Open" },
                  CancelCommand = new { Name = "Cancel" },
                  Multiselect = true,
                  Files = new[] { file },
                  FileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_open_files_with_titleX2C_buttonsX2C_and_file_filter()
        {
            // arrange
            var multiselect = false;
            var file = new Mock<IFile>().Object;
            var interactionRequest = new OpenFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (OpenFileInteraction) ( args = e ).Interaction;
                interaction.Files.Add( file );
                interaction.ExecuteDefaultCommand();
            };

            // act
            var openedFiles = await interactionRequest.RequestAsync( "Open", "Select", "Close", new[] { new FileType( "Text Files", ".txt" ) }, multiselect );

            // assert
            openedFiles.Single().Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Open",
                  DefaultCommand = new { Name = "Select" },
                  CancelCommand = new { Name = "Close" },
                  Multiselect = false,
                  Files = new[] { file },
                  FileTypeFilter = new[] { new FileType( "Text Files", ".txt" ) }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_save_files_with_file_choices()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) ( args = e ).Interaction;
                interaction.SavedFile = file;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var savedFile = await interactionRequest.RequestAsync( new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            savedFile.Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Save File",
                  DefaultCommand = new { Name = "Save" },
                  CancelCommand = new { Name = "Cancel" },
                  FileTypeChoices = new[] { new FileType( "Text Files", ".txt" ) },
                  SavedFile = file
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_save_file_with_title_and_file_choices()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) ( args = e ).Interaction;
                interaction.SavedFile = file;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var savedFile = await interactionRequest.RequestAsync( "Save", new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            savedFile.Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Save",
                  DefaultCommand = new { Name = "Save" },
                  CancelCommand = new { Name = "Cancel" },
                  FileTypeChoices = new[] { new FileType( "Text Files", ".txt" ) },
                  SavedFile = file
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_save_file_with_titleX2C_buttonsX2C_and_file_choices()
        {
            // arrange
            var file = new Mock<IFile>().Object;
            var interactionRequest = new SaveFileInteractionRequest( DefaultAction.None );
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SaveFileInteraction) ( args = e ).Interaction;
                interaction.SavedFile = file;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var savedFile = await interactionRequest.RequestAsync( "Save", "Select", "Close", new[] { new FileType( "Text Files", ".txt" ) } );

            // assert
            savedFile.Should().Be( file );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Save",
                  DefaultCommand = new { Name = "Select" },
                  CancelCommand = new { Name = "Close" },
                  FileTypeChoices = new[] { new FileType( "Text Files", ".txt" ) },
                  SavedFile = file
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_select_folder()
        {
            // arrange
            var folder = new Mock<IFolder>().Object;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SelectFolderInteraction) ( args = e ).Interaction;
                interaction.Folder = folder;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var selectedFolder = await interactionRequest.RequestAsync();

            // assert
            selectedFolder.Should().Be( folder );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Select Folder",
                  DefaultCommand = new { Name = "Select" },
                  CancelCommand = new { Name = "Cancel" },
                  Folder = folder
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_select_folder_with_title()
        {
            // arrange
            var folder = new Mock<IFolder>().Object;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SelectFolderInteraction) ( args = e ).Interaction;
                interaction.Folder = folder;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var selectedFolder = await interactionRequest.RequestAsync( "Pick Folder" );

            // assert
            selectedFolder.Should().Be( folder );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Pick Folder",
                  DefaultCommand = new { Name = "Select" },
                  CancelCommand = new { Name = "Cancel" },
                  Folder = folder
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_select_folder_with_title_and_buttons()
        {
            // arrange
            var folder = new Mock<IFolder>().Object;
            var interactionRequest = new InteractionRequest<SelectFolderInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (SelectFolderInteraction) ( args = e ).Interaction;
                interaction.Folder = folder;
                e.Interaction.ExecuteDefaultCommand();
            };

            // act
            var selectedFolder = await interactionRequest.RequestAsync( "Select", "Pick", "Close" );

            // assert
            selectedFolder.Should().Be( folder );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Select",
                  DefaultCommand = new { Name = "Pick" },
                  CancelCommand = new { Name = "Close" },
                  Folder = folder
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_text_input_with_prompt()
        {
            // arrange
            var interactionRequest = new InteractionRequest<TextInputInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) ( args = e ).Interaction;
                interaction.Response = "Test";
                interaction.ExecuteDefaultCommand();
            };

            // act
            var response = await interactionRequest.RequestAsync( "Enter someting:" );

            // assert
            response.Should().Be( "Test" );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "",
                  Content = "Enter someting:",
                  Response = "Test",
                  DefaultResponse = "",
                  DefaultCommand = new { Name = "OK" },
                  CancelCommand = new { Name = "Cancel" }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_text_input_with_prompt_and_title()
        {
            // arrange
            var interactionRequest = new InteractionRequest<TextInputInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) ( args = e ).Interaction;
                interaction.Response = "Test";
                interaction.ExecuteDefaultCommand();
            };

            // act
            var response = await interactionRequest.RequestAsync( "Input Box", "Enter someting:" );

            // assert
            response.Should().Be( "Test" );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Input Box",
                  Content = "Enter something:",
                  Response = "Test",
                  DefaultResponse = "",
                  DefaultCommand = new { Name = "OK" },
                  CancelCommand = new { Name = "Cancel" }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_text_input_with_promptX2C_titleX2C_and_default_response()
        {
            // arrange
            var interactionRequest = new InteractionRequest<TextInputInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) ( args = e ).Interaction;
                interaction.Response = interaction.DefaultResponse;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var response = await interactionRequest.RequestAsync( "Input Box", "Enter something:", "Test" );

            // assert
            response.Should().Be( "Test" );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Input Box",
                  Content = "Enter something:",
                  Response = "Test",
                  DefaultResponse = "Test",
                  DefaultCommand = new { Name = "OK" },
                  CancelCommand = new { Name = "Cancel" }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_text_input_with_promptX2C_titleX2C_default_responseX2C_and_buttons()
        {
            // arrange
            var interactionRequest = new InteractionRequest<TextInputInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) =>
            {
                var interaction = (TextInputInteraction) ( args = e ).Interaction;
                interaction.Response = interaction.DefaultResponse;
                interaction.ExecuteDefaultCommand();
            };

            // act
            var response = await interactionRequest.RequestAsync(
                                            "Input Box",
                                            "Enter something:",
                                            "Test",
                                            "Accept",
                                            "Close" );

            // assert
            response.Should().Be( "Test" );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  Title = "Input Box",
                  Content = "Enter something:",
                  Response = "Test",
                  DefaultResponse = "Test",
                  DefaultCommand = new { Name = "Accept" },
                  CancelCommand = new { Name = "Close" }
              },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_request_authentication_with_request_uri_and_callback_uri()
        {
            // arrange
            var requestUrl = new Uri( "http://tempuri.org" );
            var callbackUrl = new Uri( "http://tempuri.org/callback" );
            var webAuthenticationResult = new Mock<IWebAuthenticationResult>().Object;
            var interactionRequest = new InteractionRequest<WebAuthenticateInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.DefaultCommand.Execute( webAuthenticationResult );

            // act
            var result = await interactionRequest.RequestAsync( requestUrl, callbackUrl );

            // assert
            result.Should().Be( webAuthenticationResult );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new { RequestUri = requestUrl, CallbackUri = callbackUrl },
              options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public async Task request_async_should_replace_userX2Ddefined_commands()
        {
            // arrange
            var interaction = new WebAuthenticateInteraction( new Uri( "http://tempuri.org" ) )
            {
                DefaultCommandIndex = 0,
                Commands = { new NamedCommand<object>( "Authenticate", DefaultAction.None ) }
            };
            var webAuthenticationResult = new Mock<IWebAuthenticationResult>().Object;
            var interactionRequest = new InteractionRequest<WebAuthenticateInteraction>();
            var args = default( InteractionRequestedEventArgs );

            interactionRequest.Requested += ( s, e ) => ( args = e ).Interaction.DefaultCommand.Execute( webAuthenticationResult );

            // act
            var result = await interactionRequest.RequestAsync( interaction );

            // assert
            result.Should().Be( webAuthenticationResult );
            args.Interaction.Should().ShouldBeEquivalentTo(
              new
              {
                  DefaultCommandIndex = 0,
                  CancelCommandIndex = 1
              },
              options => options.ExcludingMissingMembers() );
            args.Interaction.Commands.Should().HaveCount( 2 );
        }
    }
}