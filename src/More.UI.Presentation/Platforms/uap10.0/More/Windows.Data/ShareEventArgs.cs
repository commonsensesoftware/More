namespace More.Windows.Data
{
    using Data;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.ApplicationModel.DataTransfer.ShareTarget;
    using System;

    /// <summary>
    /// Represents the arguments for a share event.
    /// </summary>
    [CLSCompliant( false )]
    public partial class ShareEventArgs : EventArgs, IShareOperation
    {
        readonly ShareOperation adapted;
        readonly Lazy<IDataPackageView> dataPackageView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShareEventArgs"/> class.
        /// </summary>
        /// <param name="previousExecutionState">The <see cref="ApplicationExecutionState">execution state</see> of the application before sharing began.</param>
        /// <param name="shareOperation">The representation of the <see cref="ShareOperation">share operation</see>.</param>
        public ShareEventArgs( ApplicationExecutionState previousExecutionState, ShareOperation shareOperation )
        {
            Arg.NotNull( shareOperation, nameof( shareOperation ) );

            PreviousExecutionState = previousExecutionState;
            adapted = shareOperation;
            dataPackageView = new Lazy<IDataPackageView>( () => new DataPackageViewAdapter( adapted.Data ) );
        }

        /// <summary>
        /// Gets the execution state of the application before sharing began.
        /// </summary>
        /// <value>One of the <see cref="ApplicationExecutionState"/> values.</value>
        public ApplicationExecutionState PreviousExecutionState { get; private set; }

        /// <summary>
        /// Gets a <see cref="IDataPackageView">read-only data package</see> with the data that the user wants to share.
        /// </summary>
        /// <value>A <see cref="IDataPackageView"/> containing the data that the user wants to share.</value>
        public IDataPackageView Data => dataPackageView.Value;

        /// <summary>
        /// Gets the quick link identifier.
        /// </summary>
        /// <value>The ID of the quick link.</value>
        public string QuickLinkId => adapted.QuickLinkId;

        /// <summary>
        /// Removes the quick link from the list of quick links that are available to the user.
        /// </summary>
        public void RemoveThisQuickLink() => adapted.RemoveThisQuickLink();

        /// <summary>
        /// Specifies that the sharing operation is complete.
        /// </summary>
        public void ReportCompleted() => adapted.ReportCompleted();

        /// <summary>
        /// Specifies that the sharing operation is complete.
        /// </summary>
        /// <param name="quickLink">A <see cref="QuickLink">quick link</see> that the system saves as a shortcut for future sharing operations.</param>
        public void ReportCompleted( QuickLink quickLink ) => adapted.ReportCompleted( quickLink );

        /// <summary>
        /// Specifies that the application has acquired the content that the user wants to share.
        /// </summary>
        public void ReportDataRetrieved() => adapted.ReportDataRetrieved();

        /// <summary>
        /// Specifies that an error occurred during the sharing operation.
        /// </summary>
        /// <param name="value">Specifies the error message. The system displays this message to the user.</param>
        public void ReportError( string value ) => adapted.ReportError( value );

        /// <summary>
        /// Specifies that the application has started to acquire the content that the user wants to share.
        /// </summary>
        public void ReportStarted() => adapted.ReportStarted();

        /// <summary>
        /// Specifies that the application has requested that the system allow the sharing operation to run as a background task.
        /// </summary>
        public void ReportSubmittedBackgroundTask() => adapted.ReportSubmittedBackgroundTask();
#if WPA81
        void IShareOperation.DismissUI() { }
#else
        /// <summary>
        /// Closes the share pane.
        /// </summary>
        public void DismissUI() => adapted.DismissUI();
#endif
    }
}