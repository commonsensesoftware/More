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
    /// Provides unit tests for <see cref="DataItemCommand{TParameter,TItem}"/>.
    /// </summary>
    public class DataItemCommandT1T2Test
    {
        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<object, object>, DataItemCommand<object, object>>( execute => new DataItemCommand<object, object>( execute, null ) ) };
                yield return new object[] { new Func<Action<object, object>, DataItemCommand<object, object>>( execute => new DataItemCommand<object, object>( execute, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> DataItemData
        {
            get
            {
                yield return new object[] { new Func<object, DataItemCommand<object, object>>( item => new DataItemCommand<object, object>( DefaultAction.None, item ) ) };
                yield return new object[] { new Func<object, DataItemCommand<object, object>>( item => new DataItemCommand<object, object>( DefaultAction.None, ( i, p ) => true, item ) ) };
            }
        }

        [Theory( DisplayName = "new data item command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Action<object, object>, DataItemCommand<object, object>> test )
        {
            // arrange
            Action<object, object> executeMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeMethod ) );

            // assert
            Assert.Equal( "executeMethod", ex.ParamName );
        }

        [Fact( DisplayName = "new data item command should not allow null can execute method" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod()
        {
            // arrange
            Func<object, object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new DataItemCommand<object, object>( DefaultAction.None, canExecuteMethod, null ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new data item command should set data item" )]
        [MemberData( "DataItemData" )]
        public void ConstructorShouldSetDataItem( Func<object, DataItemCommand<object, object>> @new )
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
            var execute = new Mock<Action<object, object>>();

            execute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) );

            var command = new DataItemCommand<object, object>( execute.Object, dataItem );

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

            var command = new DataItemCommand<object, object>( DefaultAction.None, canExecute.Object, dataItem );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( dataItem, null ), Times.Once() );
        }
    }
}
