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
    /// Provides unit tests for <see cref="AsyncDataItemCommand{TParameter,TItem}"/>.
    /// </summary>
    public class AsyncDataItemCommandT1T2Test
    {
        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, object, Task>, AsyncDataItemCommand<object, object>>( execute => new AsyncDataItemCommand<object, object>( execute, null ) ) };
                yield return new object[] { new Func<Func<object, object, Task>, AsyncDataItemCommand<object, object>>( execute => new AsyncDataItemCommand<object, object>( execute, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> DataItemData
        {
            get
            {
                yield return new object[] { new Func<object, AsyncDataItemCommand<object, object>>( item => new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), item ) ) };
                yield return new object[] { new Func<object, AsyncDataItemCommand<object, object>>( item => new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, item ) ) };
            }
        }

        [Theory( DisplayName = "new data item command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Func<object, object, Task>, AsyncDataItemCommand<object, object>> test )
        {
            // arrange
            Func<object, object, Task> executeAsyncMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeAsyncMethod ) );

            // assert
            Assert.Equal( "executeAsyncMethod", ex.ParamName );
        }

        [Fact( DisplayName = "new data item command should not allow null can execute method" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod()
        {
            // arrange
            Func<object, object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), canExecuteMethod, null ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new data item command should set data item" )]
        [MemberData( "DataItemData" )]
        public void ConstructorShouldSetDataItem( Func<object, AsyncDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = new object();

            // act
            var command = @new( expected );
            var actual = command.Item;

            // assert
            Assert.Same( expected, actual );
        }

        [Fact( DisplayName = "execute should invoke callback" )]
        public void ExecuteShouldInvokeCallback()
        {
            // arrange
            var dataItem = new object();
            var execute = new Mock<Func<object, object, Task>>();

            execute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) ).Returns( Task.FromResult( 0 ) );

            var command = new AsyncDataItemCommand<object, object>( execute.Object, dataItem );

            // act
            command.Execute();

            // assert
            execute.Verify( f => f( dataItem, null ), Times.Once() );
        }

        [Fact( DisplayName = "can execute should invoke callback" )]
        public void CanExecuteShouldInvokeCallback()
        {
            // arrange
            var dataItem = new object();
            var canExecute = new Mock<Func<object, object, bool>>();

            canExecute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) ).Returns( true );

            var command = new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), canExecute.Object, dataItem );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( dataItem, null ), Times.Once() );
        }
    }
}
