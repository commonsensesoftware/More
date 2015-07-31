namespace System.Collections.Generic
{
    using More.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IEnumerableExtensions"/>.
    /// </summary>
    public class IEnumerableExtensionsTest
    {
        /// <summary>
        /// Provides a test object for Flatten.
        /// </summary>
        /// <typeparam name="T">The type of node value.</typeparam>
        private sealed class TreeNode<T>
        {
            internal TreeNode()
            {
                Children = new List<TreeNode<T>>();
            }

            internal TreeNode( T value )
                : this()
            {
                Value = value;
            }

            internal T Value
            {
                get;
                set;
            }

            internal IList<TreeNode<T>> Children
            {
                get;
                private set;
            }
        }

        [Fact( DisplayName = "for each should invoke action" )]
        public void ForEachShouldInvokeActionOnAllItems()
        {
            // arrange
            var count = 0;
            var target = new List<object>() { new object(), new object(), new object() };

            // act
            target.ForEach( i => ++count );

            // assert
            Assert.Equal( target.Count, count );
        }

        [Fact( DisplayName = "with should invoke action" )]
        public void WithShouldInvokeActionOnAllItems()
        {
            // arrange
            var count = 0;
            var target = new List<object>() { new object(), new object(), new object() };

            // act
            var actual = target.With( i => ++count );
            var cacheOfActual = actual.ToList();

            // assert
            Assert.NotSame( target, actual ); // new projection
            Assert.Equal( target.Count, cacheOfActual.Count );
            Assert.Equal( target.Count, count );
            Assert.True( target.SequenceEqual( cacheOfActual ) );
        }

        [Fact( DisplayName = "index of should return expected result" )]
        public void IndexOfShouldReturnCorrectMatchedIndex()
        {
            // arrange
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), item };
            var expected = 2;

            // act
            var actual = target.IndexOf( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "index of should return -1 when unmatched" )]
        public void IndexOfShouldReturnNegativeOneWhenUnmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), new object() };
            var expected = -1;

            // act
            var actual = target.IndexOf( item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "between should return expected sequence" )]
        public void BetweenShouldReturnCorrectSequence()
        {
            // arrange
            var target = new List<object>()
            {
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object()
            };

            // act
            var actual = target.Between( 1, 3 ).ToList();

            // assert
            Assert.Equal( 3, actual.Count );
            Assert.Equal( target[3], actual[0] );
            Assert.Equal( target[4], actual[1] );
            Assert.Equal( target[5], actual[2] );
        }

        [Fact( DisplayName = "page count should return expected result" )]
        public void PageCountShouldReturnCorrectValue()
        {
            // arrange
            var target = new List<object>()
            {
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object()
            };
            var expected = 3;

            // act
            var actual = target.PageCount( 3 );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "find index should return expected result" )]
        public void FindIndexShouldReturnCorrectValueWhenMatched()
        {
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), item };
            var expected = 2;
            var actual = target.FindIndex( i => i == item );
            
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "find index should return -1 when unmatched" )]
        public void FindIndexShouldReturnNegativeOneWhenUnmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), new object() };
            var expected = -1;

            // act
            var actual = target.FindIndex( i => i == item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "find index with offset should return expected result" )]
        public void FindIndexWithOffsetShouldReturnCorrectValueWhenMatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), item };
            var expected = 2;

            // act
            var actual = target.FindIndex( 1, i => i == item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "find index with offset should return -1 when unmatched" )]
        public void FindIndexWithOffsetShouldReturnNegativeOneWhenUnmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> target = new List<object>() { new object(), new object(), new object() };
            var expected = -1;

            // act
            var actual = target.FindIndex( 1, i => i == item );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "self and flatten should return expected sequence" )]
        public void SelfAndFlattenShouldReturnExpectedSequence()
        {
            // arrange
            var target = new Node<string>( "1" )
            {
                new Node<string>( "1.1" )
                {
                    new Node<string>( "1.1.1" )
                },
                new Node<string>( "1.2" )
            };
            var expected = new[] { "1", "1.1", "1.1.1", "1.2" };
            
            // act
            var actual = target.SelfAndFlatten().Select( n => n.Value );
            
            actual.ForEach( i => Debug.WriteLine( i ) );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "flatten should return expected sequence" )]
        public void FlattenShouldReturnExpectedSequence()
        {
            // arrange
            var target = new Node<string>( "1" )
            {
                new Node<string>( "1.1" )
                {
                    new Node<string>( "1.1.1" )
                },
                new Node<string>( "1.2" )
            };
            var expected = new[] { "1.1", "1.1.1", "1.2" };

            // act
            var actual = target.Flatten().Select( n => n.Value );

            actual.ForEach( i => Debug.WriteLine( i ) );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "flatten with selector should return expected sequence" )]
        public void FlattenWithSelectorShouldReturnExpectedSequence()
        {
            // arrange
            var nodes = new TreeNode<string>( "1" )
            {
                Children =
                {
                    new TreeNode<string>( "1.1" )
                    {
                        Children =
                        {
                            new TreeNode<string>( "1.1.1" )
                        }
                    },
                    new TreeNode<string>( "1.2" )
                }
            };
            var expected = new[] { "1", "1.1", "1.1.1", "1.2" };
            var target = new List<TreeNode<string>>();

            target.Add( nodes );
            
            // act
            var actual = target.Flatten( n => n.Children ).Select( n => n.Value );

            actual.ForEach( i => Debug.WriteLine( i ) );

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }
    }
}
