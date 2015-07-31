namespace More.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel;
    using global::Windows.Storage;

    /// <summary>
    /// Represents a file system that provides access to folders and files.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <summary>
        /// Gets the folder that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the folder to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFolder">folder</see>.</returns>
        public async Task<IFolder> GetFolderAsync( string path )
        {
            Arg.NotNullOrEmpty( path, nameof( path ) );

            var folder = await StorageFolder.GetFolderFromPathAsync( path );
            return folder.AsFolder();
        }

        /// <summary>
        /// Gets the file that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        public async Task<IFile> GetFileAsync( string path )
        {
            Arg.NotNullOrEmpty( path, nameof( path ) );

            if ( !Path.IsPathRooted( path ) )
                path = Path.Combine( path, Package.Current.InstalledLocation.Path );

            var file = await StorageFile.GetFileFromPathAsync( path );
            return file.AsFile();
        }

        /// <summary>
        /// Gets the file that has the specified Uniform Resource Indicator (URI) in the file system.
        /// </summary>
        /// <param name="uri">The <see cref="Uri">URI</see> of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        public async Task<IFile> GetFileAsync( Uri uri )
        {
            Arg.NotNull( uri, nameof( uri ) );

            var file = await StorageFile.GetFileFromApplicationUriAsync( uri );
            return file.AsFile();
        }
    }
}
