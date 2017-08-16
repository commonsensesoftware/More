namespace System.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ICommandExtensionsTest
    {
        [Fact]
        public void can_execute_should_not_allow_null_command()
        {
            // arrange
            var command = default( ICommand );

            // act
            Action canExecute = () => command.CanExecute();

            // assert
            canExecute.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( command ) );
        }

        [Fact]
        public void execute_should_not_allow_null_command()
        {
            // arrange
            var command = default( ICommand );

            // act
            Action execute = () => command.Execute();

            // assert
            execute.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( command ) );
        }
    }
}