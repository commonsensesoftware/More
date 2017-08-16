namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class RuleT1T2Test
    {
        [Fact]
        public void new_rule_should_not_allow_null_callback()
        {
            // arrange
            var evaluate = default( Func<string, string> );

            // act
            Action @new = () => new Rule<string, string>( evaluate );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( evaluate ) );
        }

        [Fact]
        public void evaluate_should_invoke_callback()
        {
            // arrange
            var expected = "unit test";
            var rule = new Rule<string, string>( s => s );

            // act
            var result = rule.Evaluate( expected );

            // assert
            result.Should().Be( expected );
        }
    }
}