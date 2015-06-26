namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="TextInputInteraction"/>.
    /// </summary>
    public class TextInputInteractionTest
    {
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

        [Theory( DisplayName = "new text input interaction should not allow null title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldNotAllowNullTitle( Func<string, TextInputInteraction> test )
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Theory( DisplayName = "new text input interaction should set title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldSetTitle( Func<string, TextInputInteraction> @new )
        {
            // arrange
            var expected = "text";

            // act
            var interaction = @new( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new text input interaction should not allow null prompt" )]
        [MemberData( "PromptData" )]
        public void ConstructorShouldNotAllowNullPrompt( Func<string, TextInputInteraction> test )
        {
            // arrange
            string prompt = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( prompt ) );

            // assert
            Assert.Equal( "prompt", ex.ParamName );
        }

        [Fact( DisplayName = "new text input interaction should not allow null default response" )]
        public void ConstructorShouldNotAllowNullDefaultResponse()
        {
            // arrange
            string defaultResponse = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new TextInputInteraction( "", "", defaultResponse ) );

            // assert
            Assert.Equal( "defaultResponse", ex.ParamName );
        }

        [Theory( DisplayName = "new text input interaction should set prompt to content" )]
        [MemberData( "PromptData" )]
        public void ConstructorShouldSetPromptToContent( Func<string, TextInputInteraction> @new )
        {
            // arrange
            var expected = "test";

            // act
            var interaction = @new( expected );
            var actual = interaction.Content;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new text input interaction should set default response" )]
        public void ConstructorShouldSetDefaultResponse()
        {
            // arrange
            var expected = "test";

            // act
            var interaction = new TextInputInteraction( "", "", expected );
            var actual = interaction.DefaultResponse;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "default response should not allow null" )]
        public void DefaultResponseShouldNotAllowNull()
        {
            // arrange
            var interaction = new TextInputInteraction();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.DefaultResponse = null );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "default response should write expected value" )]
        public void DefaultResponseShouldWriteExpectedValue()
        {
            // arrange
            var expected = "test";
            var interaction = new TextInputInteraction();

            // act
            Assert.PropertyChanged( interaction, "DefaultResponse", () => interaction.DefaultResponse = expected );
            var actual = interaction.DefaultResponse;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "response should write expected value" )]
        public void ResponseShouldWriteExpectedValue()
        {
            // arrange
            var expected = "test";
            var interaction = new TextInputInteraction();

            // act
            Assert.PropertyChanged( interaction, "Response", () => interaction.Response = expected );
            var actual = interaction.Response;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
