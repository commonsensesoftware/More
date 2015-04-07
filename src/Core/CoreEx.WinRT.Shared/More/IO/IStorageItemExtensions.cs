namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.Storage;

    /// <summary>
    /// Provides extension methods to convert <see cref="IStorageItem">storage items</see> to
    /// their platform-specific implementations.
    /// </summary>
    [CLSCompliant( false )]
    public static class IStorageItemExtensions
    {
        /// <summary>
        /// Returns the platform-specific file information.
        /// </summary>
        /// <param name="file">The <see cref="IFile">file</see> to convert.</param>
        /// <returns>The platform-specific <see cref="StorageFile">file information</see>.</returns>
        public static StorageFile AsFile( this IFile file )
        {
            Contract.Requires<ArgumentNullException>( file != null, "file" );
            Contract.Ensures( Contract.Result<StorageFile>() != null );

            var platform = file as IPlatformStorageItem<StorageFile>;

            if ( platform == null )
                throw new NotSupportedException( ExceptionMessage.NativeFileNotSupported.FormatDefault( file.GetType(), typeof( IPlatformStorageItem<StorageFile> ) ) );

            return platform.NativeStorageItem;
        }

        /// <summary>
        /// Returns the platform-specific folder information.
        /// </summary>
        /// <param name="folder">The <see cref="IFolder">folder</see> to convert.</param>
        /// <returns>The platform-specific <see cref="StorageFolder">folder information</see>.</returns>
        public static StorageFolder AsFolder( this IFolder folder )
        {
            Contract.Requires<ArgumentNullException>( folder != null, "folder" );
            Contract.Ensures( Contract.Result<StorageFolder>() != null );

            var platform = folder as IPlatformStorageItem<StorageFolder>;

            if ( platform == null )
                throw new NotSupportedException( ExceptionMessage.NativeFolderNotSupported.FormatDefault( folder.GetType(), typeof( IPlatformStorageItem<StorageFolder> ) ) );

            return platform.NativeStorageItem;
        }
    }
}
