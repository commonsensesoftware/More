namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="StackDebugView{T}"/> class.
    /// </summary>
    public class StackDebugViewTTest
    {
        [Fact( DisplayName = "stack debug view should copy items" )]
        public void ItemsPropertyShouldReturnArrayOfStackItems()
        {
            // arrange
            var stack = new Stack<string>().Adapt();

            stack.Push( "test" );

            // act
            var target = new StackDebugView<string>( stack );

            // assert
            Assert.NotNull( target.Items );
            Assert.Equal( 1, target.Items.Length );
            Assert.Equal( stack.Peek(), target.Items[0] );
        }
    }
}
