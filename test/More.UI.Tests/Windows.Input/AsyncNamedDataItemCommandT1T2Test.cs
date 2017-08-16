namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="AsyncNamedDataItemCommand{TParameter,TItem}"/>.
    /// </summary>
    public class AsyncNamedDataItemCommandT1T2Test
    {
        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( id => new AsyncNamedDataItemCommand<object, object>( id, "Test", ( i, p ) => Task.FromResult( 0 ), null ) ) };
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( id => new AsyncNamedDataItemCommand<object, object>( id, "Test", ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( name => new AsyncNamedDataItemCommand<object, object>( name, ( i, p ) => Task.FromResult( 0 ), null ) ) };
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( name => new AsyncNamedDataItemCommand<object, object>( "1", name, ( i, p ) => Task.FromResult( 0 ), null ) ) };
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( name => new AsyncNamedDataItemCommand<object, object>( name, ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, null ) ) };
                yield return new object[] { new Func<string, AsyncNamedDataItemCommand<object, object>>( name => new AsyncNamedDataItemCommand<object, object>( "1", name, ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>>( execute => new AsyncNamedDataItemCommand<object, object>( "Test", execute, null ) ) };
                yield return new object[] { new Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>>( execute => new AsyncNamedDataItemCommand<object, object>( "1", "Test", execute, null ) ) };
                yield return new object[] { new Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>>( execute => new AsyncNamedDataItemCommand<object, object>( "Test", execute, ( i, p ) => true, null ) ) };
                yield return new object[] { new Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>>( execute => new AsyncNamedDataItemCommand<object, object>( "1", "Test", execute, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, object, bool>, AsyncNamedDataItemCommand<object, object>>( canExecute => new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), canExecute, null ) ) };
                yield return new object[] { new Func<Func<object, object, bool>, AsyncNamedDataItemCommand<object, object>>( canExecute => new AsyncNamedDataItemCommand<object, object>( "1", "Test", ( i, p ) => Task.FromResult( 0 ), canExecute, null ) ) };
            }
        }

        public static IEnumerable<object[]> DataItemData
        {
            get
            {
                yield return new object[] { new Func<object, AsyncNamedDataItemCommand<object, object>>( item => new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), item ) ) };
                yield return new object[] { new Func<object, AsyncNamedDataItemCommand<object, object>>( item => new AsyncNamedDataItemCommand<object, object>( "1", "Test", ( i, p ) => Task.FromResult( 0 ), item ) ) };
                yield return new object[] { new Func<object, AsyncNamedDataItemCommand<object, object>>( item => new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, item ) ) };
                yield return new object[] { new Func<object, AsyncNamedDataItemCommand<object, object>>( item => new AsyncNamedDataItemCommand<object, object>( "1", "Test", ( i, p ) => Task.FromResult( 0 ), ( i, p ) => true, item ) ) };
            }
        }

        [Theory( DisplayName = "new named data item command should set id" )]
        [MemberData( "IdData" )]
        public void ConstructorShouldSetId( Func<string, AsyncNamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = "42";

            // act
            var command = @new( expected );
            var actual = command.Id;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new named item command should not allow null name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldNotAllowNullName( Func<string, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            string name = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( name ) );

            // assert
            Assert.Equal( "name", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow empty name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldNotAllowEmptyName( Func<string, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            var name = "";

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( name ) );

            // assert
            Assert.Equal( "name", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should set name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldSetName( Func<string, AsyncNamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = "Test";

            // act
            var command = @new( expected );
            var actual = command.Name;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new named item command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            Func<object, object, Task> executeAsyncMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeAsyncMethod ) );

            // assert
            Assert.Equal( "executeAsyncMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow null can execute method" )]
        [MemberData( "CanExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod( Func<Func<object, object, bool>, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            Func<object, object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( canExecuteMethod ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should set data item" )]
        [MemberData( "DataItemData" )]
        public void ConstructorShouldSetDataItem( Func<object, AsyncNamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = new object();

            // act
            var command = @new( expected );
            var actual = command.Item;

            // assert
            Assert.Same( expected, actual );
        }

        [Theory( DisplayName = "name should not allow null or empty" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void NameShouldNotAllowNullOrEmpty( string value )
        {
            // arrange
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => command.Name = value );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "description should not allow null" )]
        public void DescriptionShouldNotAllowNull()
        {
            // arrange
            string value = null;
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => command.Description = value );

            // assert
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "name should write expected value" )]
        public void NameShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var command = new AsyncNamedDataItemCommand<object, object>( "Default", ( i, p ) => Task.FromResult( 0 ), null );

            // act
            Assert.PropertyChanged( command, "Name", () => command.Name = expected );
            var actual = command.Name;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "description should write expected value" )]
        public void DescriptionShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

            // act
            Assert.PropertyChanged( command, "Description", () => command.Description = expected );
            var actual = command.Description;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "id should write expected value" )]
        public void IdShouldWriteExpectedValue()
        {
            // arrange
            var expected = "42";
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

            // act
            Assert.PropertyChanged( command, "Id", () => command.Id = expected );
            var actual = command.Id;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
