namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SaveFileInteraction"/>.
    /// </summary>
    public class SaveFileInteractionTest
    {
        public static IEnumerable<object[]> TitleData
        {
            get
            {
                yield return new object[] { new Func<string, SaveFileInteraction>( title => new SaveFileInteraction( title ) ) };
                yield return new object[] { new Func<string, SaveFileInteraction>( title => new SaveFileInteraction( title, "" ) ) };
                yield return new object[] { new Func<string, SaveFileInteraction>( title => new SaveFileInteraction( title, "", "" ) ) };
            }
        }

        public static IEnumerable<object[]> DefaultExtData
        {
            get
            {
                yield return new object[] { new Func<string, SaveFileInteraction>( ext => new SaveFileInteraction( "", ext ) ) };
                yield return new object[] { new Func<string, SaveFileInteraction>( ext => new SaveFileInteraction( "", ext, "" ) ) };
            }
        }

        [Theory( DisplayName = "new save file interaction should not allow null title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldNotAllowNullTitle( Func<string, SaveFileInteraction> test )
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Theory( DisplayName = "new save file interaction should not allow null default extension" )]
        [MemberData( "DefaultExtData" )]
        public void ConstructorShouldNotAllowNullDefaultExtension( Func<string, SaveFileInteraction> test )
        {
            // arrange
            string defaultFileExtension = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( defaultFileExtension ) );

            // assert
            Assert.Equal( "defaultFileExtension", ex.ParamName );
        }

        [Fact( DisplayName = "new save file interaction should not allow null suggested file name" )]
        public void ConstructorShouldNotAllowNullSuggestedFileName()
        {
            // arrange
            string suggestedFileName = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new SaveFileInteraction( "", "", suggestedFileName ) );

            // assert
            Assert.Equal( "suggestedFileName", ex.ParamName );
        }

        [Theory( DisplayName = "new save file interaction should set title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldSetTitle( Func<string, SaveFileInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new save file interaction should set default extension" )]
        [MemberData( "DefaultExtData" )]
        public void ConstructorShouldSetDefaultExtension( Func<string, SaveFileInteraction> @new )
        {
            // arrange
            var expected = "*.txt";

            // act
            var interaction = @new( expected );
            var actual = interaction.DefaultFileExtension;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new save file interaction should set suggested file name" )]
        public void ConstructorShouldSetSuggestedFileName()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new SaveFileInteraction( "", "", expected );
            var actual = interaction.FileName;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "default file extension should not allow null" )]
        public void DefaultFileExtensionShouldNotAllowNull()
        {
            // arrange
            string value = null;
            var interaction = new SaveFileInteraction();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.DefaultFileExtension = value );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "default file extension should write expected value" )]
        public void DefaultFileExtensionShouldWriteExpectedValue()
        {
            // arrange
            var expected = "*.txt";
            var interaction = new SaveFileInteraction();

            // act
            Assert.PropertyChanged( interaction, "DefaultFileExtension", () => interaction.DefaultFileExtension = expected );
            var actual = interaction.DefaultFileExtension;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "file name should not allow null" )]
        public void FileNameShouldNotAllowNull()
        {
            // arrange
            string value = null;
            var interaction = new SaveFileInteraction();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.FileName = value );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "file name should write expected value" )]
        public void FileNameShouldWriteExpectedValue()
        {
            // arrange
            var expected = "test";
            var interaction = new SaveFileInteraction();

            // act
            Assert.PropertyChanged( interaction, "FileName", () => interaction.FileName = expected );
            var actual = interaction.FileName;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
