namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="NamedCommand{T}"/>.
    /// </summary>
    public class NamedCommandTTest
    {
        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, NamedCommand<object>>( id => new NamedCommand<object>( id, "Test", DefaultAction.None ) ) };
                yield return new object[] { new Func<string, NamedCommand<object>>( id => new NamedCommand<object>( id, "Test", DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, NamedCommand<object>>( name => new NamedCommand<object>( name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, NamedCommand<object>>( name => new NamedCommand<object>( "1", name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, NamedCommand<object>>( name => new NamedCommand<object>( name, DefaultAction.None, p => true ) ) };
                yield return new object[] { new Func<string, NamedCommand<object>>( name => new NamedCommand<object>( "1", name, DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<object>, NamedCommand<object>>( execute => new NamedCommand<object>( "Test", execute ) ) };
                yield return new object[] { new Func<Action<object>, NamedCommand<object>>( execute => new NamedCommand<object>( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Action<object>, NamedCommand<object>>( execute => new NamedCommand<object>( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Action<object>, NamedCommand<object>>( execute => new NamedCommand<object>( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, bool>, NamedCommand<object>>( canExecute => new NamedCommand<object>( "Test", DefaultAction.None, canExecute ) ) };
                yield return new object[] { new Func<Func<object, bool>, NamedCommand<object>>( canExecute => new NamedCommand<object>( "1", "Test", DefaultAction.None, canExecute ) ) };
            }
        }

        [Theory( DisplayName = "new named data item command should set id" )]
        [MemberData( "IdData" )]
        public void ConstructorShouldSetId( Func<string, NamedCommand<object>> @new )
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
        public void ConstructorShouldNotAllowNullName( Func<string, NamedCommand<object>> test )
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
        public void ConstructorShouldNotAllowEmptyName( Func<string, NamedCommand<object>> test )
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
        public void ConstructorShouldSetName( Func<string, NamedCommand<object>> @new )
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
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Action<object>, NamedCommand<object>> test )
        {
            // arrange
            Action<object> executeMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeMethod ) );

            // assert
            Assert.Equal( "executeMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow null can execute method" )]
        [MemberData( "CanExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod( Func<Func<object, bool>, NamedCommand<object>> test )
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
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

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
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

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
            var command = new NamedCommand<object>( "Default", DefaultAction.None );

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
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

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
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

            // act
            Assert.PropertyChanged( command, "Id", () => command.Id = expected );
            var actual = command.Id;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
