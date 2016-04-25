namespace More.Composition
{
    using System;
    using static System.TimeSpan;

    internal sealed class TimeSpanConverter : StringConverter<TimeSpan>
    {
        protected override TimeSpan Convert( string input, Type targetType, IFormatProvider formatProvider ) => Parse( input, formatProvider );
    }
}
