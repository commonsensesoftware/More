namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class SelectableItemTTest
    {
        public static IEnumerable<object[]> ConstructorData
        {
            get
            {
                yield return new object[] { new Func<SelectableItem<string>>( () => new SelectableItem<string>( "test" ) ), new bool?( false ) };
                yield return new object[] { new Func<SelectableItem<string>>( () => new SelectableItem<string>( true, "test" ) ), new bool?( true ) };
                yield return new object[] { new Func<SelectableItem<string>>( () => new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase ) ), new bool?( false ) };
                yield return new object[] { new Func<SelectableItem<string>>( () => new SelectableItem<string>( true, "test", StringComparer.OrdinalIgnoreCase ) ), new bool?( true ) };
            }
        }

        [Theory]
        [MemberData( nameof( ConstructorData ) )]
        public void new_selectable_item_should_set_expected_properties( Func<SelectableItem<string>> @new, bool? selected )
        {
            // arrange

            // act
            var item = @new();

            // assert
            item.ShouldBeEquivalentTo( new { Value = "test", IsSelected = selected }, o => o.ExcludingMissingMembers() );
        }

        [Fact]
        public void item_should_not_be_selected_by_default()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );

            // act


            // assert
            item.IsSelected.Should().BeFalse();
        }

        [Fact]
        public void item_should_select_using_property()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );

            item.MonitorEvents();

            // act
            item.IsSelected = true;

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Selected ) );
        }

        [Fact]
        public void item_should_select_using_command()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );

            item.MonitorEvents();

            // act
            item.Select.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Selected ) );
        }

        [Fact]
        public void item_should_unselect_using_property()
        {
            // arrange
            var item = new SelectableItem<string>( true, "test" );

            item.MonitorEvents();

            // act
            item.IsSelected = false;

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Unselected ) );
        }

        [Fact]
        public void item_should_unselect_using_command()
        {
            // arrange
            var item = new SelectableItem<string>( true, "test" );

            item.MonitorEvents();

            // act
            item.Unselect.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Unselected ) );
        }

        [Fact]
        public void X3DX3D_should_equate_equal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "test" );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new SelectableItem<string>( "TEST" );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_not_equate_unequal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "abc" );

            // act
            var result = item == other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "test" );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new SelectableItem<string>( "TEST" );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_equate_unequal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "abc" );

            // act
            var result = item != other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "test" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new SelectableItem<string>( "TEST" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_items()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            var other = new SelectableItem<string>( "abc" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_equate_equal_objects()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            object other = new SelectableItem<string>( "test" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_objects()
        {
            // arrange
            var item = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            object other = new SelectableItem<string>( "TEST" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_objects()
        {
            // arrange
            var item = new SelectableItem<string>( "test" );
            object other = new SelectableItem<string>( "abc" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }
    }
}