namespace More.Composition
{
    using System;

    internal sealed class EnumConverter : StringConverter<Enum>
    {
        protected override Enum Convert( string input, Type targetType, IFormatProvider formatProvider )
        {
            return (Enum) Enum.Parse( targetType, input );
        }
    }
}
