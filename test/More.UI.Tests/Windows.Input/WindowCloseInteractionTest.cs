namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="WindowCloseInteraction"/>.
    /// </summary>
    public class WindowCloseInteractionTest
    {
        [Fact( DisplayName = "new window close interaction should set canceled" )]
        public void ConstructorShouldSetCanceled()
        {
            // arrange
            var expected = true;
            var interaction = new WindowCloseInteraction( expected );

            // act
            var actual = interaction.Canceled;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new window close interaction should set title, content, and canceled" )]
        public void ConstructorShouldSetTitleContentAndCanceled()
        {
            // arrange
            var expectedTitle = "Test";
            var expectedContent = new object();
            var expectedCanceled = true;
            var interaction = new WindowCloseInteraction( expectedTitle, expectedContent, expectedCanceled );

            // act
            var actualTitle = interaction.Title;
            var actualContent = interaction.Content;
            var actualCanceled = interaction.Canceled;

            // assert
            Assert.Equal( expectedTitle, actualTitle );
            Assert.Equal( expectedContent, actualContent );
            Assert.Equal( expectedCanceled, actualCanceled );
        }

        [Fact( DisplayName = "canceled should raise property changed" )]
        public void CanceledShouldRaisePropertyChangedEvent()
        {
            // arrange
            var interaction = new WindowCloseInteraction();
            var property = nameof( WindowCloseInteraction.Canceled );

            // act
            Assert.PropertyChanged( interaction, property, () => interaction.Canceled = true );

            // assert
            Assert.True( interaction.Canceled );
        }
    }
}
