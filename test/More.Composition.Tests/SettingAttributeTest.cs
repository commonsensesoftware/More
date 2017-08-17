namespace More.Composition
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Xunit;

    public class SettingAttributeTest
    {
        public static IEnumerable<object[]> PrimitiveTypeData
        {
            get
            {
                yield return new object[] { "42", typeof( string ), "42" };
                yield return new object[] { 42, typeof( sbyte ), (sbyte) 42 };
                yield return new object[] { 42, typeof( byte ), (byte) 42 };
                yield return new object[] { 42, typeof( short ), (short) 42 };
                yield return new object[] { 42, typeof( ushort ), (ushort) 42 };
                yield return new object[] { 42, typeof( int ), 42 };
                yield return new object[] { 42, typeof( uint ), 42U };
                yield return new object[] { 42L, typeof( long ), 42L };
                yield return new object[] { 42L, typeof( ulong ), 42UL };
                yield return new object[] { 42, typeof( float ), 42f };
                yield return new object[] { 42, typeof( double ), 42d };
                yield return new object[] { 42, typeof( decimal ), 42m };
                yield return new object[] { 42, typeof( sbyte? ), new sbyte?( 42 ) };
                yield return new object[] { 42, typeof( byte? ), new byte?( 42 ) };
                yield return new object[] { 42, typeof( short? ), new short?( 42 ) };
                yield return new object[] { 42, typeof( ushort? ), new ushort?( 42 ) };
                yield return new object[] { 42, typeof( int? ), new int?( 42 ) };
                yield return new object[] { 42, typeof( uint? ), new uint?( 42U ) };
                yield return new object[] { 42L, typeof( long? ), new long?( 42L ) };
                yield return new object[] { 42L, typeof( ulong? ), new ulong?( 42UL ) };
                yield return new object[] { 42, typeof( float? ), new float?( 42f ) };
                yield return new object[] { 42, typeof( double? ), new double?( 42d ) };
                yield return new object[] { 42, typeof( decimal? ), new decimal?( 42m ) };
                yield return new object[] { DayOfWeek.Monday, typeof( DayOfWeek ), DayOfWeek.Monday };
                yield return new object[] { DayOfWeek.Monday, typeof( DayOfWeek? ), new DayOfWeek?( DayOfWeek.Monday ) };
                yield return new object[] { "Monday", typeof( DayOfWeek ), DayOfWeek.Monday };
                yield return new object[] { "Monday", typeof( DayOfWeek? ), new DayOfWeek?( DayOfWeek.Monday ) };
                yield return new object[] { "2aaea99d-b76d-42fd-adc5-6f3b4b5b972c", typeof( Guid ), new Guid( "2aaea99d-b76d-42fd-adc5-6f3b4b5b972c" ) };
                yield return new object[] { "2aaea99d-b76d-42fd-adc5-6f3b4b5b972c", typeof( Guid? ), new Guid?( new Guid( "2aaea99d-b76d-42fd-adc5-6f3b4b5b972c" ) ) };
                yield return new object[] { "04-25-2016", typeof( DateTime ), new DateTime( 2016, 4, 25 ) };
                yield return new object[] { "04-25-2016", typeof( DateTime? ), new DateTime?( new DateTime( 2016, 4, 25 ) ) };
                yield return new object[] { "13:14:50", typeof( TimeSpan ), new TimeSpan( 13, 14, 50 ) };
                yield return new object[] { "13:14:50", typeof( TimeSpan? ), new TimeSpan?( new TimeSpan( 13, 14, 50 ) ) };
                yield return new object[] { "http://tempuri.org/", typeof( Uri ), new Uri( "http://tempuri.org/" ) };
            }
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void new_setting_attribute_should_not_allow_null_or_empty_key( string key )
        {
            // arrange


            // act
            Action @new = () => new SettingAttribute( key );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( key ) );
        }

        [Fact]
        public void new_setting_attribute_should_set_key()
        {
            // arrange
            var expected = "Test";
            var setting = new SettingAttribute( expected );

            // act
            var key = setting.Key;

            // assert
            key.Should().Be( expected );
        }

        [Fact]
        public void default_value_should_initially_equal_null_setting_value()
        {
            // arrange
            var expected = SettingAttribute.NullValue;
            var setting = new SettingAttribute( "Test" );

            // act
            var defaultValue = setting.DefaultValue;

            // assert
            defaultValue.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( PrimitiveTypeData ) )]
        public void convert_should_support_converting_supported_primitive_types( object value, Type targetType, object expected )
        {
            // arrange
            var formatProvider = CultureInfo.CurrentCulture;

            // act
            var result = SettingAttribute.Convert( value, targetType, formatProvider );

            // assert
            result.Should().Be( expected );
        }

        [Fact]
        public void convert_should_leverage_userX2Ddefined_conversions()
        {
            // arrange
            var value = "Test";
            var expected = Tuple.Create( value );
            var targetType = typeof( Tuple<string> );
            var formatProvider = CultureInfo.CurrentCulture;

            SettingAttribute.SetConverter( ( o, t, p ) => Tuple.Create( o.ToString() ) );

            // act
            var result = (Tuple<string>) SettingAttribute.Convert( value, targetType, formatProvider );

            // assert
            result.Should().Be( expected );
        }
    }
}