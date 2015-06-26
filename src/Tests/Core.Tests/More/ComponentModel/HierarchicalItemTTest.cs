namespace More.ComponentModel
{
    using More.Collections.Generic;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="HierarchicalItem{T}"/> class.
    /// </summary>
    public class HierarchicalItemTTest
    {
        [Fact( DisplayName = "new hierarchical item should set expected properties" )]
        public void ConstructorShouldSetExpectedProperties()
        {
            var command = new Command<string>( Console.WriteLine );
            var target = new HierarchicalItem<string>( "test", command );

            Assert.Equal( "test", target.Value );
            Assert.Null( target.IsSelected );
            Assert.IsType<CommandInterceptor<object>>( target.Click );

            target = new HierarchicalItem<string>( "test", command, EqualityComparer<string>.Default );
            Assert.Equal( "test", target.Value );
            Assert.Null( target.IsSelected );
            Assert.IsType<CommandInterceptor<object>>( target.Click );

            target = new HierarchicalItem<string>( "test", true, command );
            Assert.Equal( "test", target.Value );
            Assert.Equal( true, target.IsSelected );
            Assert.IsType<CommandInterceptor<object>>( target.Click );

            target = new HierarchicalItem<string>( "test", true, command, EqualityComparer<string>.Default );
            Assert.Equal( "test", target.Value );
            Assert.Equal( true, target.IsSelected );
            Assert.IsType<CommandInterceptor<object>>( target.Click );
        }

        [Fact( DisplayName = "perform click should execute command" )]
        public void PerformClickShouldExecuteCommand()
        {
            var clicked = false;
            var command = new Command<string>(
                p =>
                {
                    Assert.Null( p );
                    clicked = true;
                } );
            var target = new HierarchicalItem<string>( "test", command );

            target.PerformClick();
            Assert.True( clicked );

            clicked = false;
            command = new Command<string>(
                p =>
                {
                    Assert.Equal( "test2", p );
                    clicked = true;
                } );
            target = new HierarchicalItem<string>( "test", command );
            target.PerformClick( "test2" );
            Assert.True( clicked );
        }

        [Fact( DisplayName = "item should select and unselect" )]
        public void ShouldSelectedAndUnselect()
        {
            var target = new HierarchicalItem<string>( "test", new Command<string>( Console.WriteLine ) );

            Assert.Equal( "test", target.Value );
            Assert.Null( target.IsSelected );
            Assert.PropertyChanged( target, "IsSelected", () => target.IsSelected = true );
            Assert.True( target.IsSelected.Value );

            target.Unselect.Execute( null );
            Assert.False( target.IsSelected.Value );

            target.Select.Execute( null );
            Assert.True( target.IsSelected.Value );
        }

        [Fact( DisplayName = "equals should return expected result" )]
        public void EqualsShouldReturnExpectedResult()
        {
            var command = new Command<string>( Console.WriteLine );
            var target = new HierarchicalItem<string>( "test", command );
            var child = new HierarchicalItem<string>( "test", command );

            target.Add( child );

            Assert.True( target == new HierarchicalItem<string>( "test", command ) );
            Assert.True( target != new HierarchicalItem<string>( "test1", command ) );
            Assert.True( target.Equals( new HierarchicalItem<string>( "test", command ) ) );
            Assert.False( target.Equals( new HierarchicalItem<string>( "test1", command ) ) );
            Assert.True( target.Equals( (object) new HierarchicalItem<string>( "test", command ) ) );
            Assert.False( target.Equals( (object) new HierarchicalItem<string>( "test1", command ) ) );

            Assert.False( child == target );
            Assert.True( child != target );
            Assert.False( child.Equals( target ) );
            Assert.False( child.Equals( (object) target ) );

            var comparer = StringComparer.OrdinalIgnoreCase;
            target = new HierarchicalItem<string>( "test", command, comparer );

            Assert.True( target == new HierarchicalItem<string>( "TEST", command, comparer ) );
            Assert.True( target != new HierarchicalItem<string>( "TEST1", command, comparer ) );
            Assert.True( target.Equals( new HierarchicalItem<string>( "TEST", command, comparer ) ) );
            Assert.False( target.Equals( new HierarchicalItem<string>( "TEST1", command, comparer ) ) );
            Assert.True( target.Equals( (object) new HierarchicalItem<string>( "TEST", command, comparer ) ) );
            Assert.False( target.Equals( (object) new HierarchicalItem<string>( "TEST1", command, comparer ) ) );
        }

        [Fact( DisplayName = "item depth should return correct level" )]
        public void DepthPropertyShouldReturnCorrectLevel()
        {
            var command = new Command<string>( Console.WriteLine );
            var target = new HierarchicalItem<string>( "test", command );
            Assert.Equal( 0, target.Depth );
            target.Add( new HierarchicalItem<string>( "1", command ) );
            Assert.Equal( 1, target[0].Depth );
            target[0].Add( new HierarchicalItem<string>( "2", command ) );
            Assert.Equal( 2, target[0][0].Depth );
            target[0][0].Add( new HierarchicalItem<string>( "3", command ) );
            Assert.Equal( 3, target[0][0][0].Depth );
        }

        [Fact( DisplayName = "parent should return correct object" )]
        public void ParentPropertyShouldReturnCorrectObject()
        {
            var command = new Command<string>( Console.WriteLine );
            var target = new HierarchicalItem<string>( "test", command );
            Assert.Null( target.Parent );
            target.Add( new HierarchicalItem<string>( "1", command ) );
            Assert.Equal( target, target[0].Parent );
            target[0].Add( new HierarchicalItem<string>( "2", command ) );
            Assert.Equal( target[0], target[0][0].Parent );
            target[0][0].Add( new HierarchicalItem<string>( "3", command ) );
            Assert.Equal( target[0][0], target[0][0][0].Parent );
        }

        [Fact( DisplayName = "items with equal values at different depths should yield different hash codes" )]
        public void TwoItemsWithTheSameValueAtDifferentDepthsShouldYieldDifferentHashCodes()
        {
            var comparer = new DynamicComparer<Tuple<int, string>>( tuple => tuple.Item1.GetHashCode() );
            var command = new Command<string>( Console.WriteLine );
            var item1 = new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 1, "0" ), command, comparer );
            item1.Add( new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 1, "1" ), command, comparer ) );
            var item2 = new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 1, "2" ), command, comparer );
            item1[0].Add( item2 );

            Assert.Equal( item1.Value.Item1, item2.Value.Item1 );
            Assert.NotEqual( item1.Depth, item2.Depth );
            Assert.NotEqual( item1.GetHashCode(), item2.GetHashCode() );
        }

        [Fact( DisplayName = "items with different values and depths should yield different hash codes" )]
        public void WhenValueAndDepthOfTwoItemsAreUnequalTheyShouldYieldDifferentHashCodes()
        {
            var comparer = new DynamicComparer<Tuple<int, string>>( tuple => tuple.Item1.GetHashCode() );
            var command = new Command<string>( Console.WriteLine );
            var item1 = new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 2, "0" ), command, comparer );
            item1.Add( new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 1, "1" ), command, comparer ) );
            var item2 = new HierarchicalItem<Tuple<int, string>>( new Tuple<int, string>( 0, "2" ), command, comparer );
            item1[0].Add( item2 );

            Assert.NotEqual( item1.Depth, item2.Depth );
            Assert.NotEqual( item1.GetHashCode(), item2.GetHashCode() );
        }
    }
}
