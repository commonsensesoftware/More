namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Xunit;

    public class AsyncCancelableCommandTest
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, AsyncCancelableCommand> @new )
        {
            // arrange
            var id = "42";

            // act
            var command = @new( id );

            // assert
            command.Id.Should().Be( id );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_null_name( Func<string, AsyncCancelableCommand> test )
        {
            // arrange
            var name = default( string );

            // act
            Action @new = () => test( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_empty_name( Func<string, AsyncCancelableCommand> test )
        {
            // arrange
            var name = "";

            // act
            Action @new = () => test( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_set_name( Func<string, AsyncCancelableCommand> @new )
        {
            // arrange
            var name = "Test";

            // act
            var command = @new( name );

            // assert
            command.Name.Should().Be( name );
        }

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand> test )
        {
            // arrange
            var executeAsyncMethod = default( Func<CancelEventArgs, Task> );

            // act
            Action @new = () => test( executeAsyncMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeAsyncMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand> test )
        {
            // arrange
            var canExecuteMethod = default( Func<CancelEventArgs, bool> );

            // act
            Action @new = () => test( canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Fact]
        public void execute_should_invoke_callback()
        {
            // arrange
            var parameter = new CancelEventArgs();
            var execute = new Mock<Func<CancelEventArgs, Task>>();
            var command = new AsyncCancelableCommand( "Cancel", execute.Object );

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) ).Returns( Task.FromResult( 0 ) );
            command.MonitorEvents();

            // act
            command.Execute( parameter );

            // assert
            execute.Verify( f => f( parameter ), Times.Once() );
            command.ShouldRaise( nameof( command.Executed ) );
        }

        [Fact]
        public void execute_should_invoke_callback_and_cancel()
        {
            // arrange
            var parameter = new CancelEventArgs();
            var execute = new Mock<Func<CancelEventArgs, Task>>();
            var command = new AsyncCancelableCommand( "Cancel", execute.Object );

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) )
                   .Callback( ( CancelEventArgs e ) => e.Cancel = true )
                   .Returns( Task.FromResult( 0 ) );

            command.MonitorEvents();

            // act
            command.Execute( parameter );

            // assert
            execute.Verify( f => f( parameter ), Times.Once() );
            command.ShouldNotRaise( nameof( command.Executed ) );
        }

        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncCancelableCommand>( id => new AsyncCancelableCommand( id, "Test", p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( id => new AsyncCancelableCommand( id, "Test", p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( "1", name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( name, p => Task.FromResult( 0 ), p => true ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( "1", name, p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "Test", execute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand>( canExecute => new AsyncCancelableCommand( "Test", p => Task.FromResult( 0 ), canExecute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand>( canExecute => new AsyncCancelableCommand( "1", "Test", p => Task.FromResult( 0 ), canExecute ) ) };
            }
        }
    }
}