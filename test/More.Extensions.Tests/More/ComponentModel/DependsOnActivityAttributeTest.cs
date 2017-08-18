namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="DependsOnActivityAttribute"/> class.
    /// </summary>
    public class DependsOnActivityAttributeTest
    {
        [Fact]
        public void new_depends_on_activity_attribute_should_not_allow_null_type()
        {
            // arrange
            Type activityType = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new DependsOnActivityAttribute( activityType ) );

            // assert
            Assert.Equal( "activityType", ex.ParamName );
        }
        [Fact]
        public void new_depends_on_activity_attribute_should_set_type()
        {
            // arrange
            var expected = typeof( IActivity );
            var target = new DependsOnActivityAttribute( expected );

            // act
            var actual = target.ActivityType;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}