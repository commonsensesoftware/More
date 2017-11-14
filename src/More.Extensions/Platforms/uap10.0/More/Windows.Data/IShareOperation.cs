namespace More.Windows.Data
{
    using System;
    using global::Windows.ApplicationModel.DataTransfer.ShareTarget;

    /// <summary>
    /// Defines the behavior of a share operation.
    /// </summary>
    /// <remarks>This includes the data that the user wants to share, setting or removing quick links,
    /// and informing the system about the status of the operation.</remarks>
    [CLSCompliant( false )]
    public partial interface IShareOperation
    {
        /// <summary>
        /// Gets a <see cref="IDataPackageView">read-only data package</see> with the data that the user wants to share.
        /// </summary>
        /// <value>A <see cref="IDataPackageView"/> containing the data that the user wants to share.</value>
        IDataPackageView Data { get; }

        /// <summary>
        /// Gets the quick link identifier.
        /// </summary>
        /// <value>The ID of the quick link.</value>
        string QuickLinkId { get; }

        /// <summary>
        /// Removes the quick link from the list of quick links that are available to the user.
        /// </summary>
        void RemoveThisQuickLink();

        /// <summary>
        /// Specifies that the sharing operation is complete.
        /// </summary>
        void ReportCompleted();

        /// <summary>
        /// Specifies that the sharing operation is complete.
        /// </summary>
        /// <param name="quickLink">A <see cref="QuickLink">quick link</see> that the system saves as a shortcut for future sharing operations.</param>
        void ReportCompleted( QuickLink quickLink );

        /// <summary>
        /// Specifies that the application has acquired the content that the user wants to share.
        /// </summary>
        void ReportDataRetrieved();

        /// <summary>
        /// Specifies that an error occurred during the sharing operation.
        /// </summary>
        /// <param name="value">Specifies the error message. The system displays this message to the user.</param>
        void ReportError( string value );

        /// <summary>
        /// Specifies that the application has started to acquire the content that the user wants to share.
        /// </summary>
        void ReportStarted();

        /// <summary>
        /// Specifies that the application has requested that the system allow the sharing operation to run as a background task.
        /// </summary>
        void ReportSubmittedBackgroundTask();

        /// <summary>
        /// Closes the share pane.
        /// </summary>
        void DismissUI();
    }
}