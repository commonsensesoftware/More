namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;

    public class CancelableCommandTest
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, CancelableCommand> @new )
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
        public void new_named_item_command_should_not_allow_null_name( Func<string, CancelableCommand> test )
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
        public void new_named_item_command_should_not_allow_empty_name( Func<string, CancelableCommand> test )
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
        public void new_named_item_command_should_set_name( Func<string, CancelableCommand> @new )
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
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Action<CancelEventArgs>, CancelableCommand> test )
        {
            // arrange
            var executeMethod = default( Action<CancelEventArgs> );

            // act
            Action @new = () => test( executeMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<CancelEventArgs, bool>, CancelableCommand> test )
        {
            // arrange
            var canExecuteMethod = default( Func<object, bool> );

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
            var execute = new Mock<Action<CancelEventArgs>>();
            var command = new CancelableCommand( "Cancel", execute.Object );

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) );
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
            var execute = new Mock<Action<CancelEventArgs>>();
            var command = new CancelableCommand( "Cancel", execute.Object );

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) ).Callback( ( CancelEventArgs e ) => e.Cancel = true );
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
                yield return new object[] { new Func<string, CancelableCommand>( id => new CancelableCommand( id, "Test", DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( id => new CancelableCommand( id, "Test", DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( "1", name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( name, DefaultAction.None, p => true ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( "1", name, DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "Test", execute ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, CancelableCommand>( canExecute => new CancelableCommand( "Test", DefaultAction.None, canExecute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, CancelableCommand>( canExecute => new CancelableCommand( "1", "Test", DefaultAction.None, canExecute ) ) };
            }
        }
    }
}