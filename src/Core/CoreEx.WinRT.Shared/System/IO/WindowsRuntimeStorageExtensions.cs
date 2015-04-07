namespace System.IO
{
    using More.IO;
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using global::Windows.Storage;
    using NativeStorageItem = global::Windows.Storage.IStorageItem;

    /// <summary>
    /// Provides extension methods to convert native <see cref="NativeStorageItem">storage items</see> to
    /// their portable platform implementations.
    /// </summary>
    [CLSCompliant( false )]
    public static class WindowsRuntimeStorageExtensions
    {
        /// <summary>
        /// Returns a platform-neutral version of the file information.
        /// </summary>
        /// <param name="file">The <see cref="StorageFile">file</see> to convert.</param>
        /// <returns>A platform-neutral <see cref="IFile">file</see>.</returns>
        public static IFile AsFile( this StorageFile file )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Ensures( Contract.Result<IFile>() != null );
            return new StorageFileAdapter( file );
        }

        /// <summary>
        /// Returns a platform-neutral version of the folder information.
        /// </summary>
        /// <param name="folder">The <see cref="StorageFolder">folder</see> to convert.</param>
        /// <returns>A platform-neutral <see cref="IFolder">folder</see>.</returns>
        public static IFolder AsFolder( this StorageFolder folder )
        {
            Contract.Requires<ArgumentNullException>( folder != null, "folder" );
            Contract.Ensures( Contract.Result<IFolder>() != null );
            return new StorageFolderAdapter( folder );
        }
    }
}
