namespace More.Collections.Generic
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="DataStructureExtensions"/>.
    /// </summary>
    public class DataStructureExtensionsTest
    {
        [Fact( DisplayName = "adapt should wrap stack" )]
        public void AdaptShouldWrapStack()
        {
            // arrange
            var target = new Stack<object>();

            // act
            var actual = target.Adapt();

            // assert
            Assert.IsType<StackAdapter<object>>( actual );
        }

        [Fact( DisplayName = "adapt should wrap queue" )]
        public void AdaptShouldWrapQueue()
        {
            // arrange
            var target = new Queue<object>();
            
            // act
            var actual = target.Adapt();

            // assert
            Assert.IsType<QueueAdapter<object>>( actual );
        }
    }
}
