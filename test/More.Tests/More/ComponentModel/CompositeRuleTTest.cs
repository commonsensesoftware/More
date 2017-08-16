namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class CompositeRuleTTest
    {
        [Fact]
        public void new_composite_rule_should_not_allow_null_rules()
        {
            // arrange
            var rules = default( IEnumerable<IRule<int, int>> );

            // act
            Action @new = () => new CompositeRule<int>( rules );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( rules ) );
        }

        [Fact]
        public void evaluate_should_invoke_all_rules()
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
            var result = target.Evaluate( seed );

            // assert
            result.Should().Be( 3 );
        }
    }
}