namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class AsyncNamedDataItemCommandT1T2Test
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, AsyncNamedDataItemCommand<object, object>> @new )
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
        public void new_named_item_command_should_not_allow_null_name( Func<string, AsyncNamedDataItemCommand<object, object>> test )
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
        public void new_named_item_command_should_not_allow_empty_name( Func<string, AsyncNamedDataItemCommand<object, object>> test )
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
        public void new_named_item_command_should_set_name( Func<string, AsyncNamedDataItemCommand<object, object>> @new )
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
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Func<object, object, Task>, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            var executeAsyncMethod = default( Func<object, object, Task> );

            // act
            Action @new = () => test( executeAsyncMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeAsyncMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<object, object, bool>, AsyncNamedDataItemCommand<object, object>> test )
        {
            // arrange
            var canExecuteMethod = default( Func<object, object, bool> );

            // act
            Action @new = () => test( canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Theory]
        [MemberData( nameof( DataItemData ) )]
        public void new_named_item_command_should_set_data_item( Func<object, AsyncNamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var item = new object();

            // act
            var command = @new( item );

            // assert
            command.Item.Should().BeSameAs( item );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void name_should_not_allow_null_or_empty( string value )
        {
            // arrange
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

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
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

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
            var command = new AsyncNamedDataItemCommand<object, object>( "Default", ( i, p ) => Task.FromResult( 0 ), null );

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
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

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
            var command = new AsyncNamedDataItemCommand<object, object>( "Test", ( i, p ) => Task.FromResult( 0 ), null );

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
    }
}