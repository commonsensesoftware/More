namespace More.Windows.Input
{
    using IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="InteractionRequest{T}"/> class.
    /// </summary>
    public static class InteractionRequestExtensions
    {
        private static readonly IList<IFile> NoFiles = new IFile[0];

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain a single <see cref="INamedCommand">command</see>
        /// with the identifier and name "OK".</remarks>
        public static Task AlertAsync( this InteractionRequest<Interaction> interactionRequest, string title, string message ) => interactionRequest.Alert( title, message, null );

        /// <summary>
        /// Requests an alert be displayed to a user asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <param name="acknowledgeButtonText">The alert acknowledgement text. The default value is "OK".</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The requested <see cref="Interaction">interaction</see> will always contain a single <see cref="INamedCommand">command</see> with the identifier "OK".</remarks>
        public static Task AlertAsync( this InteractionRequest<Interaction> interactionRequest, string title, string message, string acknowledgeButtonText )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );

            title = title ?? string.Empty;
            message = message ?? string.Empty;

            if ( string.IsNullOrEmpty( acknowledgeButtonText ) )
                acknowledgeButtonText = SR.OK;

            var source = new TaskCompletionSource<object>();
            var interaction = new Interaction( title, message )
            {
                DefaultCommandIndex = 0,
                Commands =
                {
                    new NamedCommand<object>( "OK", acknowledgeButtonText, p => source.SetResult( null ) )
                }
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
        public static Task<bool> ConfirmAsync(
            this InteractionRequest<Interaction> interactionRequest,
            string prompt,
            string title,
            string acceptButtonText,
            string cancelButtonText )
        {
            Arg.NotNullOrEmpty( prompt, nameof( prompt ) );

            title = title ?? string.Empty;

            if ( string.IsNullOrEmpty( acceptButtonText ) )
                acceptButtonText = SR.OK;

            if ( string.IsNullOrEmpty( cancelButtonText ) )
                cancelButtonText = SR.Cancel;

            var source = new TaskCompletionSource<bool>();
            var interaction = new Interaction()
            {
                Title = title,
                Content = prompt,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "OK",  acceptButtonText, p => source.SetResult( true ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.SetResult( false ) )
                }
            };

            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests an open file interaction for a single file asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeFilter">A <see cref="IReadOnlyList{T}">read-only list</see> of filtered file types which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always have the title "Open File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        public static async Task<IFile> RequestSingleFileAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, IReadOnlyList<string> fileTypeFilter ) =>
            ( await interactionRequest.RequestAsync( null, null, null, fileTypeFilter, false ) ).FirstOrDefault();

        /// <summary>
        /// Requests an open file interaction for a single file asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeFilter">A <see cref="IReadOnlyList{T}">read-only list</see> of filtered file types which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        public static async Task<IFile> RequestSingleFileAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, string title, IReadOnlyList<string> fileTypeFilter ) =>
            ( await interactionRequest.RequestAsync( title, null, null, fileTypeFilter, false ) ).FirstOrDefault();

        /// <summary>
        /// Requests an open file interaction for multiple files asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeFilter">A <see cref="IReadOnlyList{T}">read-only list</see> of filtered file types which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always have the title "Open File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        public static Task<IList<IFile>> RequestMultipleFilesAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, IReadOnlyList<string> fileTypeFilter ) =>
            interactionRequest.RequestAsync( null, null, null, fileTypeFilter, true );

        /// <summary>
        /// Requests an open file interaction for multiple files asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeFilter">A <see cref="IReadOnlyList{T}">read-only list</see> of filtered file types which can be opened.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Open" and "Cancel", respectively.</remarks>
        public static Task<IList<IFile>> RequestMultipleFilesAsync( this InteractionRequest<OpenFileInteraction> interactionRequest, string title, IReadOnlyList<string> fileTypeFilter ) =>
            interactionRequest.RequestAsync( title, null, null, fileTypeFilter, true );

        /// <summary>
        /// Requests an open file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Open".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <param name="fileTypeFilter">A <see cref="IReadOnlyList{T}">read-only list</see> of filtered file types which can be opened.</param>
        /// <param name="multiselect">Indicates whether multiple files can be selected.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="IFile">files</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="OpenFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Open" and "Cancel".</remarks>
        public static Task<IList<IFile>> RequestAsync(
            this InteractionRequest<OpenFileInteraction> interactionRequest,
            string title,
            string acceptButtonText,
            string cancelButtonText,
            IReadOnlyList<string> fileTypeFilter,
            bool multiselect )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( fileTypeFilter, nameof( fileTypeFilter ) );

            if ( string.IsNullOrEmpty( title ) )
                title = SR.OpenFileTitle;

            if ( string.IsNullOrEmpty( acceptButtonText ) )
                acceptButtonText = SR.Open;

            if ( string.IsNullOrEmpty( cancelButtonText ) )
                cancelButtonText = SR.Cancel;

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
                    new NamedCommand<object>( "Open", acceptButtonText, p => source.SetResult( interaction.Files ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.SetResult( NoFiles ) )
                }
            };

            interaction.FileTypeFilter.AddRange( fileTypeFilter );
            interactionRequest.Request( interaction );
            return source.Task;
        }

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of <see cref="KeyValuePair{TKey, TValue}">key/value pairs</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always have the title "Save File" and contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Save" and "Cancel", respectively.</remarks>
        public static Task<IFile> RequestAsync( this InteractionRequest<SaveFileInteraction> interactionRequest, IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> fileTypeChoices ) =>
            interactionRequest.RequestAsync( null, null, null, fileTypeChoices );

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of <see cref="KeyValuePair{TKey, TValue}">key/value pairs</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Save" and "Cancel", respectively.</remarks>
        public static Task<IFile> RequestAsync( this InteractionRequest<SaveFileInteraction> interactionRequest, string title, IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> fileTypeChoices ) =>
            interactionRequest.RequestAsync( title, null, null, fileTypeChoices );

        /// <summary>
        /// Requests a save file interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <param name="acceptButtonText">The text for the accept button. The default value is "Save".</param>
        /// <param name="cancelButtonText">The text for the cancel button. The default value is "Cancel".</param>
        /// <param name="fileTypeChoices">The <see cref="IEnumerable{T}">sequence</see> of <see cref="KeyValuePair{TKey, TValue}">key/value pairs</see> representing the
        /// file type choices and their associated file extensions.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the saved <see cref="IFile">file</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SaveFileInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see> with the
        /// identifiers "Save" and "Cancel".</remarks>
        public static Task<IFile> RequestAsync(
            this InteractionRequest<SaveFileInteraction> interactionRequest,
            string title,
            string acceptButtonText,
            string cancelButtonText,
            IEnumerable<KeyValuePair<string, IReadOnlyList<string>>> fileTypeChoices )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( fileTypeChoices, nameof( fileTypeChoices ) );

            if ( string.IsNullOrEmpty( title ) )
                title = SR.SaveFileTitle;

            if ( string.IsNullOrEmpty( acceptButtonText ) )
                acceptButtonText = SR.Save;

            if ( string.IsNullOrEmpty( cancelButtonText ) )
                cancelButtonText = SR.Cancel;

            var source = new TaskCompletionSource<IFile>();
            SaveFileInteraction interaction = null;

            interaction = new SaveFileInteraction()
            {
                Title = title,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Save", acceptButtonText, p => source.SetResult( interaction.SavedFile ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.SetResult( null ) )
                }
            };

            var defaultFound = false;

            foreach ( var fileTypeChoice in fileTypeChoices )
            {
                var key = fileTypeChoice.Key;
                var value = fileTypeChoice.Value.ToArray();

                if ( !defaultFound && value.Length > 0 )
                {
                    var ext = value[0];

                    if ( !string.IsNullOrEmpty( ext ) )
                    {
                        // set the default file extension to the first entry with a non-null, non-empty value
                        interaction.DefaultFileExtension = ext;
                        defaultFound = true;
                    }
                }

                // copy to the interaction
                interaction.FileTypeChoices.Add( key, value );
            }

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
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest ) => interactionRequest.RequestAsync( null, null, null );

        /// <summary>
        /// Requests a select folder interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="title">The interaction title.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing the selected <see cref="IFolder">folder</see> or <c>null</c> if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectFolderInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, string title ) => interactionRequest.RequestAsync( title, null, null );

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
        public static Task<IFolder> RequestAsync( this InteractionRequest<SelectFolderInteraction> interactionRequest, string title, string acceptButtonText, string cancelButtonText )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Contract.Ensures( Contract.Result<Task<IFolder>>() != null );

            if ( string.IsNullOrEmpty( title ) )
                title = SR.SelectFolderTitle;

            if ( string.IsNullOrEmpty( acceptButtonText ) )
                acceptButtonText = SR.Select;

            if ( string.IsNullOrEmpty( cancelButtonText ) )
                cancelButtonText = SR.Cancel;

            var source = new TaskCompletionSource<IFolder>();
            SelectFolderInteraction interaction = null;

            interaction = new SelectFolderInteraction()
            {
                Title = title,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Select", acceptButtonText, p => source.SetResult( interaction.Folder ) ),
                    new NamedCommand<object>( "Cancel", cancelButtonText, p => source.SetResult( null ) )
                }
            };

            interactionRequest.Request( interaction );
            return source.Task;
        }
    }
}
