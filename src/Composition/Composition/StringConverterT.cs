namespace More.Composition
{
    using System;
    using static System.Activator;
    using static System.Convert;

    internal abstract class StringConverter<T> : ITypeConverter
    {
        protected abstract T Convert( string input, Type targetType, IFormatProvider formatProvider );

        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            var input = value as string;
            Type typeArg;

            if ( targetType.IsNullable( out typeArg ) )
            {
                if ( input == null )
                    return null;

                value = Convert( input, typeArg, formatProvider );
                return CreateInstance( targetType, value );
            }

            if ( input == null )
                return ChangeType( value, typeof( T ), formatProvider );

            return Convert( input, targetType, formatProvider );
        }
    }
}
