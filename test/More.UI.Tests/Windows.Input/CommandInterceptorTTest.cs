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
    /// Provides unit tests for <see cref="CommandInterceptor{T}"/>.
    /// </summary>
    public class CommandInterceptorTTest
    {
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

        [Theory( DisplayName = "new command interceptor should not allow null command" )]
        [MemberData( "CommandData" )]
        public void ConstructorShouldNotAllowNullCommand( Func<ICommand, CommandInterceptor<object>> test )
        {
            // arrange
            ICommand command = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( command ) );

            // assert
            Assert.Equal( "command", ex.ParamName );
        }

        [Theory( DisplayName = "new command interceptor should not allow null pre-action" )]
        [MemberData( "PreActionData" )]
        public void ConstructorShouldNotAllowNullPreAction( Func<Action<object>, CommandInterceptor<object>> test )
        {
            // arrange
            Action<object> preAction = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( preAction ) );

            // assert
            Assert.Equal( "preAction", ex.ParamName );
        }

        [Theory( DisplayName = "new command interceptor should not allow null post-action" )]
        [MemberData( "PostActionData" )]
        public void ConstructorShouldNotAllowNullPostAction( Func<Action<object>, CommandInterceptor<object>> test )
        {
            // arrange
            Action<object> postAction = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( postAction ) );

            // assert
            Assert.Equal( "postAction", ex.ParamName );
        }

        [Fact( DisplayName = "can execute should invoke underlying command" )]
        public void CanExecuteShouldInvokeCommand()
        {
            // arrange
            var command = new Mock<ICommand>();

            command.Setup( c => c.CanExecute( It.IsAny<object>() ) ).Returns( true );

            var interceptor = new CommandInterceptor<object>( DefaultAction.None, DefaultAction.None, command.Object );

            // act
            Assert.True( interceptor.CanExecute() );

            // assert
            command.Verify( c => c.CanExecute( null ), Times.Once() );
        }

        [Fact( DisplayName = "raise can execute changed should raise event" )]
        public void RaiseCanExecuteChangedShouldRaiseEvent()
        {
            // arrange
            var command = new Mock<INotifyCommandChanged>();

            command.Setup( c => c.RaiseCanExecuteChanged() );

            var interceptor = new CommandInterceptor<object>( DefaultAction.None, DefaultAction.None, command.Object );

            // act
            interceptor.RaiseCanExecuteChanged();

            // assert
            command.Verify( c => c.RaiseCanExecuteChanged(), Times.Once() );
        }

        [Fact( DisplayName = "execute should invoke command" )]
        public void ExecuteShouldInvokeCommand()
        {
            // arrange
            var pre = new Mock<Action<object>>();
            var post = new Mock<Action<object>>();

            pre.Setup( f => f( It.IsAny<object>() ) );
            post.Setup( f => f( It.IsAny<object>() ) );

            var command = new Mock<INotifyCommandChanged>();

            command.Setup( c => c.Execute( It.IsAny<object>() ) ).Raises( c => c.Executed += null, EventArgs.Empty );

            var interceptor = new CommandInterceptor<object>( pre.Object, post.Object, command.Object );
            var raised = false;

            interceptor.Executed += ( s, e ) => raised = true;

            // act
            interceptor.Execute();

            // assert
            pre.Verify( f => f( null ), Times.Once() );
            command.Verify( c => c.Execute( null ), Times.Once() );
            post.Verify( f => f( null ), Times.Once() );
            Assert.True( raised );
        }
    }
}
