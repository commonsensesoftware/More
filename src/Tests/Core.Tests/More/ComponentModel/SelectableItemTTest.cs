namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="SelectableItem{T}"/> class.
    /// </summary>
    public class SelectableItemTTest
    {
        [Fact( DisplayName = "new selectable item should set expected properties" )]
        public void ConstructorShouldSetExpectedProperties()
        {
            var target = new SelectableItem<string>( "test" );

            Assert.Equal( "test", target.Value );
            Assert.NotNull( target.IsSelected );
            Assert.Equal( false, target.IsSelected );

            target = new SelectableItem<string>( true, "test" );
            Assert.Equal( "test", target.Value );
            Assert.Equal( true, target.IsSelected );

            target = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );
            Assert.Equal( "test", target.Value );
            Assert.NotNull( target.IsSelected );
            Assert.Equal( false, target.IsSelected );

            target = new SelectableItem<string>( true, "test", StringComparer.OrdinalIgnoreCase );
            Assert.Equal( "test", target.Value );
            Assert.Equal( true, target.IsSelected );
        }

        [Fact( DisplayName = "item should select and unselect" )]
        public void ShouldSelectedAndUnselect()
        {
            var target = new SelectableItem<string>( "test" );

            Assert.Equal( "test", target.Value );
            Assert.NotNull( target.IsSelected );
            Assert.PropertyChanged( target, "IsSelected", () => target.IsSelected = true );
            Assert.Equal( true, target.IsSelected );

            target.Unselect.Execute( null );
            Assert.Equal( false, target.IsSelected );

            target.Select.Execute( null );
            Assert.Equal( true, target.IsSelected );
        }

        [Fact( DisplayName = "equals should return expected result" )]
        public void EqualsShouldReturnExpectedResult()
        {
            var target = new SelectableItem<string>( "test" );

            Assert.True( target == new SelectableItem<string>( "test" ) );
            Assert.True( target != new SelectableItem<string>( "test1" ) );
            Assert.True( target.Equals( new SelectableItem<string>( "test" ) ) );
            Assert.False( target.Equals( new SelectableItem<string>( "test1" ) ) );
            Assert.True( target.Equals( (object) new SelectableItem<string>( "test" ) ) );
            Assert.False( target.Equals( (object) new SelectableItem<string>( "test1" ) ) );

            target = new SelectableItem<string>( "test", StringComparer.OrdinalIgnoreCase );

            Assert.True( target == new SelectableItem<string>( "TEST" ) );
            Assert.True( target != new SelectableItem<string>( "TEST1" ) );
            Assert.True( target.Equals( new SelectableItem<string>( "TEST" ) ) );
            Assert.False( target.Equals( new SelectableItem<string>( "TEST1" ) ) );
            Assert.True( target.Equals( (object) new SelectableItem<string>( "TEST" ) ) );
            Assert.False( target.Equals( (object) new SelectableItem<string>( "TEST1" ) ) );
        }
    }
}
