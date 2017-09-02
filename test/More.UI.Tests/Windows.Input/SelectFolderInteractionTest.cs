namespace More.Windows.Input
{
    using FluentAssertions;
    using Xunit;

    public class SelectFolderInteractionTest
    {
        [Fact]
        public void new_select_folder_interaction_should_set_title()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new SelectFolderInteraction( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Fact]
        public void new_select_folder_interaction_should_add_file_type_filter()
        {
            // arrange
            var expected = new[] { "*.*", "*.txt", "*.csv" };

            // act
            var interaction = new SelectFolderInteraction( "", expected );

            // assert
            interaction.FileTypeFilter.Should().Equal( expected );
        }
    }
}