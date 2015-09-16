namespace More.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel;
    using global::Windows.Storage;

    /// <summary>
    /// Represents a file system that provides access to folders and files.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        private static async Task<IFolder> GetFolderForProtocolAsync( Uri uri )
        {
            var scheme = uri.Scheme.ToLowerInvariant();
            var segments = new Queue<string>( uri.Segments.Select( s => s.Trim( '/' ) ).Where( s => s.Length > 0 ) );
            StorageFolder nativeFolder = null;

            switch ( scheme )
            {
                case "ms-appdata":
                    {
                        // ms-appdata cannot be requested without specifying a folder
                        if ( segments.Count == 0 )
                            return null;

                        var appData = ApplicationData.Current;
                        var name = segments.Dequeue().ToLowerInvariant();

                        // determine which folder is being requested
                        switch ( name )
                        {
                            case "local":
                                nativeFolder = appData.LocalFolder;
                                break;
                            case "roaming":
                                nativeFolder = appData.RoamingFolder;
                                break;
                            case "temp":
                                nativeFolder = appData.TemporaryFolder;
                                break;
                        }

                        break;
                    }
                case "ms-appx":
                    {
                        // ms-appx always starts at the installed package location
                        nativeFolder = Package.Current.InstalledLocation;
                        break;
                    }
                default:
                    return null;
            }
            
            // traveral remainder of the path to the requested subfolder
            while ( segments.Count > 0 )
            {
                var name = segments.Dequeue();
                nativeFolder = await nativeFolder.GetFolderAsync( name );
            }

            return nativeFolder.AsFolder();
        }

        /// <summary>
        /// Gets the folder that has the specified path in the file system.
        /// </summary>
        /// <param name="path">The path of the folder to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFolder">folder</see>.</returns>
        /// <remarks>This implementation of the <see cref="IFileSystem">file system</see> also supports returning special folders
        /// using the platform-specific protocols. A valid path may take the form of "ms-appdata:///local/", "ms-appdata:///roaming/",
        /// "ms-appdata:///temp/", or "ms-appx://". If the path contains a subfolder that exists, that folder will be retrieved relative
        /// to the special folder.</remarks>
        public async Task<IFolder> GetFolderAsync( string path )
        {
            Arg.NotNullOrEmpty( path, nameof( path ) );

            Uri uri;

            if ( Uri.TryCreate( path, UriKind.Absolute, out uri ) )
                return await GetFolderForProtocolAsync( uri ).ConfigureAwait( false );

            return ( await StorageFolder.GetFolderFromPathAsync( path ) ).AsFolder();
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

            return ( await StorageFile.GetFileFromPathAsync( path ) ).AsFile();
        }

        /// <summary>
        /// Gets the file that has the specified Uniform Resource Indicator (URI) in the file system.
        /// </summary>
        /// <param name="uri">The <see cref="Uri">URI</see> of the file to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the retreived <see cref="IFile">file</see>.</returns>
        public async Task<IFile> GetFileAsync( Uri uri )
        {
            Arg.NotNull( uri, nameof( uri ) );
            return ( await StorageFile.GetFileFromApplicationUriAsync( uri ) ).AsFile();
        }
    }
}
