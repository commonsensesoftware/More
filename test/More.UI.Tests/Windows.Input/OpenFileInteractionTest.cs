namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class OpenFileInteractionTest
    {
        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_open_file_interaction_should_not_allow_null_title( Func<string, OpenFileInteraction> newOpenFileInteraction )
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => newOpenFileInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Theory]
        [MemberData( nameof( FileTypeFilterData ) )]
        public void new_open_file_interaction_should_not_allow_null_file_type_filter( Func<FileType[], OpenFileInteraction> newOpenFileInteraction )
        {
            // arrange
            var fileTypeFilter = default( FileType[] );

            // act
            Action @new = () => newOpenFileInteraction( fileTypeFilter );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( fileTypeFilter ) );
        }

        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_open_file_interaction_should_set_title( Func<string, OpenFileInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( FileTypeFilterData ) )]
        public void new_open_file_interaction_should_set_file_type_filter( Func<FileType[], OpenFileInteraction> @new )
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

            // assert
            interaction.FileTypeFilter.Should().Equal( expected );
        }

        [Fact]
        public void new_open_file_interaction_should_set_multiselect_option()
        {
            // arrange
            var multiselect = true;

            // act
            var interaction = new OpenFileInteraction( "", multiselect );

            // assert
            interaction.Multiselect.Should().BeTrue();
        }

        [Fact]
        public void multiselect_should_write_expected_value()
        {
            // arrange
            var interaction = new OpenFileInteraction();

            interaction.MonitorEvents();

            // act
            interaction.Multiselect = true;

            // assert
            interaction.Multiselect.Should().BeTrue();
            interaction.ShouldRaisePropertyChangeFor( i => i.Multiselect );
        }

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
    }
}