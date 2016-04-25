namespace More.Composition
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    internal sealed class UserDefinedConverter<T> : ITypeConverter
    {
        private readonly Func<object, Type, IFormatProvider, T> converter;

        internal UserDefinedConverter( Func<object, Type, IFormatProvider, T> converter )
        {
            Contract.Requires( converter != null );
            this.converter = converter;
        }

        public object Convert( object value, Type targetType, IFormatProvider formatProvider ) => converter( value, targetType, formatProvider );
    }
}
