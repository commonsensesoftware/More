namespace More.Windows.Input
{
    using More.IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents an interaction request to select a folder.
    /// </summary>
    public class SelectFolderInteraction : Interaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteraction"/> class.
        /// </summary>
        public SelectFolderInteraction()
            : this( string.Empty )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectFolderInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="fileTypeFilter">An array of filter strings that specify the file types to display.</param>
        public SelectFolderInteraction( string title, params string[] fileTypeFilter )
            : base( title )
        {
            Arg.NotNull( fileTypeFilter, nameof( fileTypeFilter ) );
            FileTypeFilter.AddRange( fileTypeFilter );
        }

        /// <summary>
        /// Gets the collection of file types that the folder contains.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of filter strings that specify the file types to display.</value>
        public virtual IList<string> FileTypeFilter { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the selected folder.
        /// </summary>
        /// <value>The initial <see cref="IFolder">folder</see> before a selection is made or
        /// the selected <see cref="IFolder">folder</see>. This property will be <c>null</c>
        /// if the operation is cancelled.</value>
        public IFolder Folder { get; set; }
    }
}
