namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="ExpandableItem{T}"/> class.
    /// </summary>
    public class ExpandableItemTTest
    {
        [Fact( DisplayName = "item should expand and collapse" )]
        public void ShouldExpandAndCollapse()
        {
            var target = new ExpandableItem<string>( "test" );

            Assert.Equal( "test", target.Value );
            Assert.False( target.IsExpanded );
            Assert.PropertyChanged( target, "IsExpanded", () => target.IsExpanded = true );
            Assert.True( target.IsExpanded );

            target.Collapse.Execute( null );
            Assert.False( target.IsExpanded );

            target.Expand.Execute( null );
            Assert.True( target.IsExpanded );
        }

        [Fact( DisplayName = "equals should return expected result" )]
        public void EqualsShouldReturnExpectedResult()
        {
            var target = new ExpandableItem<string>( "test" );

            Assert.True( target == new ExpandableItem<string>( "test" ) );
            Assert.True( target != new ExpandableItem<string>( "test1" ) );
            Assert.True( target.Equals( new ExpandableItem<string>( "test" ) ) );
            Assert.False( target.Equals( new ExpandableItem<string>( "test1" ) ) );
            Assert.True( target.Equals( (object) new ExpandableItem<string>( "test" ) ) );
            Assert.False( target.Equals( (object) new ExpandableItem<string>( "test1" ) ) );

            target = new ExpandableItem<string>( "test", StringComparer.OrdinalIgnoreCase );

            Assert.True( target == new ExpandableItem<string>( "TEST" ) );
            Assert.True( target != new ExpandableItem<string>( "TEST1" ) );
            Assert.True( target.Equals( new ExpandableItem<string>( "TEST" ) ) );
            Assert.False( target.Equals( new ExpandableItem<string>( "TEST1" ) ) );
            Assert.True( target.Equals( (object) new ExpandableItem<string>( "TEST" ) ) );
            Assert.False( target.Equals( (object) new ExpandableItem<string>( "TEST1" ) ) );
        }
    }
}
