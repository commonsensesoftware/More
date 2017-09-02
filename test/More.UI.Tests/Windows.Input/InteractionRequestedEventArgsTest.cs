namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class InteractionRequestedEventArgsTest
    {
        [Fact]
        public void new_interaction_requested_event_args_should_not_allow_null_interaction()
        {
            // arrange
            var interaction = default( Interaction );

            // act
            Action @new = () => new InteractionRequestedEventArgs( interaction );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( interaction ) );
        }

        [Fact]
        public void new_interaction_requested_event_args_should_set_interaction()
        {
            // arrange
            var expected = new Interaction();

            // act
            var args = new InteractionRequestedEventArgs( expected );

            // assert
            args.Interaction.Should().Be( expected );
        }
    }
}