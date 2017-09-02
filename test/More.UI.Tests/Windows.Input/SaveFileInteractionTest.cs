namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SaveFileInteractionTest
    {
        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_save_file_interaction_should_not_allow_null_title( Func<string, SaveFileInteraction> newSaveFileInteraction )
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => newSaveFileInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Theory]
        [MemberData( nameof( DefaultExtData ) )]
        public void new_save_file_interaction_should_not_allow_null_default_extension( Func<string, SaveFileInteraction> newSaveFileInteraction )
        {
            // arrange
            var defaultFileExtension = default( string );

            // act
            Action @new = () => newSaveFileInteraction( defaultFileExtension );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( defaultFileExtension ) );
        }

        [Fact]
        public void new_save_file_interaction_should_not_allow_null_suggested_file_name()
        {
            // arrange
            var suggestedFileName = default( string );

            // act
            Action @new = () => new SaveFileInteraction( "", "", suggestedFileName );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( suggestedFileName ) );
        }

        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_save_file_interaction_should_set_title( Func<string, SaveFileInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( DefaultExtData ) )]
        public void new_save_file_interaction_should_set_default_extension( Func<string, SaveFileInteraction> @new )
        {
            // arrange
            var expected = "*.txt";

            // act
            var interaction = @new( expected );

            // assert
            interaction.DefaultFileExtension.Should().Be( expected );
        }

        [Fact]
        public void new_save_file_interaction_should_set_suggested_file_name()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new SaveFileInteraction( "", "", expected );

            // assert
            interaction.FileName.Should().Be( expected );
        }

        [Fact]
        public void default_file_extension_should_not_allow_null()
        {
            // arrange
            var value = default( string );
            var interaction = new SaveFileInteraction();

            // act
            Action setDefaultFileExtension = () => interaction.DefaultFileExtension = value;

            // assert
            setDefaultFileExtension.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void default_file_extension_should_write_expected_value()
        {
            // arrange
            var expected = "*.txt";
            var interaction = new SaveFileInteraction();

            interaction.MonitorEvents();

            // act
            interaction.DefaultFileExtension = expected;

            // assert
            interaction.DefaultFileExtension.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.DefaultFileExtension );
        }

        [Fact]
        public void file_name_should_not_allow_null()
        {
            // arrange
            var value = default( string );
            var interaction = new SaveFileInteraction();

            // act
            Action setFileName = () => interaction.FileName = value;

            // assert
            setFileName.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void file_name_should_write_expected_value()
        {
            // arrange
            var expected = "test";
            var interaction = new SaveFileInteraction();

            interaction.MonitorEvents();

            // act
            interaction.FileName = expected;

            // assert
            interaction.FileName.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.FileName );
        }

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
    }
}