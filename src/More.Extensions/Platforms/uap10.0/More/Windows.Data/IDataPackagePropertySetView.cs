namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using global::Windows.Storage.Streams;
    using global::Windows.UI;

    /// <summary>
    /// Defines the behavior of a set of properties to use with an <see cref="IDataPackage"/>.
    /// </summary>
    [CLSCompliant( false )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Matches the WinRT type name." )]
    public interface IDataPackagePropertySetView : IReadOnlyDictionary<string, object>, IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Gets the Uniform Resource Identifier (URI) of the application's location.
        /// </summary>
        /// <value>The <see cref="Uri">Uniform Resource Identifier (URI)</see> of the application.</value>
        Uri ApplicationListingUri { get; }

        /// <summary>
        /// Gets the name of the application that created the <see cref="IDataPackage"/>.
        /// </summary>
        /// <value>Specifies the name of the app that created the <see cref="IDataPackage"/>.</value>
        string ApplicationName { get; }

        /// <summary>
        /// Gets the application link to the content from the source application.
        /// </summary>
        /// <value>The <see cref="Uri">Uniform Resource Identifier (URI)</see> of the application link to shared content.</value>
        Uri ContentSourceApplicationLink { get; }

        /// <summary>
        /// Provides a web link to shared content that's currently displayed in the application.
        /// </summary>
        /// <value>The <see cref="Uri">Uniform Resource Identifier (URI)</see> of the web link to shared content.</value>
        Uri ContentSourceWebLink { get; }

        /// <summary>
        /// Gets text that describes the contents of the <see cref="IDataPackage"/>.
        /// </summary>
        /// <value>A description of the contents.</value>
        string Description { get; }

        /// <summary>
        /// Specifies a vector object that contains the types of files stored in the <see cref="IDataPackage"/>.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}"/> of the types of files stored in the data package.</value>
        IReadOnlyList<string> FileTypes { get; }

        /// <summary>
        /// Gets a background color for the sharing applications's logo.
        /// </summary>
        /// <value>The <see cref="T:Color">color</see> of the <see cref="P:Square30x30Logo">logo's</see> background.</value>
        Color LogoBackgroundColor { get; }

        /// <summary>
        /// Gets the package family name of the source application.
        /// </summary>
        /// <value>The package family name.</value>
        string PackageFamilyName { get; }

        /// <summary>
        /// Gets the source application's logo.
        /// </summary>
        /// <value>The logo's bitmap as a <see cref="IRandomAccessStreamReference">stream</see>.</value>
        [SuppressMessage( "Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "x", Justification = "Matches the WinRT member for which this property abstracts." )]
        IRandomAccessStreamReference Square30x30Logo { get; }

        /// <summary>
        /// Gets a thumbnail image for the <see cref="IDataPackage"/>.
        /// </summary>
        /// <value>The <see cref="IRandomAccessStreamReference">stream</see> that represents the thumbnail image.</value>
        IRandomAccessStreamReference Thumbnail { get; }

        /// <summary>
        /// Gets the text that displays as a title for the contents of the <see cref="IDataPackage"/>.
        /// </summary>
        /// <value>The text that displays as a title for the contents.</value>
        string Title { get; }
    }
}