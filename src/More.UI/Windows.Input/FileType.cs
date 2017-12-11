namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a file type that can be used for file choices and filters.
    /// </summary>
    public class FileType
    {
        readonly string name;
        readonly IReadOnlyList<string> extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileType" /> class.
        /// </summary>
        /// <param name="name">The file type name.</param>
        /// <param name="extensions">A <see cref="IEnumerable{T}">sequence</see> of related file extensions.</param>
        public FileType( string name, params string[] extensions )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNull( extensions, nameof( extensions ) );

            this.name = name;
            this.extensions = extensions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileType" /> class.
        /// </summary>
        /// <param name="name">The file type name.</param>
        /// <param name="extensions">A <see cref="IEnumerable{T}">sequence</see> of related file extensions.</param>
        public FileType( string name, IEnumerable<string> extensions )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNull( extensions, nameof( extensions ) );

            this.name = name;
            this.extensions = extensions.ToArray();
        }

        /// <summary>
        /// Gets name of the file type.
        /// </summary>
        /// <value>The file type name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( name ) );
                return name;
            }
        }

        /// <summary>
        /// Gets a list of file extensions related to the file type.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see>  of file extensions.</value>
        public IReadOnlyList<string> Extensions
        {
            get
            {
                Contract.Ensures( extensions != null );
                return extensions;
            }
        }
    }
}