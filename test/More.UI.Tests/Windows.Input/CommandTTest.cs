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
    /// Provides unit tests for <see cref="Command{T}"/>.
    /// </summary>
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

        [Theory( DisplayName = "new command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Action<object>, Command<object>> test )
        {
            // arrange
            Action<object> executeMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeMethod ) );

            // assert
            Assert.Equal( "executeMethod", ex.ParamName );
        }

        [Fact( DisplayName = "new command should not allow null can execute method" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod()
        {
            // arrange
            Func<object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new Command<object>( DefaultAction.None, canExecuteMethod ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Fact( DisplayName = "execute should invoke callback" )]
        public void ExecuteShouldInvokeCallback()
        {
            // arrange
            var execute = new Mock<Action<object>>();

            execute.Setup( f => f( It.IsAny<object>() ) );

            var command = new Command<object>( execute.Object );
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

            var command = new Command<object>( DefaultAction.None, canExecute.Object );

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
            var command = new Command<object>( DefaultAction.None );

            command.CanExecuteChanged += ( s, e ) => raised = true;

            // act
            command.RaiseCanExecuteChanged();

            // assert
            Assert.True( raised );
        }
    }
}
