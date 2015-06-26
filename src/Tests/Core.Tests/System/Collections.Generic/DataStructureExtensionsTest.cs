namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DataStructureExtensions"/>.
    /// </summary>
    public class DataStructureExtensionsTest
    {
        [Fact( DisplayName = "adapt should not allow null stack" )]
        public void AdaptShouldNotAllowNullStack()
        {
            // arrange
            Stack<object> stack = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => stack.Adapt() );

            // assert
            Assert.Equal( "stack", ex.ParamName );
        }

        [Fact( DisplayName = "adapt should not allow null queue" )]
        public void AdaptShouldNotAllowNullQueue()
        {
            // arrange
            Queue<object> queue = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => queue.Adapt() );

            // assert
            Assert.Equal( "queue", ex.ParamName );
        }
    }
}
