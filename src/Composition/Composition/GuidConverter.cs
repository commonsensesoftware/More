namespace More.Composition
{
    using System;

    internal sealed class GuidConverter : StringConverter<Guid>
    {
        protected override Guid Convert( string input, Type targetType, IFormatProvider formatProvider )
        {
            return Guid.Parse( input );
        }
    }
}
