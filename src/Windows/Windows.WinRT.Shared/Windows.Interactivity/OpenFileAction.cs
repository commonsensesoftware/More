namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using global::Windows.Storage.Pickers;

    /// <summary>
    /// Represents an <see cref="T:Interactivity.TriggerAction">interactivity action</see> that can be used to open a file for the
    /// <see cref="OpenFileInteraction">interaction</see> received from an <see cref="E:IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class OpenFileAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Gets or sets the settings identifier associated with the file open picker instance.
        /// </summary>
        /// <value>The settings identifier.</value>
        public string SettingsIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial location where the open file picker looks for folders to present to the user.
        /// </summary>
        /// <value>The identifier of the starting location.</value>
        public PickerLocationId SuggestedStartLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the view mode that the file open picker uses to display items.
        /// </summary>
        /// <value>One of the <see cref="ViewMode"/> values. The default value is <see cref="F:PickerViewMode.List"/>.</value>
        public PickerViewMode ViewMode
        {
            get;
            set;
        }
    }
}
