namespace More.Collections.Generic
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DictionaryDebugViewT1T2Test
    {
        [Fact]
        public void new_dictionary_debug_view_should_set_items()
        {
            // arrange
            var expected = new Dictionary<string, object>() { { "Test", new object() } };
            var target = new DictionaryDebugView<string, object>( expected );

            // act
            var actual = target.Items;

            // assert
            actual.Should().Equal( expected );
        }
    }
}
