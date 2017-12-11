namespace More.Windows.Input
{
    using IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="InteractionRequest{T}"/> class.
    /// </summary>
    public static class InteractionRequestExtensions
    {
        static readonly IList<IFile> NoFiles = new IFile[0];

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain a single <see cref="INamedCommand">command</see>
        /// with the identifier and name "OK".</remarks>
        public static Task AlertAsync( this InteractionRequest<Interaction> interactionRequest, string title, string message ) => interactionRequest.AlertAsync( title, message, null );

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <param name="acknowledgeButtonText">The alert acknowledgement text. The default value is "OK".</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain a single <see cref="INamedCommand">command</see> with the identifier "OK".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task AlertAsync( this InteractionRequest<Interaction> interactionRequest, string title, string message, string acknowledgeButtonText )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );

            if ( string.IsNullOrEmpty( acknowledgeButtonText ) )
            {
                acknowledgeButtonText = SR.OK;
            }

            var source = new TaskCompletionSource<object>();
            var interaction = new Interaction( title ?? string.Empty, message ?? string.Empty )
            {
                DefaultCommandIndex = 0,
                Commands =
                {
                    new NamedCommand<object>( "OK", acknowledgeButtonText, p => source.TrySetResult( null ) ),
                },
            };
            interactionRequest.Request( interaction );

            return source.Task;
        }

        /// <summary>
        /// Requests a user confirmation asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "OK" and "Cancel", respectively.</remarks>
        public static Task<bool> ConfirmAsync( this InteractionRequest<Interaction> interactionRequest, string prompt ) =>
            interactionRequest.ConfirmAsync( prompt, null, null, null );

        /// <summary>
        /// Requests a user confirmation asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <param name="title">The prompt title.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "OK" and "Cancel", respectively.</remarks>
        public static Task<bool> ConfirmAsync( this InteractionRequest<Interaction> interactionRequest, string prompt, string title ) =>
            interactionRequest.ConfirmAsync( prompt, title, null, null );

        /// <summary>
        /// Requests a user confirmation asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="prompt">The user confirmation prompt.</param>
        /// <param name="title">The prompt title.</param>
        /// <param name="acceptButtonText">The confirmation acceptance text. The default value is "OK".</param>
        /// <param name="cancelButtonText">The confirmation cancellation text. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a value indicating whether the user accepted or canceled the prompt.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "OK" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<bool> ConfirmAsync(
            this InteractionRequest<Interaction> interactionRequest,
            string prompt,
            string title,
            string acceptButtonText,
            string cancelButtonText )
        {
            Arg.NotNullOrEmpty( prompt, nameof( prompt ) );

            if ( string.IsNullOrEmpty( acceptButtonText ) )
            {
                acceptButtonText = SR.OK;
            }

            if ( string.IsNullOrEmpty( cancelButtonText ) )
            {
                cancelButtonText = SR.Cancel;
            }

            var source = new TaskCompletionSource<bool>();
            var interaction = new Interaction()
            {
                Title = title ?? string.Empty,
                Content = prompt,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "OK",  acceptButtonText, p => source.TrySetResult( true ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.TrySetResult( false ) ),
                },
            };

            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests an open file interaction for a single file asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always have the title "Open File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for IEnumerable<T> with a generic argument." )]
        public static async Task<IFile> RequestSingleFileAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, IEnumerable<FileType> fileTypeFilter ) =>
            ( await interactionRequest.RequestAsync( null, null, null, fileTypeFilter, false ).ConfigureAwait( true ) ).FirstOrDefault();

        /// <summary>
        /// Requests an open file interaction for a single file asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for IEnumerable<T> with a generic argument." )]
        public static async Task<IFile> RequestSingleFileAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, string title, IEnumerable<FileType> fileTypeFilter ) =>
            ( await interactionRequest.RequestAsync( title, null, null, fileTypeFilter, false ).ConfigureAwait( true ) ).FirstOrDefault();

        /// <summary>
        /// Requests an open file interaction for multiple files asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always have the title "Open File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for IEnumerable<T> with a generic argument." )]
        public static Task<IList<IFile>> RequestMultipleFilesAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, IEnumerable<FileType> fileTypeFilter ) =>
            interactionRequest.RequestAsync( null, null, null, fileTypeFilter, true );

        /// <summary>
        /// Requests an open file interaction for multiple files asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic argument." )]
        public static Task<IList<IFile>> RequestMultipleFilesAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, string title, IEnumerable<FileType> fileTypeFilter ) =>
            interactionRequest.RequestAsync( title, null, null, fileTypeFilter, true );

        /// <summary>
        /// Requests an open file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <param name="multiselect">Indicates whether multiple files can be selected.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Open" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic argument." )]
        public static Task<IList<IFile>> RequestAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, IEnumerable<FileType> fileTypeFilter, bool multiselect ) =>
            interactionRequest.RequestAsync( null, null, null, fileTypeFilter, multiselect );

        /// <summary>
        /// Requests an open file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Open".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <param name="fileTypeFilter">A <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> which can be opened.</param>
        /// <param name="multiselect">Indicates whether multiple files can be selected.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Open" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic argument." )]
        public static Task<IList<IFile>> RequestAsync(
            this InteractionRequest<OpenFileInteraction> interactionRequest,
            string title,
            string acceptButtonText,
            string cancelButtonText,
            IEnumerable<FileType> fileTypeFilter,
            bool multiselect )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( fileTypeFilter, nameof( fileTypeFilter ) );

            if ( string.IsNullOrEmpty( title ) )
            {
                title = SR.OpenFileTitle;
            }

            if ( string.IsNullOrEmpty( acceptButtonText ) )
            {
                acceptButtonText = SR.Open;
            }

            if ( string.IsNullOrEmpty( cancelButtonText ) )
            {
                cancelButtonText = SR.Cancel;
            }

            var source = new TaskCompletionSource<IList<IFile>>();
            OpenFileInteraction interaction = null;

            interaction = new OpenFileInteraction()
            {
                Title = title,
                Multiselect = multiselect,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Open", acceptButtonText, p => source.TrySetResult( interaction.Files ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.TrySetResult( NoFiles ) ),
                },
            };

            interaction.FileTypeFilter.AddRange( fileTypeFilter );
            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always have the title "Save File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Save" and "Cancel", respectively.</remarks>
        public static Task<IFile> RequestAsync( this InteractionRequest<SaveFileInteraction> interactionRequest, IEnumerable<FileType> fileTypeChoices ) =>
            interactionRequest.RequestAsync( null, null, null, fileTypeChoices );

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Save" and "Cancel", respectively.</remarks>
        public static Task<IFile> RequestAsync( this InteractionRequest<SaveFileInteraction> interactionRequest, string title, IEnumerable<FileType> fileTypeChoices ) =>
            interactionRequest.RequestAsync( title, null, null, fileTypeChoices );

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Save".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of filtered <see cref="FileType">file types</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Save" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<IFile> RequestAsync(
            this InteractionRequest<SaveFileInteraction> interactionRequest,
            string title,
            string acceptButtonText,
            string cancelButtonText,
            IEnumerable<FileType> fileTypeChoices )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( fileTypeChoices, nameof( fileTypeChoices ) );

            if ( string.IsNullOrEmpty( title ) )
            {
                title = SR.SaveFileTitle;
            }

            if ( string.IsNullOrEmpty( acceptButtonText ) )
            {
                acceptButtonText = SR.Save;
            }

            if ( string.IsNullOrEmpty( cancelButtonText ) )
            {
                cancelButtonText = SR.Cancel;
            }

            var source = new TaskCompletionSource<IFile>();
            SaveFileInteraction interaction = null;

            interaction = new SaveFileInteraction()
            {
                Title = title,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Save", acceptButtonText, p => source.TrySetResult( interaction.SavedFile ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.TrySetResult( null ) ),
                },
            };

            var defaultExt = ( from choice in fileTypeChoices
                               from ext in choice.Extensions
                               where !string.IsNullOrEmpty( ext )
                               select ext ).FirstOrDefault();

            // set the default file extension to the first entry with a non-null, non-empty value
            if ( !string.IsNullOrEmpty( defaultExt ) )
            {
                interaction.DefaultFileExtension = defaultExt;
            }

            interaction.FileTypeChoices.AddRange( fileTypeChoices );
            interactionRequest.Request( interaction );

            return source.Task;
        }

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always have the title "Select Folder" and will contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest ) => interactionRequest.RequestAsync( null, null, null, null );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="initialFolder">The initial <see cref="IFolder">folder</see> to select.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always have the title "Select Folder" and will contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, IFolder initialFolder ) =>
            interactionRequest.RequestAsync( initialFolder, null, null, null );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, string title ) => interactionRequest.RequestAsync( null, title, null, null );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="initialFolder">The initial <see cref="IFolder">folder</see> to select.</param>
        /// <param name="title">The interaction title.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, IFolder initialFolder, string title ) =>
            interactionRequest.RequestAsync( initialFolder, title, null, null );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Select".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Select" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, string title, string acceptButtonText, string cancelButtonText ) =>
            interactionRequest.RequestAsync( null, title, acceptButtonText, cancelButtonText );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="initialFolder">The initial <see cref="IFolder">folder</see> to select.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Select".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Select" and "Cancel".</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, IFolder initialFolder, string title, string acceptButtonText, string cancelButtonText )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );

            if ( string.IsNullOrEmpty( title ) )
            {
                title = SR.SelectFolderTitle;
            }

            if ( string.IsNullOrEmpty( acceptButtonText ) )
            {
                acceptButtonText = SR.Select;
            }

            if ( string.IsNullOrEmpty( cancelButtonText ) )
            {
                cancelButtonText = SR.Cancel;
            }

            var source = new TaskCompletionSource<IFolder>();
            SelectFolderInteraction interaction = null;

            interaction = new SelectFolderInteraction()
            {
                Title = title,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Folder = initialFolder,
                Commands =
                {
                    new NamedCommand<object>( "Select", acceptButtonText, p => source.TrySetResult( interaction.Folder ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.TrySetResult( null ) ),
                },
            };

            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests a text input interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="prompt">The interaction prompt.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the text input provided by the user. If the
        /// user canceled the operation, the text will be <c>null</c>.</returns>
        /// <remarks>The requested <see cref="TextInputInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "OK" and "Cancel", respectively.</remarks>
        public static Task<string> RequestAsync( this InteractionRequest<TextInputInteraction> interactionRequest, string prompt ) =>
            interactionRequest.RequestAsync( null, prompt, null, null, null );

        /// <summary>
        /// Requests a text input interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="prompt">The interaction prompt.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the text input provided by the user. If the
        /// user canceled the operation, the text will be <c>null</c>.</returns>
        /// <remarks>The requested <see cref="TextInputInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "OK" and "Cancel", respectively.</remarks>
        public static Task<string> RequestAsync( this InteractionRequest<TextInputInteraction> interactionRequest, string title, string prompt ) =>
            interactionRequest.RequestAsync( title, prompt, null, null, null );

        /// <summary>
        /// Requests a text input interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="prompt">The interaction prompt.</param>
        /// <param name="defaultResponse">The default response. The default value is an empty string.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the text input provided by the user. If the
        /// user canceled the operation, the text will be <c>null</c>.</returns>
        /// <remarks>The requested <see cref="TextInputInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "OK" and "Cancel", respectively.</remarks>
        public static Task<string> RequestAsync( this InteractionRequest<TextInputInteraction> interactionRequest, string title, string prompt, string defaultResponse ) =>
            interactionRequest.RequestAsync( title, prompt, defaultResponse, null, null );

        /// <summary>
        /// Requests a text input interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="prompt">The interaction prompt.</param>
        /// <param name="defaultResponse">The default response. The default value is an empty string.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "OK".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the text input provided by the user. If the
        /// user canceled the operation, the text will be <c>null</c>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<string> RequestAsync( this InteractionRequest<TextInputInteraction> interactionRequest, string title, string prompt, string defaultResponse, string acceptButtonText, string cancelButtonText )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNullOrEmpty( prompt, nameof( prompt ) );
            Contract.Ensures( Contract.Result<Task<string>>() != null );

            if ( string.IsNullOrEmpty( acceptButtonText ) )
            {
                acceptButtonText = SR.OK;
            }

            if ( string.IsNullOrEmpty( cancelButtonText ) )
            {
                cancelButtonText = SR.Cancel;
            }

            var source = new TaskCompletionSource<string>();
            TextInputInteraction interaction = null;

            interaction = new TextInputInteraction( title ?? string.Empty, prompt, defaultResponse ?? string.Empty )
            {
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "OK", acceptButtonText, p => source.TrySetResult( interaction.Response ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.TrySetResult( null ) ),
                },
            };

            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests a web authentication interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="requestUri">The starting Uniform Resource Identifier (URI) of the web service.</param>
        /// <param name="callbackUri">The callback <see cref="Uri">URI</see> that indicates the completion of the web authentication.
        /// The broker matches this <see cref="Uri">URI</see> against every <see cref="Uri">URI</see> that it is about to navigate to.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IWebAuthenticationResult">authentication result</see>.</returns>
        /// <remarks>The requested <see cref="WebAuthenticateInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "OK" and "Cancel".</remarks>
        public static Task<IWebAuthenticationResult> RequestAsync( this InteractionRequest<WebAuthenticateInteraction> interactionRequest, Uri requestUri, Uri callbackUri )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( requestUri, nameof( requestUri ) );
            return interactionRequest.RequestAsync( new WebAuthenticateInteraction( requestUri ) { CallbackUri = callbackUri } );
        }

        /// <summary>
        /// Requests a web authentication interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="interaction">The requested <see cref="WebAuthenticateInteraction">interaction</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IWebAuthenticationResult">authentication result</see>.</returns>
        /// <remarks>The requested <see cref="WebAuthenticateInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "OK" and "Cancel". If the specified <paramref name="interaction"/> has any commands predefined, they will be cleared.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated a code contract." )]
        public static Task<IWebAuthenticationResult> RequestAsync( this InteractionRequest<WebAuthenticateInteraction> interactionRequest, WebAuthenticateInteraction interaction )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( interaction, nameof( interaction ) );

            var source = new TaskCompletionSource<IWebAuthenticationResult>();

            interaction.Commands.Clear();
            interaction.Commands.Add( new NamedCommand<IWebAuthenticationResult>( "OK", p => source.TrySetResult( p ) ) );
            interaction.Commands.Add( new NamedCommand<IWebAuthenticationResult>( "Cancel", p => source.TrySetResult( p ) ) );

            if ( interaction.DefaultCommandIndex < 0 || interaction.DefaultCommandIndex > 1 )
            {
                interaction.DefaultCommandIndex = 0;
            }

            if ( interaction.CancelCommandIndex < 0 || interaction.CancelCommandIndex > 1 )
            {
                interaction.CancelCommandIndex = 1;
            }

            interactionRequest.Request( interaction );

            return source.Task;
        }
    }
}