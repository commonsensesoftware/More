namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using More.Collections.Generic;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;
    using static Moq.Times;

    public class HierarchicalItemTTest
    {
        public static IEnumerable<object[]> ConstructorData
        {
            get
            {
                var command = new Command<string>( DefaultAction.None );
                yield return new object[] { new Func<HierarchicalItem<string>>( () => new HierarchicalItem<string>( "test", command ) ), default( bool? ) };
                yield return new object[] { new Func<HierarchicalItem<string>>( () => new HierarchicalItem<string>( "test", command, EqualityComparer<string>.Default ) ), default( bool? ) };
                yield return new object[] { new Func<HierarchicalItem<string>>( () => new HierarchicalItem<string>( "test", true, command ) ), new bool?( true ) };
                yield return new object[] { new Func<HierarchicalItem<string>>( () => new HierarchicalItem<string>( "test", true, command, EqualityComparer<string>.Default ) ), new bool?( true ) };
            }
        }

        [Theory]
        [MemberData( nameof( ConstructorData ) )]
        public void new_hierarchical_item_should_set_expected_properties( Func<HierarchicalItem<string>> @new, bool? selected )
        {
            // arrange

            // act
            var item = @new();

            // assert
            item.Value.Should().Be( "test" );
            item.IsSelected.Should().Be( selected );
            item.Click.Should().BeOfType<CommandInterceptor<object>>();
        }

        [Fact]
        public void perform_click_should_execute_command()
        {
            // arrange
            var click = new Mock<Action<string>>();
            var command = new Command<string>( click.Object );
            var item = new HierarchicalItem<string>( "test", command );

            click.Setup( f => f( It.IsAny<string>() ) );
            item.MonitorEvents();

            // act
            item.PerformClick();

            // assert
            click.Verify( f => f( null ), Once() );
            item.ShouldRaise( nameof( item.Clicked ) );
        }

        [Fact]
        public void perform_click_should_execute_command_with_parameter()
        {
            // arrange
            var click = new Mock<Action<string>>();
            var command = new Command<string>( click.Object );
            var item = new HierarchicalItem<string>( "test", command );

            click.Setup( f => f( It.IsAny<string>() ) );
            item.MonitorEvents();

            // act
            item.PerformClick( "param" );

            // assert
            click.Verify( f => f( "param" ), Once() );
            item.ShouldRaise( nameof( item.Clicked ) );
        }

        [Fact]
        public void item_selection_state_should_be_indeterminate_by_default()
        {
            // arrange
            var item = new HierarchicalItem<string>( "test", new Command<string>( DefaultAction.None ) );

            // act


            // assert
            item.IsSelected.Should().BeNull();
        }

        [Fact]
        public void item_should_be_selected_using_property()
        {
            // arrange
            var item = new HierarchicalItem<string>( "test", new Command<string>( DefaultAction.None ) );

            item.MonitorEvents();
            ( (INotifyPropertyChanged) item ).MonitorEvents<INotifyPropertyChanged>();

            // act
            item.IsSelected = true;

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Selected ) );
        }

        [Fact]
        public void item_should_be_selected_using_command()
        {
            // arrange
            var item = new HierarchicalItem<string>( "test", new Command<string>( DefaultAction.None ) );

            item.MonitorEvents();
            ( (INotifyPropertyChanged) item ).MonitorEvents<INotifyPropertyChanged>();

            // act
            item.Select.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Selected ) );
        }

        [Fact]
        public void item_should_be_unselected_using_property()
        {
            // arrange
            var item = new HierarchicalItem<string>( "test", true, new Command<string>( DefaultAction.None ) );

            item.MonitorEvents();
            ( (INotifyPropertyChanged) item ).MonitorEvents<INotifyPropertyChanged>();

            // act
            item.IsSelected = false;

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Unselected ) );
        }

        [Fact]
        public void item_should_be_unselected_using_command()
        {
            // arrange
            var item = new HierarchicalItem<string>( "test", true, new Command<string>( DefaultAction.None ) );

            item.MonitorEvents();
            ( (INotifyPropertyChanged) item ).MonitorEvents<INotifyPropertyChanged>();

            // act
            item.Unselect.Execute( null );

            // assert
            item.ShouldRaisePropertyChangeFor( i => i.IsSelected );
            item.ShouldRaise( nameof( item.Unselected ) );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 1 )]
        [InlineData( 2 )]
        [InlineData( 3 )]
        public void item_depth_should_return_correct_level( int depth )
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );

            // act
            for ( var i = 0; i < depth; i++ )
            {
                var value = ( i + 1 ).ToString();
                var newItem = new HierarchicalItem<string>( value, command );

                item.Add( newItem );
                item = newItem;
            }

            // assert
            item.Depth.Should().Be( depth );
        }

        [Theory]
        [InlineData( 0 )]
        [InlineData( 1 )]
        [InlineData( 2 )]
        [InlineData( 3 )]
        public void parent_should_return_correct_object( int depth )
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var parent = default( HierarchicalItem<string> );

            // act
            for ( var i = 0; i < depth; i++ )
            {
                var value = ( i + 1 ).ToString();
                var newItem = new HierarchicalItem<string>( value, command );

                item.Add( newItem );
                parent = newItem.Parent;
                item = newItem;
            }

            // assert
            item.Parent.Should().BeSameAs( parent );
        }

        [Fact]
        public void items_with_equal_values_at_different_depths_should_yield_different_hash_codes()
        {
            // arrange
            var comparer = new DynamicComparer<Tuple<int, string>>( tuple => tuple.Item1.GetHashCode() );
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 1, "0" ), command, comparer )
            {
                new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 1, "1" ), command, comparer )
            };
            var other = new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 1, "2" ), command, comparer );

            // act
            item[0].Add( other );

            // assert
            item.Depth.Should().NotBe( other.Depth );
            item.GetHashCode().Should().NotBe( other.GetHashCode() );
        }

        [Fact]
        public void items_with_different_values_and_depths_should_yield_different_hash_codes()
        {
            // arrange
            var comparer = new DynamicComparer<Tuple<int, string>>( tuple => tuple.Item1.GetHashCode() );
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 2, "0" ), command, comparer )
            {
                new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 1, "1" ), command, comparer )
            };
            var other = new HierarchicalItem<Tuple<int, string>>( Tuple.Create( 0, "2" ), command, comparer );

            // act
            item[0].Add( other );

            // assert
            item.Depth.Should().NotBe( other.Depth );
            item.GetHashCode().Should().NotBe( other.GetHashCode() );
        }

        [Fact]
        public void X3DX3D_should_equate_equal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var comparer = StringComparer.OrdinalIgnoreCase;
            var item = new HierarchicalItem<string>( "test", command, comparer );
            var other = new HierarchicalItem<string>( "TEST", command, comparer );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_not_equate_unequal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "abc", command );

            // act
            var result = item == other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void X3DX3D_should_not_equate_equal_values_for_items_at_different_depths()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            item.Add( other );

            // act
            var result = item == other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var comparer = StringComparer.OrdinalIgnoreCase;
            var item = new HierarchicalItem<string>( "test", command, comparer );
            var other = new HierarchicalItem<string>( "TEST", command, comparer );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_equate_unequal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "abc", command );

            // act
            var result = item != other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ne_should_not_equate_equal_values_for_items_at_different_depths()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            item.Add( other );

            // act
            var result = item != other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var comparer = StringComparer.OrdinalIgnoreCase;
            var item = new HierarchicalItem<string>( "test", command, comparer );
            var other = new HierarchicalItem<string>( "TEST", command, comparer );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_items()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "abc", command );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_not_equate_equal_values_for_items_at_different_depths()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var other = new HierarchicalItem<string>( "test", command );

            item.Add( other );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_equate_equal_objects()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            object other = new HierarchicalItem<string>( "test", command );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_objects()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var comparer = StringComparer.OrdinalIgnoreCase;
            var item = new HierarchicalItem<string>( "test", command, comparer );
            object other = new HierarchicalItem<string>( "TEST", command, comparer );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_objects()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            object other = new HierarchicalItem<string>( "abc", command );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_not_equate_equal_values_for_objects_at_different_depths()
        {
            // arrange
            var command = new Command<string>( DefaultAction.None );
            var item = new HierarchicalItem<string>( "test", command );
            var child = new HierarchicalItem<string>( "test", command );
            object other = child;

            item.Add( child );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }
    }
}