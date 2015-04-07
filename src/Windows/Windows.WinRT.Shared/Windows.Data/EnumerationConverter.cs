namespace More.Windows.Data
{
    using System;
    using System.Reflection;

    /// <content>
    /// Provides additional implementation for Windows Runtime applications.
    /// </content>
    public partial class EnumerationConverter : FormatConverter
    {
        private static bool IsBooleanType( Type type )
        {
            if ( type == null )
                return false;

            var expectedType = typeof( bool );

            if ( type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) )
                return expectedType.Equals( type.GenericTypeArguments[0] );

            return expectedType.Equals( type );
        }

        private static bool IsNumericType( Type type, out Type underlyingType, out bool nullable )
        {
            nullable = false;
            underlyingType = type;

            // exit if the type is null or it's an enumeration (we're only interested in literal numeric types)
            if ( type == null || typeof( Enum ).GetTypeInfo().IsAssignableFrom( type.GetTypeInfo() ) )
                return false;

            if ( type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) )
            {
                underlyingType = type.GenericTypeArguments[0];
                nullable = true;
            }

            if ( typeof( byte ).Equals( underlyingType ) )
                return true;

            if ( typeof( decimal ).Equals( underlyingType ) )
                return true;

            if ( typeof( double ).Equals( underlyingType ) )
                return true;

            if ( typeof( short ).Equals( underlyingType ) )
                return true;

            if ( typeof( int ).Equals( underlyingType ) )
                return true;

            if ( typeof( long ).Equals( underlyingType ) )
                return true;

            if ( typeof( sbyte ).Equals( underlyingType ) )
                return true;

            if ( typeof( float ).Equals( underlyingType ) )
                return true;

            if ( typeof( ushort ).Equals( underlyingType ) )
                return true;

            if ( typeof( uint ).Equals( underlyingType ) )
                return true;

            if ( typeof( ulong ).Equals( underlyingType ) )
                return true;

            return false;
        }
    }
}
