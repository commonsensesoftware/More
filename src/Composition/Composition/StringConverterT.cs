namespace More.Composition
{
    using System;
    using System.Diagnostics;

    internal abstract class StringConverter<T> : ITypeConverter
    {
        protected abstract T Convert( string input, Type targetType, IFormatProvider formatProvider );

        public object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Debug.Assert( typeof( T ).Equals( targetType ), "The specified conversion type is not supported by the converter." );
            var input = value as string;
            return input == null ? System.Convert.ChangeType( value, typeof( T ), formatProvider ) : this.Convert( input, targetType, formatProvider );
        }
    }
}
