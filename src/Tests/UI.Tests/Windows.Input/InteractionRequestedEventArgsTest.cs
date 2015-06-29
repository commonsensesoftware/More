namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="InteractionRequestedEventArgs"/>.
    /// </summary>
    public class InteractionRequestedEventArgsTest
    {
        [Fact( DisplayName = "new interaction requested event args should not allow null interaction" )]
        public void ConstructorShouldNotAllowNullInteraction()
        {
            // arrange
            Interaction interaction = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new InteractionRequestedEventArgs( interaction ) );

            // assert
            Assert.Equal( "interaction", ex.ParamName );
        }

        [Fact( DisplayName = "new interaction requested event args should set interaction" )]
        public void ConstructorShouldSetInteraction()
        {
            // arrange
            var expected = new Interaction();
            var args = new InteractionRequestedEventArgs( expected );

            // act
            var actual = args.Interaction;

            // assert
            Assert.Same( expected, actual );
        }
    }
}
