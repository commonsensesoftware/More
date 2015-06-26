namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CompositeRule{T}"/>.
    /// </summary>
    public class CompositeRuleTTest
    {
        [Fact( DisplayName = "new composite rule should not allow null rules" )]
        public void ConstructorShouldNotAcceptNullRules()
        {
            // arrange
            IEnumerable<IRule<int, int>> rules = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new CompositeRule<int>( rules ) );

            // assert
            Assert.Equal( "rules", ex.ParamName );
        }

        [Fact( DisplayName = "evaluate should invoke all rules" )]
        public void EvaluateShouldInvokeAllComposedRules()
        {
            // arrange
            var target = new CompositeRule<int>()
            {
                new Rule<int, int>( i => ++i ),
                new Rule<int, int>( i => ++i ),
                new Rule<int, int>( i => ++i )
            };
            var seed = 0;

            // act
            var actual = target.Evaluate( seed );

            // assert
            Assert.Equal( 3, actual );
        }
    }
}
