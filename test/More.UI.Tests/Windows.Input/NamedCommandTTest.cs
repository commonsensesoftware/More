namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class NamedCommandTTest
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, NamedCommand<object>> newNamedCommand )
        {
            // arrange
            var expected = "42";

            // act
            var command = newNamedCommand( expected );

            // assert
            command.Id.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_null_name( Func<string, NamedCommand<object>> newNamedCommand )
        {
            // arrange
            string name = null;

            // act
            Action @new = () => newNamedCommand( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_empty_name( Func<string, NamedCommand<object>> newNamedCommand )
        {
            // arrange
            var name = "";

            // act
            Action @new = () => newNamedCommand( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_set_name( Func<string, NamedCommand<object>> @new )
        {
            // arrange
            var expected = "Test";

            // act
            var command = @new( expected );

            // assert
            command.Name.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Action<object>, NamedCommand<object>> newNamedCommand )
        {
            // arrange
            Action<object> executeMethod = null;

            // act
            Action @new = () => newNamedCommand( executeMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<object, bool>, NamedCommand<object>> newNamedCommand )
        {
            // arrange
            var canExecuteMethod = default( Func<object, bool> );

            // act
            Action @new = () => newNamedCommand( canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void name_should_not_allow_null_or_empty( string value )
        {
            // arrange
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

            // act
            Action setName = () => command.Name = value;

            // assert
            setName.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void description_should_not_allow_null()
        {
            // arrange
            var value = default( string );
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

            // act
            Action setDescription = () => command.Description = value;

            // assert
            setDescription.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void name_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var command = new NamedCommand<object>( "Default", DefaultAction.None );

            command.MonitorEvents();

            // act
            command.Name = expected;

            // assert
            command.Name.Should().Be( expected );
            command.ShouldRaisePropertyChangeFor( c => c.Name );
        }

        [Fact]
        public void description_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

            command.MonitorEvents();

            // act
            command.Description = expected;

            // assert
            command.Description.Should().Be( expected );
            command.ShouldRaisePropertyChangeFor( c => c.Description );
        }

        [Fact]
        public void id_should_write_expected_value()
        {
            // arrange
            var expected = "42";
            var command = new NamedCommand<object>( "Test", DefaultAction.None );

            command.MonitorEvents();

            // act
            command.Id = expected;

            // assert
            command.Id.Should().Be( expected );
            command.ShouldRaisePropertyChangeFor( c => c.Id );
        }

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
    }
}