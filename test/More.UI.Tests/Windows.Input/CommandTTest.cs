namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Xunit;

    public class CommandTTest
    {
        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<object>, Command<object>>( execute => new Command<object>( execute ) ) };
                yield return new object[] { new Func<Action<object>, Command<object>>( execute => new Command<object>( execute, p => true ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_command_should_not_allow_null_execute_method( Func<Action<object>, Command<object>> test )
        {
            // arrange
            var executeMethod = default( Action<object> );

            // act
            Action @new = () => test( executeMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeMethod ) );
        }

        [Fact]
        public void new_command_should_not_allow_null_can_execute_method()
        {
            // arrange
            var canExecuteMethod = default( Func<object, bool> );

            // act
            Action @new = () => new Command<object>( DefaultAction.None, canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Fact]
        public void execute_should_invoke_callback()
        {
            // arrange
            var execute = new Mock<Action<object>>();
            var command = new Command<object>( execute.Object );

            execute.Setup( f => f( It.IsAny<object>() ) );
            command.MonitorEvents();

            // act
            command.Execute();

            // assert
            execute.Verify( f => f( null ), Times.Once() );
            command.ShouldRaise( nameof( command.Executed ) );
        }

        [Fact]
        public void can_execute_should_invoke_callback()
        {
            // arrange
            var canExecute = new Mock<Func<object, bool>>();
            var command = new Command<object>( DefaultAction.None, canExecute.Object );

            canExecute.Setup( f => f( It.IsAny<object>() ) ).Returns( true );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( null ), Times.Once() );
        }

        [Fact]
        public void raise_can_execute_changed_should_raise_event()
        {
            // arrange
            var command = new Command<object>( DefaultAction.None );

            command.MonitorEvents();

            // act
            command.RaiseCanExecuteChanged();

            // assert
            command.ShouldRaise( nameof( command.CanExecuteChanged ) );
        }
    }
}