namespace More.Composition
{
    using System;
    using static System.UriKind;

    sealed class UriConverter : StringConverter<Uri>
    {
        protected override Uri Convert( string input, Type targetType, IFormatProvider formatProvider ) => new Uri( input, RelativeOrAbsolute );
    }
}