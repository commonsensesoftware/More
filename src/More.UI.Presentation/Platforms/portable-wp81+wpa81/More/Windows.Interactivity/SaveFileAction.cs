namespace More.Windows.Interactivity
{
    using Microsoft.Xaml.Interactivity;
    using More.Windows.Input;
    using System;
    using global::Windows.Storage.Pickers;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents an <see cref="IAction">interactivity action</see> that can be used to save a file for the
    /// <see cref="SaveFileInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class SaveFileAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets the settings identifier associated with the file save picker instance.
        /// </summary>
        /// <value>The settings identifier.</value>
        public string SettingsIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the initial location where the save file picker looks for folders to present to the user.
        /// </summary>
        /// <value>The identifier of the starting location.</value>
        public PickerLocationId SuggestedStartLocation { get; set; }
    }
}