namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SelectFolderInteraction"/>.
    /// </summary>
    public class SelectFolderInteractionTest
    {
        [Fact( DisplayName = "new select folder interaction should set title" )]
        public void ConstructorShouldSetTitle()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new SelectFolderInteraction( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new select folder interaction should add file type filter" )]
        public void ConstructorShouldAddFileTypeFilters()
        {
            // arrange
            var expected = new[] { "*.*", "*.txt", "*.csv" };

            // act
            var interaction = new SelectFolderInteraction( "", expected );
            var actual = interaction.FileTypeFilter;

            // assert
            Assert.Equal( expected.AsEnumerable(), actual.AsEnumerable() );
        }
    }
}
