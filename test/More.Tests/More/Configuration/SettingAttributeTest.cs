namespace More.Configuration
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class SettingAttributeTest
    {
        [Fact]
        public void new_setting_attribute_should_set_key()
        {
            // arrange
            var setting = new SettingAttribute( "Test" );

            // act
            var key = setting.Key;

            // assert
            key.Should().Be( "Test" );
        }
    }
}