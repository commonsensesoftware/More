namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="OpenFileInteraction"/>.
    /// </summary>
    public class OpenFileInteractionTest
    {
        public static IEnumerable<object[]> TitleData
        {
            get
            {
                yield return new object[] { new Func<string, OpenFileInteraction>( title => new OpenFileInteraction( title ) ) };
                yield return new object[] { new Func<string, OpenFileInteraction>( title => new OpenFileInteraction( title, false ) ) };
            }
        }

        public static IEnumerable<object[]> FileTypeFilterData
        {
            get
            {
                yield return new object[] { new Func<FileType[], OpenFileInteraction>( fileTypeFilter => new OpenFileInteraction( "", fileTypeFilter ) ) };
                yield return new object[] { new Func<FileType[], OpenFileInteraction>( fileTypeFilter => new OpenFileInteraction( "", false, fileTypeFilter ) ) };
            }
        }

        [Theory( DisplayName = "new open file interaction should not allow null title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldNotAllowNullTitle( Func<string, OpenFileInteraction> test )
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Theory( DisplayName = "new open file interaction should not allow null file type filter" )]
        [MemberData( "FileTypeFilterData" )]
        public void ConstructorShouldNotAllowNullFileTypeFilter( Func<FileType[], OpenFileInteraction> test )
        {
            // arrange
            FileType[] fileTypeFilter = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( fileTypeFilter ) );

            // assert
            Assert.Equal( "fileTypeFilter", ex.ParamName );
        }

        [Theory( DisplayName = "new open file interaction should set title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldSetTitle( Func<string, OpenFileInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new open file interaction should set file type filter" )]
        [MemberData( "FileTypeFilterData" )]
        public void ConstructorShouldSetFileTypeFilter( Func<FileType[], OpenFileInteraction> @new )
        {
            // arrange
            var expected = new[]
            {
                new FileType( "Text Files", ".txt" ),
                new FileType( "Comma-Separated Values Files", ".csv" ),
                new FileType( "All Files", ".*" )
            };

            // act
            var interaction = @new( expected );
            var actual = interaction.FileTypeFilter;

            // assert
            Assert.Equal( expected.AsEnumerable(), actual.AsEnumerable() );
        }

        [Fact( DisplayName = "new open file interaction should set multiselect option" )]
        public void ConstructorShouldSetMultiselectOption()
        {
            // arrange
            var expected = true;

            // act
            var interaction = new OpenFileInteraction( "", expected );
            var actual = interaction.Multiselect;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "multiselect should write expected value" )]
        public void MultiselectShouldWriteExpectedValue()
        {
            // arrange
            var expected = true;
            var interaction = new OpenFileInteraction();

            // act
            Assert.PropertyChanged( interaction, "Multiselect", () => interaction.Multiselect = expected );
            var actual = interaction.Multiselect;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
