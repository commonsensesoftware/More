namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ActivityCompletedEventArgs"/>.
    /// </summary>
    public class ActivityCompletedEventArgsTest
    {
        [Fact( DisplayName = "new activity completed event args should not allow null service provider" )]
        public void ConstructorShouldNotAllowNullServiceProvider()
        {
            // arrange
            IServiceProvider serviceProvider = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new ActivityCompletedEventArgs( serviceProvider ) );

            // assert
            Assert.Equal( "serviceProvider", ex.ParamName );
        }

        [Fact( DisplayName = "new activity should set expected service provider" )]
        public void ConstructorShouldSetServiceProviderProperty()
        {
            // arrange
            var expected = ServiceProvider.Default;
            var args = new ActivityCompletedEventArgs( expected );

            // act
            var actual = args.ServiceProvider;

            // assert
            Assert.Same( expected, actual );
        }
    }
}
