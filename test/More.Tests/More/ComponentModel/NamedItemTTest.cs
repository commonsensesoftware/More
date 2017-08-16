namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static System.String;

    public class NamedItemTTest
    {
        public static IEnumerable<object[]> NullNameData
        {
            get
            {
                yield return new object[] { new Action<string>( name => new NamedItem<object>( name, Empty ) ), null };
                yield return new object[] { new Action<string>( name => new NamedItem<object>( name, Empty, null ) ), null };
                yield return new object[] { new Action<string>( name => new NamedItem<object>( name, Empty ) ), Empty };
                yield return new object[] { new Action<string>( name => new NamedItem<object>( name, Empty, null ) ), Empty };
            }
        }

        [Theory]
        [MemberData( nameof( NullNameData ) )]
        public void new_named_item_should_not_allow_null_or_empty_name( Action<string> newNamedItems, string name )
        {
            // arrange

            // act
            Action @new = () => newNamedItems( name );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( name ) );
        }

        public static IEnumerable<object[]> NullDescriptionData
        {
            get
            {
                yield return new object[] { new Action<string>( description => new NamedItem<object>( "Test", description ) ) };
                yield return new object[] { new Action<string>( description => new NamedItem<object>( "Test", description, null ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( NullDescriptionData ) )]
        public void new_named_item_should_not_allow_null_description( Action<string> newNamedItems )
        {
            // arrange
            var description = default( string );

            // act
            Action @new = () => newNamedItems( description );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( description ) );
        }

        [Fact]
        public void new_named_item_should_set_name_property()
        {
            // arrange
            var item = new NamedItem<object>( "Actual", Empty );

            item.MonitorEvents();

            // act
            item.Name = "Expected";

            // assert
            item.Name.Should().Be( "Expected" );
            item.ShouldRaisePropertyChangeFor( i => i.Name );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void name_should_not_allow_null_or_empty_value( string value )
        {
            // arrange
            var item = new NamedItem<object>( "Test", Empty );

            // act
            Action setName = () => item.Name = value;

            // assert
            setName.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void new_named_item_should_set_description_property()
        {
            // arrange
            var item = new NamedItem<object>( "Test", "Actual" );

            item.MonitorEvents();

            // act
            item.Description = "Expected";

            // assert
            item.Description.Should().Be( "Expected" );
            item.ShouldRaisePropertyChangeFor( i => i.Description );
        }

        [Fact]
        public void description_should_not_allow_null_value()
        {
            // arrange
            var item = new NamedItem<object>( "Test", Empty );
            var value = default( string );

            // act
            Action setDescription = () => item.Description = value;

            // assert
            setDescription.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( value ) );
        }

        [Fact]
        public void value_property_should_write_expected_value()
        {
            // arrange
            var item = new NamedItem<object>( "Test", Empty, null );
            var expected = new object();

            item.MonitorEvents();

            // act
            item.Value = expected;

            // assert
            item.Value.Should().Be( expected );
            item.ShouldRaisePropertyChangeFor( i => i.Value );
        }
    }
}