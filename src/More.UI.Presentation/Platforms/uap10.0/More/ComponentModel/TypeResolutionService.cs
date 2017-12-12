namespace More.ComponentModel
{
    using global::Windows.ApplicationModel;
    using global::Windows.Storage;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides runtime <see cref="Type">type</see> resolution services for Windows Runtime applications.
    /// </summary>
    public sealed class TypeResolutionService : ITypeResolutionService
    {
        readonly List<AssemblyEntry> entries = new List<AssemblyEntry>();
        volatile bool initialized;

        async Task EnsureAssemblyEntries( StorageFolder folder )
        {
            Contract.Requires( folder != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            if ( initialized )
            {
                return;
            }

            var temp = new List<AssemblyEntry>();

            foreach ( var file in await folder.GetFilesAsync() )
            {
                if ( file.FileType != ".dll" && file.FileType != ".exe" )
                {
                    continue;
                }

                var name = new AssemblyName( Path.GetFileNameWithoutExtension( file.Name ) );
                var assembly = Assembly.Load( name );
                var entry = new AssemblyEntry( file.Path, assembly );

                temp.Add( entry );
            }

            lock ( entries )
            {
                if ( initialized )
                {
                    return;
                }

                initialized = true;
                entries.ReplaceAll( temp );
            }
        }

        void EnsureAssemblyEntries()
        {
            if ( initialized )
            {
                return;
            }

            // note: the current package cannot be accessed on a background thread. get the installation
            // folder now and forward just the folder for enumeration in the background task.
            var folder = Package.Current.InstalledLocation;
            var task = Task.Run( () => EnsureAssemblyEntries( folder ) );

            // this method is not async and in order to make it async, we would have to change the api.
            // instead we run the async operation in the background and wait for the results. this
            // operation only needs to occur once, so although we are blocking (bad), the operation is
            // fast and only needs to happen once. in the future, we might consider changing the api.
            if ( !task.IsCompleted )
            {
                task.Wait();
            }
        }

        Type GetTypeByName( string typeName, bool throwOnError, Exception ex )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );
            Contract.Requires( ex != null );

            EnsureAssemblyEntries();

            var sourceType = new TypeName( typeName );
            var types = ( from entry in entries
                          from type in entry.Assembly.ExportedTypes
                          let targetType = new TypeName( type )
                          where sourceType.IsMatch( targetType )
                          select type ).ToArray();

            switch ( types.Length )
            {
                case 0:
                    if ( throwOnError )
                    {
                        throw ex;
                    }

                    return null;
                case 1:
                    return types[0];
                default:
                    if ( throwOnError )
                    {
                        throw new AmbiguousMatchException( ExceptionMessage.AmbiguousTypeName.FormatDefault( typeName ) );
                    }

                    return null;
            }
        }

        /// <summary>
        /// Gets the requested assembly.
        /// </summary>
        /// <param name="name">The name of the assembly to retrieve.</param>
        /// <returns>An instance of the requested <see cref="Assembly">assembly</see>, or null if no assembly can be located.</returns>
        public Assembly GetAssembly( AssemblyName name ) => GetAssembly( name, true );

        /// <summary>
        /// Gets the path to the file from which the assembly was loaded.
        /// </summary>
        /// <param name="name">The name of the assembly.</param>
        /// <returns>The path to the file from which the assembly was loaded.</returns>
        public string GetPathOfAssembly( AssemblyName name )
        {
            EnsureAssemblyEntries();

            var locations = from entry in entries
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
        public Type GetType( string name ) => GetType( name, true, false );

        /// <summary>
        /// Loads a type with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Type">type</see>. If the type name is not a fully qualified name that
        /// indicates an assembly, this service will search its internal set of referenced assemblies.</param>
        /// <param name="throwOnError">True if this method should throw an exception if the type cannot be located; otherwise,
        /// false, and this method returns null if the type cannot be located.</param>
        /// <returns>An instance of <see cref="Type">type</see> that corresponds to the specified name, or null if no type can be found.</returns>
        public Type GetType( string name, bool throwOnError ) => GetType( name, throwOnError, false );

        /// <summary>
        /// Adds a reference to the specified assembly.
        /// </summary>
        /// <param name="name">An <see cref="AssemblyName"/> that indicates the assembly to reference.</param>
        void ITypeResolutionService.ReferenceAssembly( AssemblyName name ) => throw new PlatformNotSupportedException();

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
            {
                throw new ArgumentNullException( nameof( name ) );
            }

            EnsureAssemblyEntries();

            var assembly = ( from entry in entries
                             let otherName = entry.Assembly.GetName()
                             where name.FullName == otherName.FullName &&
                                   name.Version == otherName.Version &&
                                   name.GetPublicKeyToken().SequenceEqual( otherName.GetPublicKeyToken() )
                             select entry.Assembly ).FirstOrDefault();

            // note: not really sure which exception to throw here. this is effectively
            // the same behavior has Assembly.Load( <AssemblyName> )
            if ( assembly == null && throwOnError )
            {
                throw new FileNotFoundException( ExceptionMessage.AssemblyNotFound.FormatDefault( name.FullName ) );
            }

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
            {
                throw new ArgumentNullException( nameof( name ) );
            }

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

            return GetTypeByName( name, throwOnError, ex );
        }

        sealed class AssemblyEntry
        {
#pragma warning disable SA1401 // Fields should be private
            internal readonly string Location;
            internal readonly Assembly Assembly;
#pragma warning restore SA1401 // Fields should be private

            internal AssemblyEntry( string location, Assembly assembly )
            {
                Contract.Requires( !string.IsNullOrEmpty( location ) );
                Contract.Requires( assembly != null );

                Location = location;
                Assembly = assembly;
            }
        }
    }
}