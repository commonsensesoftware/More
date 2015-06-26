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
    /// Provides unit tests for <see cref="AsyncCommand{T}"/>.
    /// </summary>
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

        [Theory( DisplayName = "new command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Func<object, Task>, AsyncCommand<object>> test )
        {
            // arrange
            Func<object, Task> executeAsyncMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeAsyncMethod ) );

            // assert
            Assert.Equal( "executeAsyncMethod", ex.ParamName );
        }

        [Fact( DisplayName = "new command should not allow null can execute method" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod()
        {
            // arrange
            Func<object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new AsyncCommand<object>( p => Task.FromResult( 0 ), canExecuteMethod ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Fact( DisplayName = "execute should invoke callback" )]
        public void ExecuteShouldInvokeCallback()
        {
            // arrange
            var execute = new Mock<Func<object, Task>>();

            execute.Setup( f => f( It.IsAny<object>() ) ).Returns( Task.FromResult( 0 ) );

            var command = new AsyncCommand<object>( execute.Object );
            var executed = false;

            command.Executed += ( s, e ) => executed = true;

            // act
            command.Execute();

            // assert
            Assert.True( executed );
            execute.Verify( f => f( null ), Times.Once() );
        }

        [Fact( DisplayName = "can execute should invoke callback" )]
        public void CanExecuteShouldInvokeCallback()
        {
            // arrange
            var canExecute = new Mock<Func<object, bool>>();

            canExecute.Setup( f => f( It.IsAny<object>() ) ).Returns( true );

            var command = new AsyncCommand<object>( p => Task.FromResult( 0 ), canExecute.Object );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( null ), Times.Once() );
        }

        [Fact( DisplayName = "raise can execute changed should raise event" )]
        public void RaiseCanExecuteChangedShouldRaiseEvent()
        {
            // arrange
            var raised = false;
            var command = new AsyncCommand<object>( p => Task.FromResult( 0 ) );

            command.CanExecuteChanged += ( s, e ) => raised = true;

            // act
            command.RaiseCanExecuteChanged();

            // assert
            Assert.True( raised );
        }
    }
}
