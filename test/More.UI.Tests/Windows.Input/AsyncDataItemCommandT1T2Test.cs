namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xunit;

    public class AsyncDataItemCommandT1T2Test
    {
        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_data_item_command_should_not_allow_null_execute_method( Func<Func<object, object, Task>, AsyncDataItemCommand<object, object>> test )
        {
            // arrange
            var executeAsyncMethod = default( Func<object, object, Task> );

            // act
            Action @new = () => test( executeAsyncMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeAsyncMethod ) );
        }

        [Fact]
        public void new_data_item_command_should_not_allow_null_can_execute_method()
        {
            // arrange
            var canExecuteMethod = default( Func<object, object, bool> );

            // act
            Action @new = () => new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), canExecuteMethod, null );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Theory]
        [MemberData( nameof( DataItemData ) )]
        public void new_data_item_command_should_set_data_item( Func<object, AsyncDataItemCommand<object, object>> @new )
        {
            // arrange
            var item = new object();

            // act
            var command = @new( item );

            // assert
            command.Item.Should().BeSameAs( item );
        }

        [Fact]
        public void execute_should_invoke_callback()
        {
            // arrange
            var dataItem = new object();
            var execute = new Mock<Func<object, object, Task>>();
            var command = new AsyncDataItemCommand<object, object>( execute.Object, dataItem );

            execute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) ).Returns( Task.FromResult( 0 ) );

            // act
            command.Execute();

            // assert
            execute.Verify( f => f( dataItem, null ), Times.Once() );
        }

        [Fact]
        public void can_execute_should_invoke_callback()
        {
            // arrange
            var dataItem = new object();
            var canExecute = new Mock<Func<object, object, bool>>();
            var command = new AsyncDataItemCommand<object, object>( ( i, p ) => Task.FromResult( 0 ), canExecute.Object, dataItem );

            canExecute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) ).Returns( true );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( dataItem, null ), Times.Once() );
        }

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
    }
}