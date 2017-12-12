namespace System.Windows
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="Type">type</see> class.
    /// </summary>
    public static partial class TypeExtensions
    {
        const string XamlResourceNameFormat = AppXFormat + ".xaml";
        internal const string AppXFormat = "ms-appx:///{0}";

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
            var uri = new Uri( resourceName, UriKind.Absolute );

            return uri;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not used by all platforms." )]
        internal static Type GetNestedPropertyType( this Type parentType, string propertyPath )
        {
            Contract.Requires( parentType != null );

            if ( parentType == null )
            {
                return null;
            }

            if ( string.IsNullOrEmpty( propertyPath ) )
            {
                return parentType;
            }

            var propertyType = parentType;
            var path = propertyPath.Split( '.' );

            for ( var i = 0; i < path.Length; i++ )
            {
                var property = propertyType.GetRuntimeProperty( path[i] );

                if ( property == null )
                {
                    return null;
                }

                propertyType = property.PropertyType;
            }

            return propertyType;
        }
    }
}