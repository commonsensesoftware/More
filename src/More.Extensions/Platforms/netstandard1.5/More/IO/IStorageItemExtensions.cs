namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Provides extension methods to convert <see cref="IStorageItem">storage items</see> to
    /// their platform-specific implementations.
    /// </summary>
    public static class IStorageItemExtensions
    {
        /// <summary>
        /// Returns the platform-specific file information.
        /// </summary>
        /// <param name="file">The <see cref="IFile">file</see> to convert.</param>
        /// <returns>The platform-specific <see cref="FileInfo">file information</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static FileInfo AsFileInfo( this IFile file )
        {
            Arg.NotNull( file, nameof( file ) );
            Contract.Ensures( Contract.Result<FileInfo>() != null );

            if ( file is IPlatformStorageItem<FileInfo> platform )
            {
                return platform.NativeStorageItem;
            }

            throw new NotSupportedException( ExceptionMessage.NativeFileNotSupported.FormatDefault( file.GetType(), typeof( IPlatformStorageItem<FileInfo> ) ) );
        }

        /// <summary>
        /// Returns the platform-specific folder information.
        /// </summary>
        /// <param name="folder">The <see cref="IFolder">folder</see> to convert.</param>
        /// <returns>The platform-specific <see cref="DirectoryInfo">folder information</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static DirectoryInfo AsDirectoryInfo( this IFolder folder )
        {
            Arg.NotNull( folder, nameof( folder ) );
            Contract.Ensures( Contract.Result<DirectoryInfo>() != null );

            if ( folder is IPlatformStorageItem<DirectoryInfo> platform )
            {
                return platform.NativeStorageItem;
            }

            throw new NotSupportedException( ExceptionMessage.NativeFolderNotSupported.FormatDefault( folder.GetType(), typeof( IPlatformStorageItem<DirectoryInfo> ) ) );
        }
    }
}