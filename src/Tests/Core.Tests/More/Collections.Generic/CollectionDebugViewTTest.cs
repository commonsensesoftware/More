namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CollectionDebugView{T}"/>.
    /// </summary>
    public class CollectionDebugViewTTest
    {
        [Fact( DisplayName = "new collection debug view set items" )]
        public void ConstructorShouldSetItemsProperty()
        {
            // arrange
            var expected = new List<object>() { new object() };
            var target = new CollectionDebugView<object>( expected );
            
            // act
            var actual = target.Items;
            
            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }
    }
}
