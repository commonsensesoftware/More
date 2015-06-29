namespace More.Windows.Input
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="Interaction"/>.
    /// </summary>
    public class InteractionTest
    {
        public static IEnumerable<object[]> TitleData
        {
            get
            {
                yield return new object[] { new Func<string, Interaction>( title => new Interaction( title ) ) };
                yield return new object[] { new Func<string, Interaction>( title => new Interaction( title, (object) null ) ) };
            }
        }

        [Theory( DisplayName = "new interation should not allow null title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldNotAllowTitle( Func<string, Interaction> test )
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Theory( DisplayName = "new interation should set title" )]
        [MemberData( "TitleData" )]
        public void ConstructorShouldSetTitle( Func<string,Interaction> @new )
        {
            // arrange
            var expected = "Title";
            
            // act
            var interaction = @new( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "title should not allow null" )]
        public void TitleShouldNotAllowNull()
        {
            // arrange
            var interaction = new Interaction();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.Title = null );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "title should write expected value" )]
        public void TitleShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var interaction = new Interaction();

            // act
            Assert.PropertyChanged( interaction, "Title", () => interaction.Title = expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new interaction should set content" )]
        public void ConstructorShouldSetContent()
        {
            // arrange
            var expected = new object();

            // act
            var interaction = new Interaction( "Test", expected );
            var actual = interaction.Content;

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "content should write expected value" )]
        public void ContentShouldWriteExpectedValue()
        {
            // arrange
            var expected = new object();
            var interaction = new Interaction();

            // act
            Assert.PropertyChanged( interaction, "Content", () => interaction.Content = expected );
            var actual = interaction.Content;

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "default command index should write expected value" )]
        public void DefaultCommandIndexShouldWriteExpectedValue()
        {
            // arrange
            var expected = 0;
            var interaction = new Interaction();

            // act
            Assert.PropertyChanged( interaction, "DefaultCommandIndex", () => interaction.DefaultCommandIndex = expected );
            var actual = interaction.DefaultCommandIndex;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "default command should return expected command" )]
        public void DefaultCommandShouldReturnExpectedCommand()
        {
            // arrange
            var expected = new Mock<INamedCommand>().Object;
            var interaction = new Interaction()
            {
                Commands =
                {
                    new Mock<INamedCommand>().Object,
                    expected,
                    new Mock<INamedCommand>().Object
                }
            };

            // act
            Assert.PropertyChanged( interaction, "DefaultCommand", () => interaction.DefaultCommandIndex = 1 );
            var actual = interaction.DefaultCommand;

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "cancel command index should write expected value" )]
        public void CancelCommandIndexShouldWriteExpectedValue()
        {
            // arrange
            var expected = 0;
            var interaction = new Interaction();

            // act
            Assert.PropertyChanged( interaction, "CancelCommandIndex", () => interaction.CancelCommandIndex = expected );
            var actual = interaction.CancelCommandIndex;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "cancel command should return expected command" )]
        public void CancelCommandShouldReturnExpectedCommand()
        {
            // arrange
            var expected = new Mock<INamedCommand>().Object;
            var interaction = new Interaction()
            {
                Commands =
                {
                    new Mock<INamedCommand>().Object,
                    new Mock<INamedCommand>().Object,
                    expected
                }
            };

            // act
            Assert.PropertyChanged( interaction, "CancelCommand", () => interaction.CancelCommandIndex = 2 );
            var actual = interaction.CancelCommand;

            // assert
            Assert.Same( expected, actual );
        }
    }
}
