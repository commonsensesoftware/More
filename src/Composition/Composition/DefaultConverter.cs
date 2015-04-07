namespace More.Composition
{
    using System;
    using System.Reflection;

    internal sealed class DefaultConverter : ITypeConverter
    {
        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            var targetTypeInfo = targetType.GetTypeInfo();

            if ( value == null )
            {
                // use null for reference types; special handling for Nullable<>, which is a nullable value type
                if ( !targetTypeInfo.IsValueType || ( targetTypeInfo.IsGenericType && targetTypeInfo.GetGenericTypeDefinition().Equals( typeof( Nullable<> ) ) ) )
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
