namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::Windows.ApplicationModel;
    using global::Windows.Storage;

    /// <summary>
    /// Provides runtime <see cref="Type">type</see> resolution services for Windows Runtime applications.
    /// </summary>
    public sealed class TypeResolutionService : ITypeResolutionService
    {
        private sealed class AssemblyEntry
        {
            internal readonly string Location;
            internal readonly Assembly Assembly;

            internal AssemblyEntry( string location, Assembly assembly )
            {
                Contract.Requires( !string.IsNullOrEmpty( location ) );
                Contract.Requires( assembly != null );

                this.Location = location;
                this.Assembly = assembly;
            }
        }

        private readonly List<AssemblyEntry> entries = new List<AssemblyEntry>();
        private volatile bool initialized;

        private async Task EnsureAssemblyEntries( StorageFolder folder )
        {
            Contract.Requires( folder != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            if ( this.initialized )
                return;

            var temp = new List<AssemblyEntry>();

            foreach ( var file in await folder.GetFilesAsync() )
            {
                if ( file.FileType != ".dll" && file.FileType != ".exe" )
                    continue;

                var name = new AssemblyName( Path.GetFileNameWithoutExtension( file.Name ) );
                var assembly = Assembly.Load( name );
                var entry = new AssemblyEntry( file.Path, assembly );

                temp.Add( entry );
            }

            lock ( this.entries )
            {
                if ( this.initialized )
                    return;

                this.initialized = true;
                this.entries.ReplaceAll( temp );
            }
        }

        private void EnsureAssemblyEntries()
        {
            if ( this.initialized )
                return;

            // note: the current package cannot be accessed on a background thread. get the installation
            // folder now and forward just the folder for enumeration in the background task.
            var folder = Package.Current.InstalledLocation;
            var task = Task.Run( () => this.EnsureAssemblyEntries( folder ) );

            // this method is not async and in order to make it async, we would have to change the api.
            // instead we run the async operation in the background and wait for the results. this
            // operation only needs to occur once, so although we are waiting (bad), the operation is
            // fast and only needs to happen once. in the future, we might consider changing the api.
            if ( !task.IsCompleted )
                task.Wait();
        }

        private Type GetTypeByName( string typeName, bool throwOnError, Exception ex )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );
            Contract.Requires( ex != null );

            this.EnsureAssemblyEntries();

            var sourceType = new TypeName( typeName );
            var types = ( from entry in this.entries
                          from type in entry.Assembly.ExportedTypes
                          let targetType = new TypeName( type )
                          where sourceType.IsMatch( targetType )
                          select type ).ToArray();

            switch ( types.Length )
            {
                case 0:
                    {
                        if ( throwOnError )
                            throw ex;

                        return null;
                    }
                case 1:
                    {
                        return types[0];
                    }
                default:
                    {
                        if ( throwOnError )
                            throw new AmbiguousMatchException( ExceptionMessage.AmbiguousTypeName.FormatDefault( typeName ) );

                        return null;
                    }
            }
        }

        /// <summary>
        /// Gets the requested assembly.
        /// </summary>
        /// <param name="name">The name of the assembly to retrieve.</param>
        /// <returns>An instance of the requested <see cref="Assembly">assembly</see>, or null if no assembly can be located.</returns>
        public Assembly GetAssembly( AssemblyName name )
        {
            return this.GetAssembly( name, true );
        }

        /// <summary>
        /// Gets the path to the file from which the assembly was loaded.
        /// </summary>
        /// <param name="name">The name of the assembly.</param>
        /// <returns>The path to the file from which the assembly was loaded.</returns>
        public string GetPathOfAssembly( AssemblyName name )
        {
            this.EnsureAssemblyEntries();

            var locations = from entry in this.entries
                            let otherName = entry.Assembly.GetName()
                            where name.FullName == otherName.FullName &&
                                  name.Version == otherName.Version &&
                                  name.GetPublicKeyToken().SequenceEqual( otherName.GetPublicKeyToken() )
                            select entry.Location;

            return locations.FirstOrDefault();
        }

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        public Type GetType( string name )
        {
            return this.GetType( name, true, false );
        }

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the type cannot be located; otherwise,
        /// false, and this method returns null if the type cannot be located.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        public Type GetType( string name, bool throwOnError )
        {
            return this.GetType( name, throwOnError, false );
        }

        /// <summary>
        /// Adds a reference to the specified assembly.
        /// </summary>
        /// <param name="name">An <see cref="AssemblyName"/> that indicates the assembly to reference.</param>
        void ITypeResolutionService.ReferenceAssembly( AssemblyName name )
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Gets the requested assembly.
        /// </summary>
        /// <param name="name">The name of the assembly to retrieve.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the assembly cannot be located; otherwise,
        /// false, and this method returns null if the assembly cannot be located.</param>
        /// <returns>An instance of the requested <see cref="Assembly">assembly</see>, or null if no assembly can be located.</returns>
        public Assembly GetAssembly( AssemblyName name, bool throwOnError )
        {
            if ( name == null )
                throw new ArgumentNullException( "name" );

            this.EnsureAssemblyEntries();

            var assembly = ( from entry in this.entries
                             let otherName = entry.Assembly.GetName()
                             where name.FullName == otherName.FullName &&
                                   name.Version == otherName.Version &&
                                   name.GetPublicKeyToken().SequenceEqual( otherName.GetPublicKeyToken() )
                             select entry.Assembly ).FirstOrDefault();

            // note: not really sure which exception to throw here. this is effectively
            // the same behavior has Assembly.Load( <AssemblyName> )
            if ( assembly == null && throwOnError )
                throw new FileNotFoundException( ExceptionMessage.AssemblyNotFound.FormatDefault( name.FullName ) );

            return assembly;
        }

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the type cannot be located; otherwise,
        /// false, and this method returns null if the type cannot be located.</param>
        /// <param name="ignoreCase">True to ignore casing when searching for types; otherwise, false.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        public Type GetType( string name, bool throwOnError, bool ignoreCase )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
                throw new ArgumentNullException( "name" );

            Exception ex = null;

            try
            {
                return Type.GetType( name, true );
            }
            catch ( TargetInvocationException e )
            {
                ex = e;
            }
            catch ( TypeInitializationException e )
            {
                ex = e;
            }
            catch ( TypeLoadException e )
            {
                ex = e;
            }
            catch ( FileNotFoundException e )
            {
                ex = e;
            }
            catch
            {
                throw;
            }

            return this.GetTypeByName( name, throwOnError, ex );
        }
    }
}
