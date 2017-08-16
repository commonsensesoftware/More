namespace System
{
    using FluentAssertions;
    using More;
    using System.Globalization;
    using Xunit;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;

    public class IFormattableExtensionsTest
    {
        [Fact]
        public void to_string_extension_should_not_allow_null_value()
        {
            // arrange
            var value = default( IFormattable );

            // act
            Action toString = () => IFormattableExtensions.ToString( value, CultureInfo.CurrentCulture, "g" );

            // assert
            toString.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void to_string_extension_should_not_allow_null_or_empty_format( string format )
        {
            // arrange


            // act
            Action toString = () => IFormattableExtensions.ToString( Today, CurrentCulture, format );

            // assert
            toString.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( format ) );
        }

        [Fact]
        public void to_string_extension_should_use_format_provider()
        {
            // arrange
            var provider = new DateTimeFormatProvider();
            var target = new DateTime( 2010, 9, 30 );

            // act
            var text = target.ToString( provider, "%q" );

            // assert
            text.Should().Be( "3" );
        }

        [Fact]
        public void to_string_extension_should_use_culture()
        {
            // arrange
            var target = new DateTime( 2010, 9, 30 );

            // act
            var text = target.ToString( CurrentCulture, "%d" );

            // assert
            text.Should().Be( "30" );
        }

        [Fact]
        public void to_string_extension_should_use_default_format_provider_when_null()
        {
            // arrange
            var target = new DateTime( 2010, 9, 30 );

            // act
            var text = target.ToString( default( IFormatProvider ), "%d" );

            // assert
            text.Should().Be( "30" );
        }
    }
}