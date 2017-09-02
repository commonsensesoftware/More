namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ExpandableItemTTest
    {
        [Fact]
        public void item_should_be_collapsed_by_default()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );

            // act


            // assert
            item.IsExpanded.Should().BeFalse();
        }

        [Fact]
        public void item_should_expand_using_property()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );

            item.MonitorEvents();

            // act
            item.IsExpanded = true;

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsExpanded );
            item.ShouldRaise( nameof( item.Expanded ) );
        }

        [Fact]
        public void item_should_expand_using_command()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );

            item.MonitorEvents();

            // act
            item.Expand.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsExpanded );
            item.ShouldRaise( nameof( item.Expanded ) );
        }

        [Fact]
        public void item_should_collapse_using_command()
        {
            // arrange
            var item = new ExpandableItem<string>( true, "test" );

            item.MonitorEvents();

            // act
            item.Collapse.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsExpanded );
            item.ShouldRaise( nameof( item.Collapsed ) );
        }

        [Fact]
        public void X3DX3D_should_equate_equal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "test" );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new ExpandableItem<string>( "TEST" );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_not_equate_unequal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "abc" );

            // act
            var result = item == other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "test" );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new ExpandableItem<string>( "TEST" );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_equate_unequal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "abc" );

            // act
            var result = item != other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "test" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            var other = new ExpandableItem<string>( "TEST" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_items()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            var other = new ExpandableItem<string>( "abc" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_equate_equal_objects()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            object other = new ExpandableItem<string>( "test" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_objects()
        {
            // arrange
            var item = new ExpandableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            object other = new ExpandableItem<string>( "TEST" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_objects()
        {
            // arrange
            var item = new ExpandableItem<string>( "test" );
            object other = new ExpandableItem<string>( "abc" );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }
    }
}