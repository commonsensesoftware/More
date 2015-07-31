namespace More.ComponentModel
{
    using Moq;
    using Moq.Protected;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CompositeRule{TInput,TOutput}"/>.
    /// </summary>
    public class CompositeRuleT1T2Test
    {
        private sealed class MockCompositeRule : CompositeRule<string, bool>
        {
            public override bool Evaluate( string item )
            {
                using ( var iterator = NestedRules.GetEnumerator() )
                {
                    if ( !iterator.MoveNext() )
                        return false;

                    var result = iterator.Current.Evaluate( item );

                    while ( iterator.MoveNext() )
                        result &= iterator.Current.Evaluate( item );

                    return result;
                }
            }
        }

        [Fact( DisplayName = "new composition rule should not allow null rules" )]
        public void ConstructorShouldNotAcceptNullRules()
        {
            // arrange
            IEnumerable<IRule<object, object>> rules = null;
            var mock = new Mock<CompositeRule<object, object>>( rules );

            // act
            var mockEx = Assert.Throws<TargetInvocationException>( () => mock.Object );
            var ex = Assert.IsType<ArgumentNullException>( mockEx.InnerException );
            
            // assert
            Assert.Equal( "rules", ex.ParamName );
        }

        [Fact( DisplayName = "add should not allow null items" )]
        public void AddShouldNotAllowNullItems()
        {
            // arrange
            var target = new Mock<CompositeRule<object, object>>() { CallBase = true }.Object;
            IRule<object, object> item = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.Add( item ) );

            // assert
            Assert.Equal( "item", ex.ParamName );
        }

        [Fact( DisplayName = "add should add item to rule set" )]
        public void AddShouldInsertItemIntoCollection()
        {
            // arrange
            var target = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => string.IsNullOrEmpty( s ) );

            // act
            target.Add( expected );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.True( target.Contains( expected ) );
        }

        [Fact( DisplayName = "clear should empty rule set" )]
        public void ClearShouldEmptyCollection()
        {
            // arrange
            var target = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => string.IsNullOrEmpty( s ) );

            target.Add( expected );

            // act
            target.Clear();

            // assert
            Assert.Equal( 0, target.Count );
            Assert.False( target.Contains( expected ) );
        }

        [Fact( DisplayName = "remove should remove item from rule set" )]
        public void RemoveShouldClearItemFromCollection()
        {
            // arrange
            var target = new Mock<CompositeRule<string, bool>>() { CallBase = true }.Object;
            var expected = new Rule<string, bool>( s => string.IsNullOrEmpty( s ) );

            target.Add( expected );

            // act
            var removed = target.Remove( expected );

            // assert
            Assert.True( removed );
        }

        [Fact( DisplayName = "evaluate should execute rule set" )]
        public void ShouldEvaluateRuleSet()
        {
            // arrange
            var target = new MockCompositeRule();

            target.Add( new Rule<string, bool>( s => !string.IsNullOrEmpty( s ) ) );
            target.Add( new Rule<string, bool>( s => s.Length > 1 ) );
            target.Add( new Rule<string, bool>( s => s.Length < 25 ) );

            // act
            var actual = target.Evaluate( "Unit Test" );

            // assert
            Assert.True( actual );
        }
    }
}
