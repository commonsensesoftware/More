namespace More.Composition
{
    using System;
    using static System.Guid;

    sealed class GuidConverter : StringConverter<Guid>
    {
        protected override Guid Convert( string input, Type targetType, IFormatProvider formatProvider ) => Parse( input );
    }
}