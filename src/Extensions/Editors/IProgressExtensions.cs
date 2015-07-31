namespace More.VisualStudio.Editors
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="IProgress{T}"/> interface.
    /// </summary>
    public static class IProgressExtensions
    {
        /// <summary>
        /// Reports the start of a progress operation.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="total">The total number of operations expected to be reported.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Start( this IProgress<GeneratorProgress> progress, int total )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.GreaterThanOrEqualTo( total, 0, nameof( total ) );
            progress.Report( new GeneratorProgress( 0, total ) );
        }

        /// <summary>
        /// Reports the next increment from the specified progress.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="currentProgress">The current <see cref="GeneratorProgress">progress</see>.</param>
        /// <remarks>The <see cref="P:GeneratorProgress.Completed">completed</see> generator progress will
        /// never exceed the <see cref="P:GeneratorProgress.Total">total</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static void Increment( this IProgress<GeneratorProgress> progress, GeneratorProgress currentProgress )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNull( currentProgress, nameof( currentProgress ) );
            progress.Report( new GeneratorProgress( Math.Min( currentProgress.Completed + 1, currentProgress.Total ), currentProgress.Total ) );
        }

        /// <summary>
        /// Reports an error with the specified message, line number, and column number.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The error message to report.</param>
        /// <param name="line">The zero-based line number where the error occurred.</param>
        /// <param name="column">The zero-based column number where the error occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportError( this IProgress<GeneratorProgress> progress, string message, int line, int column )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );
            Arg.GreaterThanOrEqualTo( line, 0, nameof( line ) );
            Arg.GreaterThanOrEqualTo( column, 0, nameof( column ) );
            progress.Report( new GeneratorProgress( new GeneratorError( message, line, column ) ) );
        }

        /// <summary>
        /// Reports an error with the specified message and column number.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The error message to report.</param>
        /// <param name="line">The zero-based line number where the error occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportError( this IProgress<GeneratorProgress> progress, string message, int line )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );
            Arg.GreaterThanOrEqualTo( line, 0, nameof( line ) );
            progress.Report( new GeneratorProgress( new GeneratorError( message, line ) ) );
        }

        /// <summary>
        /// Reports an error with the specified message.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The error message to report.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportError( this IProgress<GeneratorProgress> progress, string message )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );
            progress.Report( new GeneratorProgress( new GeneratorError( message ) ) );
        }

        /// <summary>
        /// Reports a warning with the specified message, line number, and column number.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The warning message to report.</param>
        /// <param name="line">The zero-based line number where the warning occurred.</param>
        /// <param name="column">The zero-based column number where the warning occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportWarning( this IProgress<GeneratorProgress> progress, string message, int line, int column )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );
            Arg.GreaterThanOrEqualTo( line, 0, nameof( line ) );
            Arg.GreaterThanOrEqualTo( column, 0, nameof( column ) );

            var error = new GeneratorError( message, line, column )
            {
                IsWarning = true
            };
            var value = new GeneratorProgress( error );

            progress.Report( value );
        }

        /// <summary>
        /// Reports a warning with the specified message  and column number.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The warning message to report.</param>
        /// <param name="line">The zero-based line number where the warning occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportWarning( this IProgress<GeneratorProgress> progress, string message, int line )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );
            Arg.GreaterThanOrEqualTo( line, 0, nameof( line ) );

            var error = new GeneratorError( message, line )
            {
                IsWarning = true
            };
            var value = new GeneratorProgress( error );

            progress.Report( value );
        }

        /// <summary>
        /// Reports a warning with the specified message.
        /// </summary>
        /// <param name="progress">The extended <see cref="IProgress{T}">progress</see> object.</param>
        /// <param name="message">The warning message to report.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void ReportWarning( this IProgress<GeneratorProgress> progress, string message )
        {
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNullOrEmpty( message, nameof( message ) );

            var error = new GeneratorError( message )
            {
                IsWarning = true
            };
            var value = new GeneratorProgress( error );

            progress.Report( value );
        }
    }
}
