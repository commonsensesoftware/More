namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ActivityCompletedEventArgsTest
    {
        [Fact]
        public void new_activity_completed_event_args_should_not_allow_null_service_provider()
        {
            // arrange
            var serviceProvider = default( IServiceProvider );

            // act
            Action @new = () => new ActivityCompletedEventArgs( serviceProvider );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( serviceProvider ) );
        }

        [Fact]
        public void new_activity_should_set_expected_service_provider()
        {
            // arrange
            var expected = ServiceProvider.Default;
            var args = new ActivityCompletedEventArgs( expected );

            // act
            var serviceProvider = args.ServiceProvider;

            // assert
            serviceProvider.Should().BeSameAs( expected );
        }
    }
}