namespace More.Windows.Input
{
    using More.IO;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents an interaction request to open one or more files.
    /// </summary>
    public class OpenFileInteraction : Interaction
    {
        private readonly ObservableCollection<IFile> files = new ObservableCollection<IFile>();
        private readonly ObservableCollection<string> fileTypeFilter = new ObservableCollection<string>();
        private bool multiselect;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteraction"/> class.
        /// </summary>
        public OpenFileInteraction()
            : this( string.Empty, false )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="fileTypeFilter">An array of filter strings that specify the file types to display.</param>
        public OpenFileInteraction( string title, params string[] fileTypeFilter )
            : this( title, false, fileTypeFilter )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( fileTypeFilter != null, "fileTypeFilter" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="multiselect">Indicates whether multiple selections are allowed.</param>
        /// <param name="fileTypeFilter">An array of filter strings that specify the file types to display.</param>
        public OpenFileInteraction( string title, bool multiselect, params string[] fileTypeFilter )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( fileTypeFilter != null, "fileTypeFilter" );

            this.multiselect = multiselect;
            this.fileTypeFilter.AddRange( fileTypeFilter );
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple files can be selected.
        /// </summary>
        /// <value>True if multiple selections are allowed; otherwise, false. The default is false.</value>
        public bool Multiselect
        {
            get
            {
                return this.multiselect;
            }
            set
            {
                this.SetProperty( ref this.multiselect, value );
            }
        }

        /// <summary>
        /// Gets a list of selected files.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of selected files.</value>
        public virtual IList<IFile> Files
        {
            get
            {
                return this.files;
            }
        }

        /// <summary>
        /// Gets a filter string that specifies the file types and descriptions to display.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of filter strings that specify the file types to display.</value>
        public IList<string> FileTypeFilter
        {
            get
            {
                return this.fileTypeFilter;
            }
        }
    }
}
