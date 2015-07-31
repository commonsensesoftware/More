namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    public partial class SelectFolderAction : System.Windows.Interactivity.TriggerAction
    {
        private void SelectFolder( SelectFolderInteraction selectFolder )
        {
            Contract.Requires( selectFolder != null );

            var commitButton = selectFolder.DefaultCommand;
            var dialog = new FolderPicker();

            dialog.ContinuationData.AddRange( selectFolder.ContinuationData );
            dialog.FileTypeFilter.AddRange( selectFolder.FileTypeFilter );
            dialog.SuggestedStartLocation = SuggestedStartLocation;
            dialog.ViewMode = ViewMode;

            if ( dialog.FileTypeFilter.Count == 0 )
                dialog.FileTypeFilter.Add( "*" );

            if ( !string.IsNullOrEmpty( SettingsIdentifier ) )
                dialog.SettingsIdentifier = SettingsIdentifier;

            if ( commitButton != null )
                dialog.CommitButtonText = commitButton.Name;

            dialog.PickFolderAndContinue();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns null.</returns>
        public override object Execute( object sender, object parameter )
        {
            var selectFolder = GetRequestedInteraction<SelectFolderInteraction>( parameter );

            if ( selectFolder != null )
                SelectFolder( selectFolder );

            return null;
        }
    }
}
