namespace More.Configuration
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SettingAttribute"/>
    /// </summary>
    public class SettingAttributeTest
    {
        [Fact( DisplayName = "new setting attribute should set key" )]
        public void ConstructorShouldSetKey()
        {
            // arrange
            var expected = "Test";
            var setting = new SettingAttribute( expected );

            // act
            var actual = setting.Key;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
