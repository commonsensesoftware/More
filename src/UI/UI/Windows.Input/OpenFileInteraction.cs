namespace More.Windows.Input
{
    using More.IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
            Arg.NotNull( fileTypeFilter, "fileTypeFilter" );

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
                Contract.Ensures( Contract.Result<IList<IFile>>() != null );
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
                Contract.Ensures( Contract.Result<IList<string>>() != null );
                return this.fileTypeFilter;
            }
        }
    }
}
