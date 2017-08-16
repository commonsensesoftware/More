namespace System.Collections.Generic
{
    using FluentAssertions;
    using More.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class IEnumerableExtensionsTest
    {
        [Fact]
        public void for_each_should_invoke_action()
        {
            // arrange
            var count = 0;
            var sequence = new List<object>() { new object(), new object(), new object() };

            // act
            sequence.ForEach( i => ++count );

            // assert
            sequence.Count.Should().Be( count );
        }

        [Fact]
        public void with_should_invoke_action()
        {
            // arrange
            var count = 0;
            var sequence = new List<object>() { new object(), new object(), new object() };

            // act
            var result = sequence.With( i => ++count );

            // assert
            result.Should().BeEquivalentTo( sequence ).And.Should().NotBeSameAs( sequence );
        }

        [Fact]
        public void index_of_should_return_expected_result()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), item };

            // act
            var index = sequence.IndexOf( item );

            // assert
            index.Should().Be( 2 );
        }

        [Fact]
        public void index_of_should_return_X2D1_when_unmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), new object() };

            // act
            var index = sequence.IndexOf( item );

            // assert
            index.Should().Be( -1 );
        }

        [Fact]
        public void between_should_return_expected_sequence()
        {
            // arrange
            var sequence = new List<object>()
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
            var result = sequence.Between( 1, 3 ).ToArray();

            // assert
            result.Should().Equal( new[] { sequence[3], sequence[4], sequence[5] } );
        }

        [Fact]
        public void page_count_should_return_expected_result()
        {
            // arrange
            var sequence = new List<object>()
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
            var pageCount = sequence.PageCount( 3 );

            // assert
            pageCount.Should().Be( 3 );
        }

        [Fact]
        public void find_index_should_return_expected_result()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), item };

            // act
            var index = sequence.FindIndex( i => i == item );

            // assert
            index.Should().Be( 2 );
        }

        [Fact]
        public void find_index_should_return_X2D1_when_unmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), new object() };

            // act
            var index = sequence.FindIndex( i => i == item );

            // assert
            index.Should().Be( -1 );
        }

        [Fact]
        public void find_index_with_offset_should_return_expected_result()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), item };

            // act
            var index = sequence.FindIndex( 1, i => i == item );

            // assert
            index.Should().Be( 2 );
        }

        [Fact]
        public void find_index_with_offset_should_return_X2D1_when_unmatched()
        {
            // arrange
            var item = new object();
            IEnumerable<object> sequence = new List<object>() { new object(), new object(), new object() };

            // act
            var index = sequence.FindIndex( 1, i => i == item );

            // assert
            index.Should().Be( -1 );
        }

        [Fact]
        public void self_and_flatten_should_return_expected_sequence()
        {
            // arrange
            var sequence = new Node<string>( "1" )
            {
                new Node<string>( "1.1" )
                {
                    new Node<string>( "1.1.1" )
                },
                new Node<string>( "1.2" )
            };
            var expected = new[] { "1", "1.1", "1.1.1", "1.2" };

            // act
            var result = sequence.SelfAndFlatten().Select( n => n.Value );

            // assert
            result.Should().Equal( expected );
        }

        [Fact]
        public void flatten_should_return_expected_sequence()
        {
            // arrange
            var sequence = new Node<string>( "1" )
            {
                new Node<string>( "1.1" )
                {
                    new Node<string>( "1.1.1" )
                },
                new Node<string>( "1.2" )
            };
            var expected = new[] { "1.1", "1.1.1", "1.2" };

            // act
            var result = sequence.Flatten().Select( n => n.Value );

            // assert
            result.Should().Equal( expected );
        }

        [Fact]
        public void flatten_with_selector_should_return_expected_sequence()
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
            var sequence = new List<TreeNode<string>>();

            sequence.Add( nodes );

            // act
            var result = sequence.Flatten( n => n.Children ).Select( n => n.Value );

            // assert
            result.Should().Equal( expected );
        }

        sealed class TreeNode<T>
        {
            internal TreeNode( T value ) => Value = value;

            internal T Value { get; set; }

            internal IList<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
        }
    }
}