namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using Xunit;
    using static Moq.Times;

    public class InteractionExtensionsTest
    {
        [Fact]
        public void execute_default_command_should_not_allow_null_interaction()
        {
            // arrange
            var interaction = default( Interaction );

            // act
            Action executeDefaultCommand = () => interaction.ExecuteDefaultCommand();

            // assert
            executeDefaultCommand.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( interaction ) );
        }

        [Fact]
        public void execute_default_command_should_execute_expected_command()
        {
            // arrange
            var command = new Mock<INamedCommand>();
            var interaction = new Interaction() { DefaultCommandIndex = 0, Commands = { command.Object } };

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            // act
            interaction.ExecuteDefaultCommand();

            // assert
            command.Verify( c => c.Execute( null ), Once() );
        }

        [Fact]
        public void execute_default_command_should_not_execute_disabled_command()
        {
            // arrange
            var command = new Mock<INamedCommand>();
            var interaction = new Interaction() { DefaultCommandIndex = 0, Commands = { command.Object } };

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( false );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            // act
            interaction.ExecuteDefaultCommand();

            // assert
            command.Verify( c => c.Execute( null ), Never() );
        }

        [Fact]
        public void execute_cancel_command_should_not_allow_null_interaction()
        {
            // arrange
            var interaction = default( Interaction );

            // act
            Action executeCancelCommand = () => interaction.ExecuteCancelCommand();

            // assert
            executeCancelCommand.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( interaction ) );
        }

        [Fact]
        public void execute_cancel_command_should_execute_expected_command()
        {
            // arrange
            var command = new Mock<INamedCommand>();
            var interaction = new Interaction() { CancelCommandIndex = 0, Commands = { command.Object } };

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            // act
            interaction.ExecuteCancelCommand();

            // assert
            command.Verify( c => c.Execute( null ), Once() );
        }

        [Fact]
        public void execute_cancel_command_should_not_execute_disabled_command()
        {
            // arrange
            var command = new Mock<INamedCommand>();
            var interaction = new Interaction() { CancelCommandIndex = 0, Commands = { command.Object } };

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( false );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            // act
            interaction.ExecuteCancelCommand();

            // assert
            command.Verify( c => c.Execute( null ), Never() );
        }
    }
}