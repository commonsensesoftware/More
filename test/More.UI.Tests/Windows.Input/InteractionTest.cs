namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class InteractionTest
    {
        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_interation_should_not_allow_null_title( Func<string, Interaction> newInteraction )
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => newInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Theory]
        [MemberData( nameof( TitleData ) )]
        public void new_interation_should_set_title( Func<string, Interaction> @new )
        {
            // arrange
            var expected = "Title";

            // act
            var interaction = @new( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Fact]
        public void title_should_not_allow_null()
        {
            // arrange
            var value = default( string );
            var interaction = new Interaction();

            // act
            Action setTitle = () => interaction.Title = value;

            // assert
            setTitle.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void title_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var interaction = new Interaction();

            interaction.MonitorEvents();

            // act
            interaction.Title = expected;

            // assert
            interaction.Title.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.Title );
        }

        [Fact]
        public void new_interaction_should_set_content()
        {
            // arrange
            var expected = new object();

            // act
            var interaction = new Interaction( "Test", expected );

            // assert
            interaction.Content.Should().Be( expected );
        }

        [Fact]
        public void content_should_write_expected_value()
        {
            // arrange
            var expected = new object();
            var interaction = new Interaction();

            interaction.MonitorEvents();

            // act
            interaction.Content = expected;

            // assert
            interaction.Content.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.Content );
        }

        [Fact]
        public void default_command_index_should_write_expected_value()
        {
            // arrange
            var expected = 0;
            var interaction = new Interaction();

            interaction.MonitorEvents();

            // act
            interaction.DefaultCommandIndex = expected;
            var actual = interaction.DefaultCommandIndex;

            // assert
            interaction.DefaultCommandIndex.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.DefaultCommandIndex );
        }

        [Fact]
        public void default_command_should_return_expected_command()
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
            interaction.DefaultCommandIndex = 1;

            // assert
            interaction.DefaultCommand.Should().Be( expected );
        }

        [Fact]
        public void cancel_command_index_should_write_expected_value()
        {
            // arrange
            var expected = 0;
            var interaction = new Interaction();

            // act
            interaction.CancelCommandIndex = expected;

            // assert
            interaction.CancelCommandIndex.Should().Be( expected );
        }

        [Fact]
        public void cancel_command_should_return_expected_command()
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
            interaction.CancelCommandIndex = 2;

            // assert
            interaction.CancelCommand.Should().Be( expected );
        }

        public static IEnumerable<object[]> TitleData
        {
            get
            {
                yield return new object[] { new Func<string, Interaction>( title => new Interaction( title ) ) };
                yield return new object[] { new Func<string, Interaction>( title => new Interaction( title, null ) ) };
            }
        }
    }
}