namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.Foundation;
    using global::Windows.Storage;
    using global::Windows.Storage.Streams;

    /// <summary>
    /// Defines the behavior of a read-only version of a <see cref="IDataPackage"/>. 
    /// </summary>
    /// <remarks>Applications that receive shared content get this object when acquiring content.</remarks>
    [CLSCompliant( false )]
    public interface IDataPackageView
    {
        /// <summary>
        /// Returns the formats the DataPackageView contains.
        /// </summary>
        /// <value>The formats the <see cref="IDataPackageView"/> contains.</value>
        IReadOnlyList<string> AvailableFormats
        {
            get;
        }

        /// <summary>
        /// Gets a DataPackagePropertySetView object, which contains a read-only set
        /// of properties for the data in the DataPackageView object.
        /// </summary>
        /// <value>A <see cref="IDataPackagePropertySetView">read-only set of properties</see> for the data.</value>
        IDataPackagePropertySetView Properties
        {
            get;
        }

        /// <summary>
        /// Gets the requested operation (such as copy or move).
        /// </summary>
        /// <value>An enumeration that states what operation (such as copy or move) was completed.</value>
        /// <remarks>Primarily used for Clipboard actions.</remarks>
        DataPackageOperation RequestedOperation
        {
            get;
        }

        /// <summary>
        /// Checks to see if the DataPackageView contains a specific data format.
        /// </summary>
        /// <param name="formatId">The name of the format.</param>
        /// <returns> True if the <see cref="IDataPackageView"/> contains the format; false otherwise.</returns>
        bool Contains( string formatId );

        /// <summary>
        /// Gets the application link in the <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the application <see cref="T:Uri">link</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<Uri> GetApplicationLinkAsync();

        /// <summary>
        /// Gets the bitmap image contained in the <see cref="IDataPackageView"/>.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the
        /// <see cref="IRandomAccessStreamReference">stream</see> with the bitmap image.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<IRandomAccessStreamReference> GetBitmapAsync();

        /// <summary>
        /// Gets the data contained in the <see cref="IDataPackageView"/>.
        /// </summary>
        /// <param name="formatId">Specifies the format of the data.</param>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the data.</returns>
        IAsyncOperation<object> GetDataAsync( string formatId );

        /// <summary>
        /// Gets the HTML stored in the <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the HTML.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<string> GetHtmlFormatAsync();

        /// <summary>
        /// Gets the data (such as an image) referenced in HTML content.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the
        /// <see cref="IReadOnlyDictionary{TKey,TValue}">data</see> referenced in the HTML content.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<IReadOnlyDictionary<string, IRandomAccessStreamReference>> GetResourceMapAsync();

        /// <summary>
        /// Gets the rich text formatted (RTF) content contained in a <see cref="IDataPackageView"/>.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the rich text formatted content.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<string> GetRtfAsync();

        /// <summary>
        /// Gets the files and folders stored in a <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing a
        /// <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IStorageItem">files and folders</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics support." )]
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<IReadOnlyList<IStorageItem>> GetStorageItemsAsync();

        /// <summary>
        /// Gets the text in the <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the text.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<string> GetTextAsync();

        /// <summary>
        /// Gets the text in the <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <param name="formatId">A string that represents the data format.</param>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the text.</returns>
        IAsyncOperation<string> GetTextAsync( string formatId );

        /// <summary>
        /// Gets the web link in the <see cref="IDataPackageView"/> object.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the web <see cref="T:Uri">link</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is asynchronous method and not a property." )]
        IAsyncOperation<Uri> GetWebLinkAsync();

        /// <summary>
        /// Informs the system that your app is finished using the DataPackageView object.
        /// </summary>
        /// <param name="value">An enumeration that states what operation (such as copy or move) was completed.</param>
        /// <remarks>Primarily used for Clipboard operations.</remarks>
        void ReportOperationCompleted( DataPackageOperation value );
    }
}
