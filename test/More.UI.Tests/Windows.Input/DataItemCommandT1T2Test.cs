namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Xunit;
    using static Moq.Times;

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

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_data_item_command_should_not_allow_null_execute_method( Func<Action<object, object>, DataItemCommand<object, object>> newDataItemCommand )
        {
            // arrange
            var executeMethod = default( Action<object, object> );

            // act
            Action @new = () => newDataItemCommand( executeMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeMethod ) );
        }

        [Fact]
        public void new_data_item_command_should_not_allow_null_can_execute_method()
        {
            // arrange
            var canExecuteMethod = default( Func<object, object, bool> );

            // act
            Action @new = () => new DataItemCommand<object, object>( DefaultAction.None, canExecuteMethod, null );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        public static IEnumerable<object[]> DataItemData
        {
            get
            {
                yield return new object[] { new Func<object, DataItemCommand<object, object>>( item => new DataItemCommand<object, object>( DefaultAction.None, item ) ) };
                yield return new object[] { new Func<object, DataItemCommand<object, object>>( item => new DataItemCommand<object, object>( DefaultAction.None, ( i, p ) => true, item ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( DataItemData ) )]
        public void new_data_item_command_should_set_data_item( Func<object, DataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = new object();

            // act
            var command = @new( expected );

            // assert
            command.Item.Should().Be( expected );
        }

        [Fact]
        public void execute_should_invoke_callback()
        {
            // arrange
            var dataItem = new object();
            var execute = new Mock<Action<object, object>>();
            var command = new DataItemCommand<object, object>( execute.Object, dataItem );

            execute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) );

            // act
            command.Execute();

            // assert
            execute.Verify( f => f( dataItem, null ), Once() );
        }

        [Fact]
        public void can_execute_should_invoke_callback()
        {
            // arrange
            var dataItem = new object();
            var canExecute = new Mock<Func<object, object, bool>>();
            var command = new DataItemCommand<object, object>( DefaultAction.None, canExecute.Object, dataItem );

            canExecute.Setup( f => f( It.IsAny<object>(), It.IsAny<object>() ) ).Returns( true );

            // act
            command.CanExecute();

            // assert
            canExecute.Verify( f => f( dataItem, null ), Once() );
        }
    }
}