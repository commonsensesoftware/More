namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="StackAdapter{T}"/>.
    /// </summary>
    public class StackAdapterTTest
    {
        [Fact( DisplayName = "peek should peek source" )]
        public void PeekShouldPeekSourceStack()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );

            // act
            target.Push( new object() );

            // assert
            Assert.Equal( source.Peek(), target.Peek() );
        }

        [Fact( DisplayName = "pop should pop source" )]
        public void PopShouldPopSourceStack()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );

            target.Push( new object() );

            var expected = source.Peek();

            // act
            var actual = target.Pop();

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "clear should clear source" )]
        public void ClearShouldClearSourceStack()
        {
            // arrange
            var source = new Stack<object>();
            var target = new StackAdapter<object>( source );

            target.Push( new object() );

            // act
            target.Clear();

            // assert
            Assert.Equal( 0, source.Count );
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "copy to should copy from source" )]
        public void CopyToShouldCopySourceStack()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            var source = new Stack<object>( expected );
            var target = new StackAdapter<object>( source );
            var actual = new object[3];

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected.Reverse() ) );
        }
    }
}
