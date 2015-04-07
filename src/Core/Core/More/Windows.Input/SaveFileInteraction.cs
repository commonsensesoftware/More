namespace More.Windows.Input
{
    using More.IO;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request to open one or more files.
    /// </summary>
    public class SaveFileInteraction : Interaction
    {
        private readonly Dictionary<string, IList<string>> fileTypeChoices = new Dictionary<string, IList<string>>();
        private string defaultFileExt;
        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        public SaveFileInteraction()
            : this( string.Empty, string.Empty, string.Empty )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public SaveFileInteraction( string title )
            : this( title, string.Empty, string.Empty )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="defaultFileExtension">The default file extension for saved files.</param>
        public SaveFileInteraction( string title, string defaultFileExtension )
            : this( title, defaultFileExtension, string.Empty )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( defaultFileExtension != null, "defaultFileExtension" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="defaultFileExtension">The default file extension for saved files.</param>
        /// <param name="suggestedFileName">The suggested save file name.</param>
        public SaveFileInteraction( string title, string defaultFileExtension, string suggestedFileName )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
            Contract.Requires<ArgumentNullException>( defaultFileExtension != null, "defaultFileExtension" );
            Contract.Requires<ArgumentNullException>( suggestedFileName != null, "suggestedFileName" );

            this.defaultFileExt = defaultFileExtension;
            this.fileName = suggestedFileName;
        }

        /// <summary>
        /// Gets or sets the default file name extension applied to files.
        /// </summary>
        /// <value>The default file name extension applied to files.</value>
        public string DefaultFileExtension
        {
            get
            {
                return this.defaultFileExt;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.SetProperty( ref this.defaultFileExt, value );
            }
        }

        /// <summary>
        /// Gets or sets the save file name.
        /// </summary>
        /// <value>The save file name.</value>
        /// <remarks>This property represents the suggested file name before a user
        /// specifies a file name and the actual save file name after a user makes
        /// their final selection.</remarks>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.SetProperty( ref this.fileName, value );
            }
        }

        /// <summary>
        /// Gets the collection of valid file types that the user can choose to assign to a file.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> that contains a <see cref="IList{T}">collection</see> of
        /// valid file types (extensions) that the user can use to save a file. Each element in this collection maps
        /// a display name to a corresponding collection of file name extensions.</value>
        /// <example>The following example demonstrates how to specify file types choices.
        /// <code lang="C#">
        /// <![CDATA[
        /// using global::System;
        /// using global::System.Windows.Input;
        /// 
        /// public void DemoFileTypeChoices()
        /// {
        ///     var saveFile = new SaveFileNotification( "Save" );
        ///     saveFile.FileTypeChoices.Add( "Text File", new []{ ".txt" } );
        ///     saveFile.FileTypeChoices.Add( "XML File", new []{ ".xml" } );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        public virtual IDictionary<string, IList<string>> FileTypeChoices
        {
            get
            {
                return this.fileTypeChoices;
            }
        }

        /// <summary>
        /// Gets or sets the saved file.
        /// </summary>
        /// <value>The saved <see cref="IFile">file</see>. This property will be <c>null</c> if the operation was cancelled.</value>
        public IFile SavedFile
        {
            get;
            set;
        }
    }
}
