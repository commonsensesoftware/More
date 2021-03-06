﻿namespace More.Windows.Input
{
    using More.IO;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using static System.String;

    /// <summary>
    /// Represents an interaction request to open one or more files.
    /// </summary>
    public class SaveFileInteraction : Interaction
    {
        readonly ObservableCollection<FileType> fileTypeChoices = new ObservableCollection<FileType>();
        string defaultFileExt;
        string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        public SaveFileInteraction() : this( Empty, Empty, Empty ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public SaveFileInteraction( string title ) : this( title, Empty, Empty ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="defaultFileExtension">The default file extension for saved files.</param>
        public SaveFileInteraction( string title, string defaultFileExtension )
            : this( title, defaultFileExtension, Empty ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        /// <param name="defaultFileExtension">The default file extension for saved files.</param>
        /// <param name="suggestedFileName">The suggested save file name.</param>
        public SaveFileInteraction( string title, string defaultFileExtension, string suggestedFileName )
            : base( title )
        {
            Arg.NotNull( defaultFileExtension, nameof( defaultFileExtension ) );
            Arg.NotNull( suggestedFileName, nameof( suggestedFileName ) );

            defaultFileExt = defaultFileExtension;
            fileName = suggestedFileName;
        }

        /// <summary>
        /// Gets or sets the default file name extension applied to files.
        /// </summary>
        /// <value>The default file name extension applied to files.</value>
        public string DefaultFileExtension
        {
            get => defaultFileExt;
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref defaultFileExt, value );
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
            get => fileName;
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref fileName, value );
            }
        }

        /// <summary>
        /// Gets the collection of valid file types that the user can choose to assign to a file.
        /// </summary>
        /// <value>A <see cref="IList{T}"/> of valid <see cref="FileType">file types</see> (extensions)
        /// that the user can use to save a file. Each element in this collection maps
        /// a display name to a corresponding collection of file name extensions.</value>
        /// <example>The following example demonstrates how to specify file types choices.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Windows.Input;
        ///
        /// public void DemoFileTypeChoices()
        /// {
        ///     var saveFile = new SaveFileNotification( "Save" );
        ///     saveFile.FileTypeChoices.Add( new FileType( "Text File", ".txt" ) );
        ///     saveFile.FileTypeChoices.Add( new FileType( "XML File", ".xml" ) );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        public virtual IList<FileType> FileTypeChoices
        {
            get
            {
                Contract.Ensures( Contract.Result<IList<FileType>>() != null );
                return fileTypeChoices;
            }
        }

        /// <summary>
        /// Gets or sets the saved file.
        /// </summary>
        /// <value>The saved <see cref="IFile">file</see>. This property will be <c>null</c> if the operation was cancelled.</value>
        public IFile SavedFile { get; set; }
    }
}