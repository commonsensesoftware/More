namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DictionaryDebugView{TKey,TValue}"/>.
    /// </summary>
    public class DictionaryDebugViewT1T2Test
    {
        [Fact( DisplayName = "new dictionary debug view should set items" )]
        public void ConstructorShouldSetItemsProperty()
        {
            // arrange
            var expected = new Dictionary<string, object>() { { "Test", new object() } };
            var target = new DictionaryDebugView<string, object>( expected );

            // act
            var actual = target.Items;

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }
    }
}
