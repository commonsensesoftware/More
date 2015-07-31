namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="IFile"/> interface.
    /// </summary>
    public static class IFileExtensions
    {
        /// <summary>
        /// Creates a copy of the file in the specified folder.
        /// </summary>
        /// <param name="file">The extended <see cref="IFile">file</see>.</param>
        /// <param name="destinationFolder">The destination folder where the copy is created.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the  <see cref="IFile">file</see> that represents the copy.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<IFile> CopyAsync( this IFile file, IFolder destinationFolder )
        {
            Arg.NotNull( file, nameof( file ) );
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Contract.Ensures( Contract.Result<Task<IFile>>() != null );
            return file.CopyAsync( destinationFolder, file.Name );
        }

        /// <summary>
        /// Moves the current file to the specified folder.
        /// </summary>
        /// <param name="file">The extended <see cref="IFile">file</see>.</param>
        /// <param name="destinationFolder">The destination folder where the file is moved. This destination folder must be a physical location.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task MoveAsync( this IFile file, IFolder destinationFolder )
        {
            Arg.NotNull( file, nameof( file ) );
            Arg.NotNull( destinationFolder, nameof( destinationFolder ) );
            Contract.Ensures( Contract.Result<Task>() != null );
            return file.MoveAsync( destinationFolder, file.Name );
        }
    }
}
