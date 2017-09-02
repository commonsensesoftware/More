namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class InteractionRequestTTest
    {
        [Fact]
        public void new_interaction_request_should_set_id()
        {
            // arrange
            var expected = "42";

            // act
            var request = new InteractionRequest<Interaction>( expected );

            // assert
            request.Id.Should().Be( expected );
        }

        [Fact]
        public void id_should_write_expected_value()
        {
            // arrange
            var expected = "42";
            var request = new InteractionRequest<Interaction>();

            request.MonitorEvents();

            // act
            request.Id = expected;

            // assert
            request.Id.Should().Be( expected );
            request.ShouldRaisePropertyChangeFor( r => r.Id );
        }

        [Fact]
        public void request_should_raise_event()
        {
            // arrange
            var expected = new Interaction();
            var request = new InteractionRequest<Interaction>();

            request.MonitorEvents();

            // act
            request.Request( expected );

            // assert
            request.ShouldRaise( nameof( request.Requested ) ).WithArgs<InteractionRequestedEventArgs>( e => e.Interaction == expected );
        }
    }
}