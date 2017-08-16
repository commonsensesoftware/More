namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;
    using static System.String;

    public class CompositeRuleT1T2Test
    {
        [Fact]
        public void new_composition_rule_should_not_allow_null_rules()
        {
            // arrange
            var rules = default( IEnumerable<IRule<object, object>> );
            var mock = new Mock<CompositeRule<object, object>>( rules );

            // act
            Action @new = () => Console.WriteLine( mock.Object );

            // assert
            @new.ShouldThrow<TargetInvocationException>().And
                .InnerException.As<ArgumentNullException>().ParamName.Should().Be( nameof( rules ) );
        }

        [Fact]
        public void add_should_not_allow_null_items()
        {
            // arrange
            var rule = new Mock<CompositeRule<object, object>>() { CallBase = true }.Object;
            var item = default( IRule<object, object> );

            // act
            Action add = () => rule.Add( item );

            // assert
            add.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( item ) );
        }

        [Fact]
        public void add_should_add_item_to_rule_set()
        {
            // arrange
            var rule = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => IsNullOrEmpty( s ) );

            // act
            rule.Add( expected );

            // assert
            rule.Should().Equal( new[] { expected } );
        }

        [Fact]
        public void clear_should_empty_rule_set()
        {
            // arrange
            var rule = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => IsNullOrEmpty( s ) );

            rule.Add( expected );

            // act
            rule.Clear();

            // assert
            rule.Should().BeEmpty();
        }

        [Fact]
        public void remove_should_remove_item_from_rule_set()
        {
            // arrange
            var rule = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => IsNullOrEmpty( s ) );

            rule.Add( expected );

            // act
            var removed = rule.Remove( expected );

            // assert
            removed.Should().BeTrue();
        }

        [Fact]
        public void evaluate_should_execute_rule_set()
        {
            // arrange
            var rule = new MockCompositeRule
            {
                new Rule<string, bool>( s => !IsNullOrEmpty( s ) ),
                new Rule<string, bool>( s => s.Length > 1 ),
                new Rule<string, bool>( s => s.Length < 25 )
            };

            // act
            var result = rule.Evaluate( "Unit Test" );

            // assert
            result.Should().BeTrue();
        }

        sealed class MockCompositeRule : CompositeRule<string, bool>
        {
            public override bool Evaluate( string item )
            {
                using ( var iterator = NestedRules.GetEnumerator() )
                {
                    if ( !iterator.MoveNext() )
                    {
                        return false;
                    }

                    var result = iterator.Current.Evaluate( item );

                    while ( iterator.MoveNext() )
                    {
                        result &= iterator.Current.Evaluate( item );
                    }

                    return result;
                }
            }
        }
    }
}