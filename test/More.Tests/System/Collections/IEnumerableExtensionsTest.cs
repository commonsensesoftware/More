namespace System.Collections
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class IEnumerableExtensionsTest
    {
        [Fact]
        public void any_should_not_allow_null_sequence()
        {
            // arrange
            var sequence = default( IEnumerable );

            // act
            Action any = () => IEnumerableExtensions.Any( sequence );

            // assert
            any.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( sequence ) );
        }

        [Fact]
        public void any_should_return_true_for_nonX2Dempty_sequence()
        {
            // arrange
            IEnumerable sequence = new List<object>() { new object() };

            // act
            var result = sequence.Any();

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void any_should_return_false_for_empty_sequence()
        {
            // arrange
            IEnumerable sequence = new List<object>();

            // act
            var result = sequence.Any();

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void count_should_not_allow_null_sequence()
        {
            // arrange
            var sequence = default( IEnumerable );

            // act
            Action count = () => IEnumerableExtensions.Count( sequence );

            // assert
            count.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( sequence ) );
        }

        [Fact]
        public void count_should_return_number_of_elements_in_sequence()
        {
            // arrange
            IEnumerable sequence = new List<object>() { new object(), new object(), new object() };

            // act
            var count = sequence.Count();

            // assert
            count.Should().Be( 3 );
        }

        [Fact]
        public void index_should_not_allow_null_sequence()
        {
            // arrange
            var sequence = default( IEnumerable );

            // act
            Action indexOf = () => IEnumerableExtensions.IndexOf( sequence, default( object ) );

            // assert
            indexOf.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( sequence ) );
        }

        [Fact]
        public void index_of_should_not_allow_null_comparer()
        {
            // arrange
            var comparer = default( IEqualityComparer );

            // act
            Action indexOf = () => IEnumerableExtensions.IndexOf( new ArrayList(), new object(), comparer );

            // assert
            indexOf.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( comparer ) );
        }

        [Fact]
        public void index_of_should_return_expected_value()
        {
            // arrange
            var item = new object();
            IEnumerable sequence = new List<object>() { new object(), new object(), item };
            IEqualityComparer comparer = EqualityComparer<object>.Default;

            // act
            var index = sequence.IndexOf( item, comparer );

            // assert
            index.Should().Be( 2 );
        }

        [Fact]
        public void index_of_should_return_X2D1_if_not_found()
        {
            // arrange
            IEnumerable sequence = new List<object>() { new object() };
            IEqualityComparer comparer = EqualityComparer<object>.Default;

            // act
            var index = sequence.IndexOf( new object(), comparer );

            // assert
            index.Should().Be( -1 );
        }

        [Fact]
        public void element_at_should_not_allow_null_sequence()
        {
            // arrange
            var sequence = default( IEnumerable );

            // act
            Action elementAt = () => IEnumerableExtensions.ElementAt( sequence, 0 );

            // assert
            elementAt.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( sequence ) );
        }

        [Theory]
        [InlineData( -1 )]
        [InlineData( 1 )]
        public void element_at_should_not_allow_index_out_of_range( int index )
        {
            // arrange
            IEnumerable sequence = new List<object>();

            // act
            Action elementAt = () => sequence.ElementAt( index );

            // assert
            elementAt.ShouldThrow<ArgumentOutOfRangeException>().And.ParamName.Should().Be( nameof( index ) );
        }

        [Fact]
        public void element_at_should_return_expected_value_from_list()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new List<object>() { new object(), new object(), expected };

            // act
            var item = sequence.ElementAt( 2 );

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void element_at_should_return_expected_value_from_dictionary()
        {
            // arrange
            ICollection<KeyValuePair<int, object>> dictionary = new Dictionary<int, object>()
            {
                [0] = new object(),
                [1] = new object(),
                [2] = new object()
            };
            IEnumerable sequence = dictionary;
            var items = new KeyValuePair<int, object>[3];

            dictionary.CopyTo( items, 0 );

            var expected = items[2];

            // act
            var item = sequence.ElementAt( 2 );

            // assert
            item.Should().Be( expected );
        }

        [Theory]
        [InlineData( -1 )]
        [InlineData( 3 )]
        public void element_at_or_default_should_return_null_for_index_out_of_range( int index )
        {
            // arrange
            IEnumerable sequence = new[] { new object(), new object(), new object() };

            // act
            var item = sequence.ElementAtOrDefault( index );

            // assert
            item.Should().BeNull();
        }

        [Fact]
        public void first_should_return_first_element()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { expected, new object(), new object() };

            // act
            var item = sequence.First();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void first_should_throw_exception_when_sequence_is_empty()
        {
            // arrange
            IEnumerable sequence = new object[0];

            // act
            Action first = () => sequence.First();

            // assert
            first.ShouldThrow<InvalidOperationException>().And.Message.Should().Be( "Sequence contains no elements." );
        }

        [Theory]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( object ), typeof( string ), typeof( int ) }, typeof( object ) )]
        public void first_or_default_should_return_expected_result( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var item = sequence.FirstOrDefault();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void last_should_return_last_element()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { new object(), new object(), expected };

            // act
            var item = sequence.Last();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void last_should_throw_exception_when_sequence_is_empty()
        {
            // arrange
            IEnumerable sequence = new object[0];

            // act
            Action last = () => sequence.Last();

            // assert
            last.ShouldThrow<InvalidOperationException>().And.Message.Should().Be( "Sequence contains no elements." );
        }

        [Theory]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( string ), typeof( int ), typeof( object ) }, typeof( object ) )]
        public void last_or_default_should_return_expected_result( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var item = sequence.LastOrDefault();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void single_should_return_exactly_one_element()
        {
            // arrange
            var expected = new object();
            IEnumerable sequence = new[] { expected };

            // act
            var item = sequence.Single();

            // assert
            item.Should().Be( expected );
        }

        [Theory]
        [InlineData( new object[0], "Sequence contains no elements." )]
        [InlineData( new[] { 1, 2, 3 }, "Sequence contains more than one matching element." )]
        public void single_should_throw_exception_when_sequence_is_not_exactly_one_element( IEnumerable sequence, string expected )
        {
            // arrange

            // act
            Action single = () => sequence.Single();

            // assert
            single.ShouldThrow<InvalidOperationException>().And.Message.Should().Be( expected );
        }

        [Theory]
        [InlineData( new Type[0], null )]
        [InlineData( new[] { typeof( object ) }, typeof( object ) )]
        public void single_or_default_should_return_expected_result( IEnumerable sequence, object expected )
        {
            // arrange


            // act
            var item = sequence.SingleOrDefault();

            // assert
            item.Should().Be( expected );
        }

        [Fact]
        public void single_or_default_should_throw_exception_when_sequence_is_more_than_one_element()
        {
            // arrange
            var sequence = new[] { 1, 2, 3 };

            // act
            Action single = () => sequence.SingleOrDefault();

            // assert
            single.ShouldThrow<InvalidOperationException>().And.Message.Should().Be( "Sequence contains more than one matching element." );
        }

        [Fact]
        public void to_array_should_return_expected_result()
        {
            // arrange
            IEnumerable sequence = new[] { new object(), new object(), new object() };

            // act
            IEnumerable array = sequence.ToArray();

            // assert
            array.Should().Equal( sequence ).And.Should().NotBeSameAs( sequence );
        }

        [Fact]
        public void to_list_should_return_expected_result()
        {
            // arrange
            IEnumerable sequence = new[] { new object(), new object(), new object() };

            // act
            IEnumerable list = sequence.ToList();

            // assert
            list.Should().Equal( sequence ).And.Should().NotBeSameAs( sequence );
        }

        [Fact]
        public void reverse_should_return_expected_result()
        {
            // arrange
            var expected = new[] { new object(), new object(), new object() };
            IEnumerable sequence = new[] { expected[2], expected[1], expected[0] };

            // act
            var result = sequence.Reverse();

            // assert
            result.Should().Equal( expected ).And.Should().NotBeSameAs( sequence );
        }

        [Fact]
        public void skip_should_bypass_expected_elements()
        {
            // arrange
            var expected = new[] { new object(), new object() };
            IEnumerable sequence = new[] { new object(), new object(), new object(), expected[0], expected[1] };

            // act
            var result = sequence.Skip( 3 );

            // assert
            result.Should().Equal( expected );
        }

        [Fact]
        public void take_should_return_expected_elements()
        {
            // arrange
            var expected = new[] { new object(), new object() };
            IEnumerable sequence = new[] { expected[0], expected[1], new object(), new object(), new object() };

            // act
            var result = sequence.Take( 2 );

            // assert
            result.Should().Equal( expected );
        }

        [Theory]
        [InlineData( new object[0], new object[0], true )]
        [InlineData( new object[] { 1 }, new object[0], false )]
        [InlineData( new object[0], new object[] { 1 }, false )]
        [InlineData( new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, true )]
        public void sequence_equal_should_return_expected_result( IEnumerable first, IEnumerable second, bool expected )
        {
            // arrange


            // act
            var result = first.SequenceEqual( second );

            // assert
            result.Should().Be( expected );
        }
    }
}