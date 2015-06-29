namespace More.Windows.Input
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="InteractionExtensions"/>.
    /// </summary>
    public class InteractionExtensionsTest
    {
        [Fact( DisplayName = "execute default command should not allow null interaction" )]
        public void ExecuteDefaultCommandShouldNotAllowNullInteraction()
        {
            // arrange
            Interaction interaction = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.ExecuteDefaultCommand() );

            // assert
            Assert.Equal( "interaction", ex.ParamName );
        }

        [Fact( DisplayName = "execute default command should execute expected command" )]
        public void ExecuteDefaultCommandShouldExecuteExpectedCommand()
        {
            // arrange
            var command = new Mock<INamedCommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            var interaction = new Interaction()
            {
                DefaultCommandIndex = 0,
                Commands = { command.Object }
            };

            // act
            interaction.ExecuteDefaultCommand();

            // assert
            command.Verify( c => c.Execute( null ), Times.Once() );
        }

        [Fact( DisplayName = "execute default command should not execute disabled command" )]
        public void ExecuteDefaultCommandShouldExecuteDisabledCommand()
        {
            // arrange
            var command = new Mock<INamedCommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( false );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            var interaction = new Interaction()
            {
                DefaultCommandIndex = 0,
                Commands = { command.Object }
            };

            // act
            interaction.ExecuteDefaultCommand();

            // assert
            command.Verify( c => c.Execute( null ), Times.Never() );
        }

        [Fact( DisplayName = "execute cancel command should not allow null interaction" )]
        public void ExecuteCancelCommandShouldNotAllowNullInteraction()
        {
            // arrange
            Interaction interaction = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => interaction.ExecuteCancelCommand() );

            // assert
            Assert.Equal( "interaction", ex.ParamName );
        }

        [Fact( DisplayName = "execute cancel command should execute expected command" )]
        public void ExecuteCancelCommandShouldExecuteExpectedCommand()
        {
            // arrange
            var command = new Mock<INamedCommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            var interaction = new Interaction()
            {
                CancelCommandIndex = 0,
                Commands = { command.Object }
            };

            // act
            interaction.ExecuteCancelCommand();

            // assert
            command.Verify( c => c.Execute( null ), Times.Once() );
        }

        [Fact( DisplayName = "execute cancel command should not execute disabled command" )]
        public void ExecuteCancelCommandShouldExecuteDisabledCommand()
        {
            // arrange
            var command = new Mock<INamedCommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( false );
            command.Setup( c => c.Execute( It.IsAny<object>() ) );

            var interaction = new Interaction()
            {
                CancelCommandIndex = 0,
                Commands = { command.Object }
            };

            // act
            interaction.ExecuteCancelCommand();

            // assert
            command.Verify( c => c.Execute( null ), Times.Never() );
        }
    }
}
