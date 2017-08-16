namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using Xunit;

    public class RuleTTest
    {
        [Fact]
        public void new_rule_should_not_allow_null_callback()
        {
            // arrange
            var evaluate = default( Action<string> );

            // act
            Action @new = () => new Rule<string>( evaluate );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( evaluate ) );
        }

        [Fact]
        public void evaluate_should_invoke_callback()
        {
            // arrange
            var evaluate = new Mock<Action<string>>();
            var rule = new Rule<string>( evaluate.Object );

            evaluate.Setup( f => f( It.IsAny<string>() ) );

            // act
            rule.Evaluate( "unit test" );

            // assert
            evaluate.Verify( f => f( "unit test" ), Times.Once() );
        }
    }
}