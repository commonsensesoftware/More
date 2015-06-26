namespace System
{
    using More;
    using System;
    using System.Globalization;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IFormattableExtensions"/>.
    /// </summary>
    public class IFormattableExtensionsTest
    {
        [Fact( DisplayName = "to string extension should not allow null value" )]
        public void ToStringShouldNotAllowNullValue()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IFormattableExtensions.ToString<IFormattable>( (IFormattable) null, CultureInfo.CurrentCulture, "g" ) );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "to string extension should not allow null or empty format" )]
        public void ToStringShouldNotAllowNullOrEmptyFormat()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => IFormattableExtensions.ToString<DateTime>( DateTime.Today, CultureInfo.CurrentCulture, (string) null ) );
            Assert.Equal( "format", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => IFormattableExtensions.ToString<DateTime>( DateTime.Today, CultureInfo.CurrentCulture, "" ) );
            Assert.Equal( "format", ex.ParamName );
        }

        [Fact( DisplayName = "to string extension should use format provider" )]
        public void ToStringShouldLeverageCustomFormatterWhenSupplied()
        {
            var provider = new DateTimeFormatProvider();
            var target = new DateTime( 2010, 9, 30 );
            var actual = target.ToString( provider, "%q" );
            Assert.Equal( "3", actual );
        }

        [Fact( DisplayName = "to string extension should use culture" )]
        public void ToStringShouldReturnCorrectFormatString()
        {
            var provider = CultureInfo.CurrentCulture;
            var target = new DateTime( 2010, 9, 30 );
            var actual = target.ToString( provider, "%d" );
            Assert.Equal( "30", actual );
        }

        [Fact( DisplayName = "to string extension should use default format provider when null" )]
        public void ToStringShouldReturnCorrectFormatStringWhenProviderIsNull()
        {
            IFormatProvider provider = null;
            var target = new DateTime( 2010, 9, 30 );
            var actual = target.ToString( provider, "%d" );
            Assert.Equal( "30", actual );
        }
    }
}
