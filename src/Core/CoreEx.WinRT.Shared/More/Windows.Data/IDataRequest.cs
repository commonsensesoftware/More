namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.ApplicationModel.DataTransfer;

    /// <summary>
    /// Defines the behavior of a data exchange request.
    /// </summary>
    [CLSCompliant( false )]
    [ContractClass( typeof( IDataRequestContract ) )]
    public interface IDataRequest
    {
        /// <summary>
        /// Gets a package that contains the content to share.
        /// </summary>
        /// <value>The <see cref="IDataPackage">data package</see> containing the content to share.</value>
        IDataPackage Data
        {
            get;
        }

        /// <summary>
        /// Gets the deadline for finishing a delayed rendering operation. If execution goes beyond that deadline, the results of delayed rendering are ignored.
        /// </summary>
        /// <value>The <see cref="DateTimeOffset">deadline</see> for the delayed rendering operation.</value>
        DateTimeOffset Deadline
        {
            get;
        }

        /// <summary>
        /// Cancels a sharing operation and supplies an error string.
        /// </summary>
        /// <param name="value">The text to display.</param>
        void FailWithDisplayText( string value );

        /// <summary>
        /// Supports asynchronous sharing operations by creating and returning a deferral object.
        /// </summary>
        /// <returns>An object that allows you to share or send content asynchronously. When the
        /// object is disposed, the deferral to the original operation ends.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method has side effects and should not be a property." )]
        IDisposable GetDeferral();
    }
}
