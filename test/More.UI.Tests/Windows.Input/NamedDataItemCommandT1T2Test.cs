namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="NamedDataItemCommand{TParameter,TItem}"/>.
    /// </summary>
    public class NamedDataItemCommandT1T2Test
    {
        [Theory]
        [MemberData( nameof( IdData ) )]
        public void new_named_data_item_command_should_set_id( Func<string, NamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = "42";

            // act
            var command = @new( expected );

            // assert
            command.Id.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_null_name( Func<string, NamedDataItemCommand<object, object>> newNamedItemCommand )
        {
            // arrange
            var name = default( string );

            // act
            Action @new = () => newNamedItemCommand( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_not_allow_empty_name( Func<string, NamedDataItemCommand<object, object>> newNamedItemCommand )
        {
            // arrange
            var name = "";

            // act
            Action @new = () => newNamedItemCommand( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        [Theory]
        [MemberData( nameof( NameData ) )]
        public void new_named_item_command_should_set_name( Func<string, NamedDataItemCommand<object, object>> @new )
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
        public void new_named_item_command_should_not_allow_null_execute_method( Func<Action<object, object>, NamedDataItemCommand<object, object>> newNamedItemCommand )
        {
            // arrange
            var executeMethod = default( Action<object, object> );

            // act
            Action @new = () => newNamedItemCommand( executeMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( executeMethod ) );
        }

        [Theory]
        [MemberData( nameof( CanExecuteMethodData ) )]
        public void new_named_item_command_should_not_allow_null_can_execute_method( Func<Func<object, object, bool>, NamedDataItemCommand<object, object>> newNamedItemCommand )
        {
            // arrange
            var canExecuteMethod = default( Func<object, object, bool> );

            // act
            Action @new = () => newNamedItemCommand( canExecuteMethod );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( canExecuteMethod ) );
        }

        [Theory]
        [MemberData( nameof( DataItemData ) )]
        public void new_named_item_command_should_set_data_item( Func<object, NamedDataItemCommand<object, object>> @new )
        {
            // arrange
            var expected = new object();

            // act
            var command = @new( expected );

            // assert
            command.Item.Should().Be( expected );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void name_should_not_allow_null_or_empty( string value )
        {
            // arrange
            var command = new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, null );

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
            var command = new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, null );

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
            var command = new NamedDataItemCommand<object, object>( "Default", DefaultAction.None, null );

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
            var command = new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, null );

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
            var command = new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, null );

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
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( id => new NamedDataItemCommand<object, object>( id, "Test", DefaultAction.None, null ) ) };
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( id => new NamedDataItemCommand<object, object>( id, "Test", DefaultAction.None, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( name => new NamedDataItemCommand<object, object>( name, DefaultAction.None, null ) ) };
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( name => new NamedDataItemCommand<object, object>( "1", name, DefaultAction.None, null ) ) };
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( name => new NamedDataItemCommand<object, object>( name, DefaultAction.None, ( i, p ) => true, null ) ) };
                yield return new object[] { new Func<string, NamedDataItemCommand<object, object>>( name => new NamedDataItemCommand<object, object>( "1", name, DefaultAction.None, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<object, object>, NamedDataItemCommand<object, object>>( execute => new NamedDataItemCommand<object, object>( "Test", execute, null ) ) };
                yield return new object[] { new Func<Action<object, object>, NamedDataItemCommand<object, object>>( execute => new NamedDataItemCommand<object, object>( "1", "Test", execute, null ) ) };
                yield return new object[] { new Func<Action<object, object>, NamedDataItemCommand<object, object>>( execute => new NamedDataItemCommand<object, object>( "Test", execute, ( i, p ) => true, null ) ) };
                yield return new object[] { new Func<Action<object, object>, NamedDataItemCommand<object, object>>( execute => new NamedDataItemCommand<object, object>( "1", "Test", execute, ( i, p ) => true, null ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<object, object, bool>, NamedDataItemCommand<object, object>>( canExecute => new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, canExecute, null ) ) };
                yield return new object[] { new Func<Func<object, object, bool>, NamedDataItemCommand<object, object>>( canExecute => new NamedDataItemCommand<object, object>( "1", "Test", DefaultAction.None, canExecute, null ) ) };
            }
        }

        public static IEnumerable<object[]> DataItemData
        {
            get
            {
                yield return new object[] { new Func<object, NamedDataItemCommand<object, object>>( item => new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, item ) ) };
                yield return new object[] { new Func<object, NamedDataItemCommand<object, object>>( item => new NamedDataItemCommand<object, object>( "1", "Test", DefaultAction.None, item ) ) };
                yield return new object[] { new Func<object, NamedDataItemCommand<object, object>>( item => new NamedDataItemCommand<object, object>( "Test", DefaultAction.None, ( i, p ) => true, item ) ) };
                yield return new object[] { new Func<object, NamedDataItemCommand<object, object>>( item => new NamedDataItemCommand<object, object>( "1", "Test", DefaultAction.None, ( i, p ) => true, item ) ) };
            }
        }
    }
}