namespace System
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Provides extension methods for the <see cref="IFormattable"/> interface.
    /// </summary>
    public static class IFormattableExtensions
    {
        /// <summary>
        /// Converts the specified value to its equivalent string representation using the specified format and provider.
        /// </summary>
        /// <typeparam name="TFormattable">The <see cref="Type">type</see> of object to format.</typeparam>
        /// <param name="value">The extended, formattable object of type <typeparamref name="TFormattable"/>.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> used to format the value.</param>
        /// <param name="format">The string format applied to the value.</param>
        /// <returns>The formatted <see cref="string">string</see> representation of the value.</returns>
        /// <remarks>This method differs from <see cref="IFormattable.ToString(string,IFormatProvider)"/> by leveraging <see cref="ICustomFormatter"/>
        /// if the supplied <see cref="IFormatProvider"/> supports it.</remarks>
        /// <example>This example demonstrates how using the an implementation of ToString which will leverage the <see cref="ICustomFormatter"/> interface.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Globalization;
        ///
        /// var calendar = new GregorianFiscalCalendar( 7 );
        /// var formatProvider = new DateTimeFormatProvider( calendar );
        /// var dateTime = new DateTime( 2010, 7, 1 );
        ///
        /// Console.WriteLine( dateTime.ToString( formatProvider, "'FY'yy, SSS, qqq, 'M'M, 'D'd" ) );
        ///
        /// // prints: "FY11, H1, Q1, M1, D1"
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static string ToString<TFormattable>( this TFormattable value, IFormatProvider formatProvider, string format ) where TFormattable : IFormattable
        {
            Arg.NotNull( value, nameof( value ) );
            Arg.NotNullOrEmpty( format, nameof( format ) );
            Contract.Ensures( Contract.Result<string>() != null );

            if ( formatProvider == null )
            {
                return value.ToString( format, CultureInfo.CurrentCulture );
            }

            var formatter = (ICustomFormatter) formatProvider.GetFormat( typeof( ICustomFormatter ) );

            if ( formatter == null )
            {
                return value.ToString( format, formatProvider );
            }

            return formatter.Format( format, value, formatProvider );
        }
    }
}