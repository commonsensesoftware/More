namespace More.Composition
{
    using System;

    internal interface ITypeConverter
    {
        object Convert( object value, Type targetType, IFormatProvider formatProvider );
    }
}
