namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Xunit;

    public class CommandInterceptorTTest
    {
        [Theory]
        [MemberData( nameof( CommandData ) )]
        public void new_command_interceptor_should_not_allow_null_command( Func<ICommand, CommandInterceptor<object>> test )
        {
            // arrange
            var command = default( ICommand );

            // act
            Action @new = () => test( command );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( command ) );
        }

        [Theory]
        [MemberData( nameof( PreActionData ) )]
        public void new_command_interceptor_should_not_allow_null_preX2Daction( Func<Action<object>, CommandInterceptor<object>> test )
        {
            // arrange
            var preAction = default( Action<object> );

            // act
            Action @new = () => test( preAction );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( preAction ) );
        }

        [Theory]
        [MemberData( nameof( PostActionData ) )]
        public void new_command_interceptor_should_not_allow_null_postX2Daction( Func<Action<object>, CommandInterceptor<object>> test )
        {
            // arrange
            var postAction = default( Action<object> );

            // act
            Action @new = () => test( postAction );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( postAction ) );
        }

        [Fact]
        public void can_execute_should_invoke_underlying_command()
        {
            // arrange
            var command = new Mock<ICommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );

            // act
            var interceptor = new CommandInterceptor<object>( DefaultAction.None, DefaultAction.None, command.Object );

            // assert
            interceptor.CanExecute().Should().BeTrue();
            command.Verify( c => c.CanExecute( null ), Times.Once() );
        }

        [Fact]
        public void raise_can_execute_changed_should_raise_event()
        {
            // arrange
            var command = new Mock<INotifyCommandChanged>();
            var interceptor = new CommandInterceptor<object>( DefaultAction.None, DefaultAction.None, command.Object );

            command.Setup( c => c.RaiseCanExecuteChanged() );

            // act
            interceptor.RaiseCanExecuteChanged();

            // assert
            command.Verify( c => c.RaiseCanExecuteChanged(), Times.Once() );
        }

        [Fact]
        public void execute_should_invoke_command()
        {
            // arrange
            var pre = new Mock<Action<object>>();
            var post = new Mock<Action<object>>();
            var command = new Mock<INotifyCommandChanged>();
            var interceptor = new CommandInterceptor<object>( pre.Object, post.Object, command.Object );

            pre.Setup( f => f( It.IsAny<object>() ) );
            post.Setup( f => f( It.IsAny<object>() ) );
            command.Setup( c => c.Execute( It.IsAny<object>() ) ).Raises( c => c.Executed += null, EventArgs.Empty );
            interceptor.MonitorEvents();

            // act
            interceptor.Execute();

            // assert
            pre.Verify( f => f( null ), Times.Once() );
            command.Verify( c => c.Execute( null ), Times.Once() );
            post.Verify( f => f( null ), Times.Once() );
            interceptor.ShouldRaise( nameof( interceptor.Executed ) );
        }

        public static IEnumerable<object[]> CommandData
        {
            get
            {
                yield return new object[] { new Func<ICommand, CommandInterceptor<object>>( c => new CommandInterceptor<object>( DefaultAction.None, c ) ) };
                yield return new object[] { new Func<ICommand, CommandInterceptor<object>>( c => new CommandInterceptor<object>( c, DefaultAction.None ) ) };
                yield return new object[] { new Func<ICommand, CommandInterceptor<object>>( c => new CommandInterceptor<object>( DefaultAction.None, DefaultAction.None, c ) ) };
            }
        }

        public static IEnumerable<object[]> PreActionData
        {
            get
            {
                var command = new Mock<ICommand>().Object;
                yield return new object[] { new Func<Action<object>, CommandInterceptor<object>>( pre => new CommandInterceptor<object>( pre, command ) ) };
                yield return new object[] { new Func<Action<object>, CommandInterceptor<object>>( pre => new CommandInterceptor<object>( pre, DefaultAction.None, command ) ) };
            }
        }

        public static IEnumerable<object[]> PostActionData
        {
            get
            {
                var command = new Mock<ICommand>().Object;
                yield return new object[] { new Func<Action<object>, CommandInterceptor<object>>( post => new CommandInterceptor<object>( command, post ) ) };
                yield return new object[] { new Func<Action<object>, CommandInterceptor<object>>( post => new CommandInterceptor<object>( DefaultAction.None, post, command ) ) };
            }
        }
    }
}