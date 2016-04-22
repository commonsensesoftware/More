namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="NavigateInteraction"/>.
    /// </summary>
    public class NavigateInteractionTest
    {
        [Fact( DisplayName = "new navigation interaction should set title" )]
        public void ConstructorShouldSetTitle()
        {
            // arrange
            var expected = "Test";
            var interaction = new NavigateInteraction( expected );

            // act
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new navigation interaction should set title and content" )]
        public void ConstructorShouldSetTitleAndContent()
        {
            // arrange
            var expectedTitle = "Test";
            var expectedContent = new object();
            var interaction = new NavigateInteraction( expectedTitle, expectedContent );

            // act
            var actualTitle = interaction.Title;
            var actualContent = interaction.Content;

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedContent, actualContent );
        }
    }
}
