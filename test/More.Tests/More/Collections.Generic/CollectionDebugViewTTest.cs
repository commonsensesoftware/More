namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class CollectionDebugViewTTest
    {
        [Fact]
        public void new_collection_debug_view_set_items()
        {
            // arrange
            var expected = new List<object>() { new object() };
            var target = new CollectionDebugView<object>( expected );
            
            // act
            var actual = target.Items;

            // assert
            actual.Should().BeEquivalentTo( expected );
        }
    }
}