namespace System.IO
{
    using More.IO;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods to convert native <see cref="FileSystemInfo">storage items</see> to
    /// their portable platform implementations.
    /// </summary>
    public static class FileSystemInfoExtensions
    {
        /// <summary>
        /// Returns a platform-neutral version of the file information.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo">file</see> to convert.</param>
        /// <returns>A platform-neutral <see cref="IFile">file</see>.</returns>
        public static IFile AsFile( this FileInfo file )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Ensures( Contract.Result<IFile>() != null );
            return new FileInfoAdapter( file );
        }

        /// <summary>
        /// Returns a platform-neutral version of the folder information.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo">directory</see> to convert.</param>
        /// <returns>A platform-neutral <see cref="IFolder">folder</see>.</returns>
        public static IFolder AsFolder( this DirectoryInfo directory )
        {
            Contract.Requires<ArgumentNullException>( directory != null, "directory" );
            Contract.Ensures( Contract.Result<IFolder>() != null );
            return new DirectoryInfoAdapter( directory );
        }
    }
}
