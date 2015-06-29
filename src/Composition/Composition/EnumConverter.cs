namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;

    internal sealed class EnumConverter : StringConverter<Enum>
    {
        protected override Enum Convert( string input, Type targetType, IFormatProvider formatProvider )
        {
            Contract.Assume( targetType != null );
            return (Enum) Enum.Parse( targetType, input );
        }
    }
}
