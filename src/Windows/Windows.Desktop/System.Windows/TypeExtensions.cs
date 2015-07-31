namespace System.Windows
{
    using More;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="Type">type</see> class.
    /// </summary>
    public static class TypeExtensions
    {
        private const string XamlResourcesFormat = "{0}.g.resources";
        private const string XamlResourceNameFormat = "{0}.xaml";
        internal const string PackUriFormat = "/{0};component/{1}";
        private static readonly ConcurrentDictionary<string, IReadOnlyList<string>> xamlResources = new ConcurrentDictionary<string, IReadOnlyList<string>>();

        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "resourceName", Justification = "Simplifies the multi-targeting source code." )]
        private static string FixupKeyName( string manifestResourceName, string resourceName )
        {
            Contract.Requires( !string.IsNullOrEmpty( manifestResourceName ) );
            Contract.Requires( !string.IsNullOrEmpty( resourceName ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            // map *.baml to *.xaml until a better method is found
            if ( manifestResourceName.EndsWith( "baml", StringComparison.OrdinalIgnoreCase ) && resourceName.EndsWith( "xaml", StringComparison.OrdinalIgnoreCase ) )
                return Path.ChangeExtension( manifestResourceName, "xaml" );

            return manifestResourceName;
        }

        private static bool MatchResourceName( string manifestResourceName, string resourceName )
        {
            Contract.Requires( !string.IsNullOrEmpty( manifestResourceName ) );
            Contract.Requires( !string.IsNullOrEmpty( resourceName ) );

            var comparison = StringComparison.OrdinalIgnoreCase;

            if ( resourceName.EndsWith( manifestResourceName, comparison ) )
                return true;

            // special case for WFP because XAML resources might be packed in binary form
            // but still resource with the *.xaml extension (ex: InitializeComponent)
            //
            // TODO: find more reliable method to match names in this case
            if ( manifestResourceName.EndsWith( "baml", comparison ) && resourceName.EndsWith( "xaml", comparison ) )
                return Path.ChangeExtension( resourceName, "baml" ).EndsWith( manifestResourceName, comparison );

            return false;
        }

        private static IReadOnlyList<string> MatchResourceNames( IEnumerable<string> resourceNames, string resourceName )
        {
            Contract.Requires( resourceNames != null );
            Contract.Requires( Contract.ForAll( resourceNames, n => !string.IsNullOrEmpty( n ) ) );
            Contract.Requires( !string.IsNullOrEmpty( resourceName ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<string>>() != null );
            Contract.Ensures( Contract.ForAll( Contract.Result<IReadOnlyList<string>>(), n => !string.IsNullOrEmpty( n ) ) );
            
            var query = from name in resourceNames
                        where MatchResourceName( name, resourceName )
                        select FixupKeyName( name, resourceName );

            return query.ToArray();
        }

        private static IReadOnlyList<string> GetResourceNames( Assembly assembly, string resourceName )
        {
            Contract.Requires( assembly != null );
            Contract.Requires( !string.IsNullOrEmpty( resourceName ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<string>>() != null );
            Contract.Ensures( Contract.ForAll( Contract.Result<IReadOnlyList<string>>(), name => !string.IsNullOrEmpty( name ) ) );

            // XAML resources are stored in the *.g.resources element
            var manifestKey = XamlResourcesFormat.FormatInvariant( assembly.GetAssemblyName() );
            IReadOnlyList<string> resourceNames;

            // look up cached resource names
            if ( xamlResources.TryGetValue( manifestKey, out resourceNames ) )
                return resourceNames;

            // no resources
            if ( !assembly.GetManifestResourceNames().Contains( manifestKey, StringComparer.Ordinal ) )
                throw new IOException( ExceptionMessage.MissingResourceException.FormatDefault( resourceName ) );

            // prevent cache from group exponentially
            if ( xamlResources.Count > 100 )
                xamlResources.Clear();

            using ( var stream = assembly.GetManifestResourceStream( manifestKey ) )
            {
                var reader = new System.Resources.ResourceReader( stream );
                resourceNames = new List<string>( reader.Cast<System.Collections.DictionaryEntry>().Select( entry => (string) entry.Key ) );
            }

            // cache resource names so the manifest doesn't have to be reloaded
            xamlResources[manifestKey] = resourceNames;

            return resourceNames;
        }

        private static string ResolveResourceName( Assembly assembly, string resourceName )
        {
            Contract.Requires( assembly != null );
            Contract.Requires( !string.IsNullOrEmpty( resourceName ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var manifestResources = GetResourceNames( assembly, resourceName );
            var resourceNames = MatchResourceNames( manifestResources, resourceName );

            // must be exactly one resource
            // NOTE: WPF and Silverlight throw different exceptions for missing resources; match the behavior
            if ( !resourceNames.Any() )
                throw new IOException( ExceptionMessage.MissingResourceException.FormatDefault( resourceName ) );
            else if ( resourceNames.Count > 1 )
                throw new IOException( ExceptionMessage.AmbiguousResourceException.FormatDefault( resourceName, assembly.GetAssemblyName(), resourceNames.Count ) );

            return resourceNames[0];
        }

        /// <summary>
        /// Creates and returns the pack URI for the XAML resource corresponding to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to create a XAML resource URI for.</param>
        /// <returns>A <see cref="Uri"/> object in the pack URI scheme.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only System.Type is supported." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract" )]
        public static Uri CreateXamlResourceUri( this Type type )
        {
            Arg.NotNull( type, nameof( type ) );
            Contract.Ensures( Contract.Result<Uri>() != null );

            var resourceName = XamlResourceNameFormat.FormatInvariant( type.FullName.Replace( '.', '/' ) );
            var componentPath = ResolveResourceName( type.Assembly, resourceName );
            var uri = new Uri( PackUriFormat.FormatInvariant( type.Assembly.GetAssemblyName(), componentPath ), UriKind.Relative );

            return uri;
        }

        /// <summary>
        /// Creates and returns the pack URI for the specified resource based on the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to create a pack URI for.</param>
        /// <param name="resourceName">The name of the resource (component) to build a URI for. The specified resource is expected to have a path
        /// relative to the extended type.</param>
        /// <returns>A <see cref="Uri"/> object in the pack URI scheme.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only System.Type is supported." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract" )]
        public static Uri CreatePackUri( this Type type, string resourceName )
        {
            Arg.NotNull( type, nameof( type ) );
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Contract.Ensures( Contract.Result<Uri>() != null );
            return type.Assembly.CreatePackUri( resourceName );
        }

        /// <summary>
        /// Creates and returns the pack URI for the specified resource based on the specified assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to create a pack URI for.</param>
        /// <param name="resourceName">The name of the resource (component) to build a URI for. The specified resource is expected to have a path
        /// relative to the extended type.</param>
        /// <returns>A <see cref="Uri"/> object in the pack URI scheme.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only System.Reflection.Assembly is supported." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract" )]
        public static Uri CreatePackUri( this Assembly assembly, string resourceName )
        {
            Arg.NotNull( assembly, nameof( assembly ) );
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Contract.Ensures( Contract.Result<Uri>() != null );
            var componentPath = ResolveResourceName( assembly, resourceName );
            var uri = new Uri( PackUriFormat.FormatInvariant( assembly.GetAssemblyName(), componentPath ), UriKind.Relative );

            return uri;
        }

        internal static string GetAssemblyName( this Assembly assembly )
        {
            Contract.Requires( assembly != null, "assembly" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            return assembly.GetName().Name;
        }

        internal static Type GetNestedPropertyType( this Type parentType, string propertyPath )
        {
            Contract.Requires( parentType != null );

            if ( parentType == null )
                return null;

            if ( string.IsNullOrEmpty( propertyPath ) )
                return parentType;

            var propertyType = parentType;
            var path = propertyPath.Split( '.' );

            for ( int i = 0; i < path.Length; i++ )
            {
                var property = propertyType.GetProperty( path[i] );

                if ( property == null )
                    return null;

                propertyType = property.PropertyType;
            }

            return propertyType;
        }
    }
}
