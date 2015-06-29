namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="DependsOnActivityAttribute"/> class.
    /// </summary>
    public class DependsOnActivityAttributeTest
    {
        [Fact( DisplayName = "new depends on activity attribute should not allow null type" )]
        public void ConstructorShouldNotAllowNullActivityType()
        {
            // arrange
            Type activityType = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new DependsOnActivityAttribute( activityType ) );

            // assert
            Assert.Equal( "activityType", ex.ParamName );
        }
        [Fact( DisplayName = "new depends on activity attribute should set type" )]
        public void ConstructorShouldSetExpectedActivityType()
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