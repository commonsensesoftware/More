namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class NavigateInteractionTest
    {
        [Fact]
        public void new_navigation_interaction_should_set_title()
        {
            // arrange
            var expected = "Test";

            // act
            var interaction = new NavigateInteraction( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Fact]
        public void new_navigation_interaction_should_set_title_and_content()
        {
            // arrange
            var content = new object();

            // act
            var interaction = new NavigateInteraction( "Test", content );

            // assert
            interaction.ShouldBeEquivalentTo( new { Title = "Test", Content = content }, o => o.ExcludingMissingMembers() );
        }
    }
}