namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class WindowCloseInteractionTest
    {
        [Fact]
        public void new_window_close_interaction_should_set_canceled()
        {
            // arrange
            var interaction = new WindowCloseInteraction( canceled: true );

            // act
            var canceled = interaction.Canceled;

            // assert
            canceled.Should().BeTrue();
        }

        [Fact]
        public void new_window_close_interaction_should_set_titleX2C_contentX2C_and_canceled()
        {
            // arrange
            var content = new object();

            // act
            var interaction = new WindowCloseInteraction( "Test", content, canceled: true );

            // assert
            interaction.ShouldBeEquivalentTo( new { Title = "Test", Content = content, Canceled = true }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void canceled_should_raise_property_changed()
        {
            // arrange
            var interaction = new WindowCloseInteraction();

            interaction.MonitorEvents();

            // act
            interaction.Canceled = true;

            // assert
            interaction.ShouldRaisePropertyChangeFor( i => i.Canceled );
        }
    }
}