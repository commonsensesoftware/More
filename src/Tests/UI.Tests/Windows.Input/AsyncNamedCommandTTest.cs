namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="AsyncNamedCommand{T}"/>.
    /// </summary>
    public class AsyncNamedCommandTTest
    {
        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( id => new AsyncNamedCommand<object>( id, "Test", p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( id => new AsyncNamedCommand<object>( id, "Test", p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( name => new AsyncNamedCommand<object>( name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( name => new AsyncNamedCommand<object>( "1", name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( name => new AsyncNamedCommand<object>( name, p => Task.FromResult( 0 ), p => true ) ) };
                yield return new object[] { new Func<string, AsyncNamedCommand<object>>( name => new AsyncNamedCommand<object>( "1", name, p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, Task>, AsyncNamedCommand<object>>( execute => new AsyncNamedCommand<object>( "Test", execute ) ) };
                yield return new object[] { new Func<Func<object, Task>, AsyncNamedCommand<object>>( execute => new AsyncNamedCommand<object>( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Func<object, Task>, AsyncNamedCommand<object>>( execute => new AsyncNamedCommand<object>( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Func<object, Task>, AsyncNamedCommand<object>>( execute => new AsyncNamedCommand<object>( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, bool>, AsyncNamedCommand<object>>( canExecute => new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ), canExecute ) ) };
                yield return new object[] { new Func<Func<object, bool>, AsyncNamedCommand<object>>( canExecute => new AsyncNamedCommand<object>( "1", "Test", p => Task.FromResult( 0 ), canExecute ) ) };
            }
        }

        [Theory( DisplayName = "new named data item command should set id" )]
        [MemberData( "IdData" )]
        public void ConstructorShouldSetId( Func<string, AsyncNamedCommand<object>> @new )
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
        public void ConstructorShouldNotAllowNullName( Func<string, AsyncNamedCommand<object>> test )
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
        public void ConstructorShouldNotAllowEmptyName( Func<string, AsyncNamedCommand<object>> test )
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
        public void ConstructorShouldSetName( Func<string, AsyncNamedCommand<object>> @new )
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
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Func<object, Task>, AsyncNamedCommand<object>> test )
        {
            // arrange
            Func<object, Task> executeAsyncMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeAsyncMethod ) );

            // assert
            Assert.Equal( "executeAsyncMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow null can execute method" )]
        [MemberData( "CanExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod( Func<Func<object, bool>, AsyncNamedCommand<object>> test )
        {
            // arrange
            Func<object, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( canExecuteMethod ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Theory( DisplayName = "name should not allow null or empty" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void NameShouldNotAllowNullOrEmpty( string value )
        {
            // arrange
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

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
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

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
            var command = new AsyncNamedCommand<object>( "Default", p => Task.FromResult( 0 ) );

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
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

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
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

            // act
            Assert.PropertyChanged( command, "Id", () => command.Id = expected );
            var actual = command.Id;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
