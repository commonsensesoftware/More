namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="Rule{TInput,TOutput}"/> class.
    /// </summary>
    public class RuleT1T2Test
    {
        [Fact( DisplayName = "new rule should not allow null callback" )]
        public void ConstructorShouldNotAllowNullCallback()
        {
            // arrange
            Func<string, string> evaluate = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new Rule<string, string>( evaluate ) );
            
            // assert
            Assert.Equal( "evaluate", ex.ParamName );
        }

        [Fact( DisplayName = "evaluate should invoke callback" )]
        public void EvaluateShouldExecuteCallback()
        {
            // arrange
            var expected = "unit test";
            var target = new Rule<string, string>( s => s );
            
            // act
            var actual = target.Evaluate( expected );
            
            // assert
            Assert.Equal( expected, actual );
        }
    }
}
