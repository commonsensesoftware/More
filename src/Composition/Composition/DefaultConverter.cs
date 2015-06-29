namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    internal sealed class DefaultConverter : ITypeConverter
    {
        private static readonly TypeInfo ValueTypeInfo = typeof( ValueType ).GetTypeInfo();
        private static readonly Type NullableType = typeof( Nullable<> );

        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Contract.Assume( targetType != null );

            var targetTypeInfo = targetType.GetTypeInfo();

            if ( value == null )
            {
                // use null for reference types (e.g. not a value type)
                if ( !ValueTypeInfo.IsAssignableFrom( targetTypeInfo ) )
                    return null;

                // use null for reference types; special handling for Nullable<>, which is a nullable value type
                if ( targetTypeInfo.IsGenericType && targetTypeInfo.GetGenericTypeDefinition().Equals( NullableType ) )
                    return null;

                return Activator.CreateInstance( targetType );
            }

            var sourceType = value.GetType();

            // no conversion needed
            if ( sourceType.Equals( targetType ) || targetTypeInfo.IsAssignableFrom( sourceType.GetTypeInfo() ) )
                return value;

            return System.Convert.ChangeType( value, targetType, formatProvider );
        }
    }
}
