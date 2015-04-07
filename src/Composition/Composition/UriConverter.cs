namespace More.Composition
{
    using System;

    internal sealed class UriConverter : StringConverter<Uri>
    {
        protected override Uri Convert( string input, Type targetType, IFormatProvider formatProvider )
        {
            return new Uri( input, UriKind.RelativeOrAbsolute );
        }
    }
}
