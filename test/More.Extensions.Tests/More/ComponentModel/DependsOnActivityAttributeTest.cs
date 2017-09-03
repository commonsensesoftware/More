namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class DependsOnActivityAttributeTest
    {
        [Fact]
        public void new_depends_on_activity_attribute_should_not_allow_null_type()
        {
            // arrange
            var activityType = default( Type );

            // act
            Action @new = () => new DependsOnActivityAttribute( activityType );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( activityType ) );
        }
        [Fact]
        public void new_depends_on_activity_attribute_should_set_type()
        {
            // arrange
            var activityType = typeof( IActivity );

            // act
            var target = new DependsOnActivityAttribute( activityType );

            // assert
            target.ActivityType.Should().Be( activityType );
        }
    }
}