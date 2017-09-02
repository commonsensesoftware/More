namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class TextInputInteractionTest
    {
        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_text_input_interaction_should_not_allow_null_title( Func<string, TextInputInteraction> newTextInputInteraction )
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => newTextInputInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_text_input_interaction_should_set_title( Func<string, TextInputInteraction> @new )
        {
            // arrange
            var expected = "text";

            // act
            var interaction = @new( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( PromptData ) )]
        public void new_text_input_interaction_should_not_allow_null_prompt( Func<string, TextInputInteraction> newTextInputInteraction )
        {
            // arrange
            var prompt = default( string );

            // act
            Action @new = () => newTextInputInteraction( prompt );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( prompt ) );
        }

        [Fact]
        public void new_text_input_interaction_should_not_allow_null_default_response()
        {
            // arrange
            var defaultResponse = default( string );

            // act
            Action @new = () => new TextInputInteraction( "", "", defaultResponse );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( defaultResponse ) );
        }

        [Theory]
        [MemberData( nameof( PromptData ) )]
        public void new_text_input_interaction_should_set_prompt_to_content( Func<string, TextInputInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );

            // assert
            interaction.Content.Should().Be( expected );
        }

        [Fact]
        public void new_text_input_interaction_should_set_default_response()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new TextInputInteraction( "", "", expected );

            // assert
            interaction.DefaultResponse.Should().Be( expected );
        }

        [Fact]
        public void default_response_should_not_allow_null()
        {
            // arrange
            var value = default( string );
            var interaction = new TextInputInteraction();

            // act
            Action setDefaultResponse = () => interaction.DefaultResponse = value;

            // assert
            setDefaultResponse.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void default_response_should_write_expected_value()
        {
            // arrange
            var interaction = new TextInputInteraction();

            interaction.MonitorEvents();

            // act
            interaction.DefaultResponse = "test";

            // assert
            interaction.DefaultResponse.Should().Be( "test" );
            interaction.ShouldRaisePropertyChangeFor( i => i.DefaultResponse );
        }

        [Fact]
        public void response_should_write_expected_value()
        {
            // arrange
            var interaction = new TextInputInteraction();

            interaction.MonitorEvents();

            // act
            interaction.Response = "test";

            // assert
            interaction.Response.Should().Be( "test" );
            interaction.ShouldRaisePropertyChangeFor( i => i.Response );
        }

        public static IEnumerable<object[]> TitleData
        {
            get
            {
                yield return new object[] { new Func<string, TextInputInteraction>( title => new TextInputInteraction( title, "" ) ) };
                yield return new object[] { new Func<string, TextInputInteraction>( title => new TextInputInteraction( title, "", "" ) ) };
            }
        }

        public static IEnumerable<object[]> PromptData
        {
            get
            {
                yield return new object[] { new Func<string, TextInputInteraction>( prompt => new TextInputInteraction( "", prompt ) ) };
                yield return new object[] { new Func<string, TextInputInteraction>( prompt => new TextInputInteraction( "", prompt, "" ) ) };
            }
        }
    }
}