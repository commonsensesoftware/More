namespace More.Composition
{
    using System;

    interface ITypeConverter
    {
        object Convert( object value, Type targetType, IFormatProvider formatProvider );
    }
}