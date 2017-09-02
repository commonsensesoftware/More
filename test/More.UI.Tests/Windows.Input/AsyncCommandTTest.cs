namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xunit;

    public class AsyncCommandTTest
    {
        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, Task>, AsyncCommand<object>>( execute => new AsyncCommand<object>( execute ) ) };
                yield return new object[] { new Func<Func<object, Task>, AsyncCommand<object>>( execute => new AsyncCommand<object>( execute, p => true ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_command_should_not_allow_null_execute_method( Func<Func<object, Task>, AsyncCommand<object>> test )
        {
            // arrange
            var executeAsyncMethod = default( Func<object, Task> );

            // act
            Action @new = () => test( executeAsyncMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeAsyncMethod ) );
        }

        [Fact]
        public void new_command_should_not_allow_null_can_execute_method()
        {
            // arrange
            var canExecuteMethod = default( Func<object, bool> );

            // act
            Action @new = () => new AsyncCommand<object>( p => Task.FromResult( 0 ), canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Fact]
        public void execute_should_invoke_callback()
        {
            // arrange
            var execute = new Mock<Func<object, Task>>();
            var command = new AsyncCommand<object>( execute.Object );

            execute.Setup( f => f( It.IsAny<object>() ) ).Returns( Task.FromResult( 0 ) );
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
            var command = new AsyncCommand<object>( p => Task.FromResult( 0 ), canExecute.Object );

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
            var command = new AsyncCommand<object>( p => Task.FromResult( 0 ) );

            command.MonitorEvents();

            // act
            command.RaiseCanExecuteChanged();

            // assert
            command.ShouldRaise( nameof( command.CanExecuteChanged ) );
        }
    }
}