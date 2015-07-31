namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a warning and error provided to the project system.
    /// </summary>
    public sealed class GeneratorError
    {
        private readonly string message;
        private readonly int? line;
        private readonly int? column;

        private GeneratorError( string message, int? line, int? column )
        {
            this.message = message;
            this.line = line;
            this.column = column;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GeneratorError( string message )
            : this( message, null, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="line">The zero-based line number where the error occurred.</param>
        public GeneratorError( string message, int line )
            : this( message, new int?( line ), null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorError"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="line">The zero-based line number where the error occurred.</param>
        /// <param name="column">The zero-based column number where the error occurred.</param>
        public GeneratorError( string message, int line, int column )
            : this( message, new int?( line ), new int?( column ) )
        {
            Arg.NotNullOrEmpty( message, nameof( message ) );
            Arg.GreaterThanOrEqualTo( line, 0, nameof( line ) );
            Arg.GreaterThanOrEqualTo( column, 0, nameof( column ) );
        }

        /// <summary>
        /// Gets or sets whether the error is a warning.
        /// </summary>
        /// <value>True if the error is a warning; otherwise, false. The default value is <c>false</c>.</value>
        public bool IsWarning
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the warning or error message.
        /// </summary>
        /// <value>The warning or error message.</value>
        public string Message
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( message ) );
                return message;
            }
        }

        /// <summary>
        /// Gets the zero-based line number where the error occurred, if any.
        /// </summary>
        /// <value>The line number where the error occurred or <c>null</c> if the line number is unknown.</value>
        public int? Line
        {
            get
            {
                Contract.Ensures( line == null || line.Value >= 0 );
                return line;
            }
        }

        /// <summary>
        /// Gets the zero-based column number where the error occurred, if any.
        /// </summary>
        /// <value>The column number where the error occurred or <c>null</c> if the column number is unknown.</value>
        public int? Column
        {
            get
            {
                Contract.Ensures( column == null || column.Value >= 0 );
                return column;
            }
        }
    }
}
