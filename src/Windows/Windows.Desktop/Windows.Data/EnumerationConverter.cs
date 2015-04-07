namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <content>
    /// Provides additional implementation for Windows Desktop applications.
    /// </content>
    public partial class EnumerationConverter : FormatConverter
    {
        private static bool IsBooleanType( Type type )
        {
            if ( type == null )
                return false;

            var expectedType = typeof( bool );

            if ( type.IsGenericType && type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) )
                return expectedType.Equals( type.GetGenericArguments()[0] );

            return expectedType.Equals( type );
        }

        private static bool IsNumericType( Type type, out Type underlyingType, out bool nullable )
        {
            nullable = false;
            underlyingType = type;

            // exit if the type is null or it's an enumeration (we're only interested in literal numeric types)
            if ( type == null || typeof( Enum ).IsAssignableFrom( type ) )
                return false;

            if ( type.IsGenericType && type.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) )
            {
                underlyingType = type.GetGenericArguments()[0];
                nullable = true;
            }

            switch ( Type.GetTypeCode( underlyingType ) )
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            return false;
        }
    }
}
