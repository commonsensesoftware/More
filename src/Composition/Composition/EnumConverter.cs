namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using static System.Activator;
    using static System.Convert;
    using static System.Enum;

    internal sealed class EnumConverter : ITypeConverter
    {
        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Contract.Assume( targetType != null );

            if ( value == null )
            {
                if ( targetType.IsNullable() )
                    return null;

                return CreateInstance( targetType );
            }

            var sourceType = value.GetType();

            if ( targetType.IsAssignableFrom( sourceType ) )
                return value;

            if ( sourceType.Equals( typeof( string ) ) )
            {
                Type typeArg;

                if ( targetType.IsNullable( out typeArg ) )
                {
                    value = Parse( typeArg, value.ToString() );
                    return CreateInstance( targetType, value );
                }

                return Parse( targetType, value.ToString() );
            }

            return ChangeType( value, targetType, formatProvider );
        }
    }
}
