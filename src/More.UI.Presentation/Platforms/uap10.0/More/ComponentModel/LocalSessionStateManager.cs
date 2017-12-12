namespace More.ComponentModel
{
    using IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a local, file-based session state manager.
    /// </summary>
    public class LocalSessionStateManager : ISessionStateManager
    {
        readonly IFileSystem fileSystem;
        readonly List<Type> knownTypes = new List<Type>();
        IDictionary<string, object> sessionState = new Dictionary<string, object>();
        string fileName = "SessionState.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalSessionStateManager"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem">file system</see> used to read and write session state.</param>
        public LocalSessionStateManager( IFileSystem fileSystem )
        {
            Arg.NotNull( fileSystem, nameof( fileSystem ) );
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Gets a read-only list of the registered well-known types.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of well-known <see cref="Type">types</see>.</value>
        protected IReadOnlyList<Type> KnownTypes
        {
            get
            {
                Contract.Ensures( knownTypes != null );
                return knownTypes;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file the session state manager reads and writes to.
        /// </summary>
        /// <value>The name of the session state file. The default value is "SessionState.xml".</value>
        /// <remarks>The specified file name should not contain a path. The session state data is
        /// always read and written to the local application storage.</remarks>
        public string FileName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( fileName ) );
                return fileName;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                fileName = value;
            }
        }

        /// <summary>
        /// Gets a dictionary of key/value pairs representing the current session state.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}">dictionary</see> containing the curren session state.</value>
        public virtual IDictionary<string, object> SessionState => sessionState;

        /// <summary>
        /// Adds a well-known session state type.
        /// </summary>
        /// <param name="sessionStateType">The well-known session state type to register.</param>
        /// <remarks>All intrinsic primitive types are well-known; however, custom session state objects
        /// may require type information in order to be serialized.</remarks>
        public virtual void AddKnownType( Type sessionStateType )
        {
            Arg.NotNull( sessionStateType, nameof( sessionStateType ) );

            if ( !knownTypes.Contains( sessionStateType ) )
            {
                knownTypes.Add( sessionStateType );
            }
        }

        /// <summary>
        /// Restores any previously saved session state asynchronously.
        /// </summary>
        /// <param name="sessionKey">The key which identifies the type of session.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The <paramref name="sessionKey">session key</paramref> can be used to disambiguate
        /// between multiple application launch scenarios.</remarks>
        public virtual async Task RestoreAsync( string sessionKey )
        {
            var localFolder = await fileSystem.GetFolderAsync( "ms-appdata:///local/" ).ConfigureAwait( false );
            var file = await localFolder.TryGetFileAsync( fileName ).ConfigureAwait( false );

            if ( file == null )
            {
                return;
            }

            var serializer = new DataContractSerializer( typeof( Dictionary<string, object> ), knownTypes );
            var restoredState = default( IDictionary<string, object> );

            using ( var stream = await file.OpenReadAsync().ConfigureAwait( false ) )
            {
                restoredState = (IDictionary<string, object>) serializer.ReadObject( stream );
            }

            SessionState.ReplaceAll( restoredState );
        }

        /// <summary>
        /// Saves the current session state asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        public virtual async Task SaveAsync()
        {
            var localFolder = await fileSystem.GetFolderAsync( "ms-appdata:///local/" ).ConfigureAwait( false );
            var file = await localFolder.TryGetFileAsync( fileName ).ConfigureAwait( false );

            if ( file == null )
            {
                if ( SessionState.Count == 0 )
                {
                    return;
                }

                file = await localFolder.CreateFileAsync( fileName ).ConfigureAwait( false );
            }

            var clone = new Dictionary<string, object>( SessionState );
            var serializer = new DataContractSerializer( typeof( Dictionary<string, object> ), knownTypes );

            using ( var stream = await file.OpenReadWriteAsync().ConfigureAwait( false ) )
            {
                serializer.WriteObject( stream, clone );
                await stream.FlushAsync().ConfigureAwait( false );
            }
        }
    }
}