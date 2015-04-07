namespace More.Windows.Input
{
    using More.IO;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request to select a folder.
    /// </summary>
    public class SelectFolderInteraction : Interaction
    {
        private readonly ObservableCollection<string> fileTypeFilter = new ObservableCollection<string>();

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
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( fileTypeFilter != null, "fileTypeFilter" );

            this.fileTypeFilter.AddRange( fileTypeFilter );
        }

        /// <summary>
        /// Gets the collection of file types that the folder contains.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of filter strings that specify the file types to display.</value>
        public virtual IList<string> FileTypeFilter
        {
            get
            {
                return this.fileTypeFilter;
            }
        }

        /// <summary>
        /// Gets or sets the selected folder.
        /// </summary>
        /// <value>The initial <see cref="IFolder">folder</see> before a selection is made or
        /// the selected <see cref="IFolder">folder</see>. This property will be <c>null</c>
        /// if the operation is cancelled.</value>
        public IFolder Folder
        {
            get;
            set;
        }
    }
}
