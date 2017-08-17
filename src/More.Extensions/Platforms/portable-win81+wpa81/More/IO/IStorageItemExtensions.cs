namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static StorageFile AsFile( this IFile file )
        {
            Arg.NotNull( file, nameof( file ) );
            Contract.Ensures( Contract.Result<StorageFile>() != null );

            if ( file is IPlatformStorageItem<StorageFile> platform )
            {
                return platform.NativeStorageItem;
            }

            throw new NotSupportedException( ExceptionMessage.NativeFileNotSupported.FormatDefault( file.GetType(), typeof( IPlatformStorageItem<StorageFile> ) ) );
        }

        /// <summary>
        /// Returns the platform-specific folder information.
        /// </summary>
        /// <param name="folder">The <see cref="IFolder">folder</see> to convert.</param>
        /// <returns>The platform-specific <see cref="StorageFolder">folder information</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static StorageFolder AsFolder( this IFolder folder )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Contract.Ensures( Contract.Result<StorageFolder>() != null );

            if ( folder is IPlatformStorageItem<StorageFolder> platform )
            {
                return platform.NativeStorageItem;
            }

            throw new NotSupportedException( ExceptionMessage.NativeFolderNotSupported.FormatDefault( folder.GetType(), typeof( IPlatformStorageItem<StorageFolder> ) ) );
        }
    }
}