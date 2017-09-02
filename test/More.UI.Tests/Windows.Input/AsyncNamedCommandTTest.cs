namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class AsyncNamedCommandTTest
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, AsyncNamedCommand<object>> @new )
        {
            // arrange
            var id = "42";

            // act
            var command = @new( id );

            // assert
            command.Id.Should().Be( id );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_null_name( Func<string, AsyncNamedCommand<object>> test )
        {
            // arrange
            var name = default( string );

            // act
            Action @new = () => test( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_empty_name( Func<string, AsyncNamedCommand<object>> test )
        {
            // arrange
            var name = "";

            // act
            Action @new = () => test( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_set_name( Func<string, AsyncNamedCommand<object>> @new )
        {
            // arrange
            var name = "Test";

            // act
            var command = @new( name );

            // assert
            command.Name.Should().Be( name );
        }

        [Theory]
        [MemberData( nameof( ExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Func<object, Task>, AsyncNamedCommand<object>> test )
        {
            // arrange
            var executeAsyncMethod = default( Func<object, Task> );

            // act
            Action @new = () => test( executeAsyncMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeAsyncMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<object, bool>, AsyncNamedCommand<object>> test )
        {
            // arrange
            var canExecuteMethod = default( Func<object, bool> );

            // act
            Action @new = () => test( canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void name_should_not_allow_null_or_empty( string value )
        {
            // arrange
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

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
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

            // act
            Action setDescription = () => command.Description = value;

            // assert
            setDescription.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void name_should_write_expected_value()
        {
            // arrange
            var name = "Test";
            var command = new AsyncNamedCommand<object>( "Default", p => Task.FromResult( 0 ) );

            command.MonitorEvents();

            // act
            command.Name = name;

            // assert
            command.Name.Should().Be( name );
            command.ShouldRaisePropertyChangeFor( c => c.Name );
        }

        [Fact]
        public void description_should_write_expected_value()
        {
            // arrange
            var description = "Test";
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

            command.MonitorEvents();

            // act
            command.Description = description;

            // assert
            command.Description.Should().Be( description );
            command.ShouldRaisePropertyChangeFor( c => c.Description );
        }

        [Fact]
        public void id_should_write_expected_value()
        {
            // arrange
            var id = "42";
            var command = new AsyncNamedCommand<object>( "Test", p => Task.FromResult( 0 ) );

            command.MonitorEvents();

            // act
            command.Id = id;

            // assert
            command.Id.Should().Be( id );
            command.ShouldRaisePropertyChangeFor( c => c.Id );
        }

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
    }
}