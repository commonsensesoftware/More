namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using static System.Activator;
    using static System.Convert;

    internal sealed class DefaultConverter : ITypeConverter
    {
        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Contract.Assume( targetType != null );

            if ( value == null )
            {
                if ( !targetType.IsValueType() || targetType.IsNullable() )
                    return null;

                return CreateInstance( targetType );
            }

            var sourceType = value.GetType();

            if ( targetType.IsAssignableFrom( sourceType ) )
                return value;

            Type typeArg;

            if ( targetType.IsNullable( out typeArg ) )
            {
                value = ChangeType( value, typeArg, formatProvider );
                return CreateInstance( targetType, value );
            }

            return ChangeType( value, targetType, formatProvider );
        }
    }
}
