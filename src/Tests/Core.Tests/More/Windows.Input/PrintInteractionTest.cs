namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="PrintInteraction"/>.
    /// </summary>
    public class PrintInteractionTest
    {
        [Fact( DisplayName = "new print interaction should not allow null" )]
        public void ConstructorShouldNotAllowNullTitle()
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new PrintInteraction( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Fact( DisplayName = "new print interaction should set title" )]
        public void ConstructorShouldSetTitle()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new PrintInteraction( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "print preview should write expected value" )]
        public void PrintPreviewShouldWriteExpectedValue()
        {
            // arrange
            var expected = true;
            var interaction = new PrintInteraction();

            // act
            Assert.PropertyChanged( interaction, "PrintPreview", () => interaction.PrintPreview = expected );
            var actual = interaction.PrintPreview;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
