namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class PrintInteractionTest
    {
        [Fact]
        public void new_print_interaction_should_not_allow_null()
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => new PrintInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Fact]
        public void new_print_interaction_should_set_title()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new PrintInteraction( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Fact]
        public void print_preview_should_write_expected_value()
        {
            // arrange
            var interaction = new PrintInteraction();

            interaction.MonitorEvents();

            // act
            interaction.PrintPreview = true;

            // assert
            interaction.PrintPreview.Should().BeTrue();
            interaction.ShouldRaisePropertyChangeFor( i => i.PrintPreview );
        }
    }
}