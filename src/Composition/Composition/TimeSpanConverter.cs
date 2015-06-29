namespace More.Composition
{
    using System;

    internal sealed class TimeSpanConverter : StringConverter<TimeSpan>
    {
        protected override TimeSpan Convert( string input, Type targetType, IFormatProvider formatProvider )
        {
            return TimeSpan.Parse( input, formatProvider );
        }
    }
}
