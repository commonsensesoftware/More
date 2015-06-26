namespace More.ComponentModel
{
    using Xunit;
    using System;

    /// <summary>
    /// Provides unit tests for the <see cref="Rule{T}"/> class.
    /// </summary>
    public class RuleTTest
    {
        [Fact( DisplayName = "new rule should not allow null callback" )]
        public void ConstructorShouldNotAllowNullCallback()
        {
            // arrange
            Action<string> evaluate = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new Rule<string>( evaluate ) );
            
            // assert
            Assert.Equal( "evaluate", ex.ParamName );
        }

        [Fact( DisplayName = "evaluate should invoke callback" )]
        public void EvaluateShouldExecuteCallback()
        {
            // arrange
            var executed = false;
            var target = new Rule<string>( s => executed = ( s == "unit test" ) );
            
            // act
            target.Evaluate( "unit test" );
            
            // assert
            Assert.True( executed );
        }
    }
}
