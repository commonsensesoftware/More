namespace System.Collections.Generic
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public partial class IDictionaryExtensionsTest
    {
        [Fact]
        public void as_readX2Donly_should_return_self_when_readX2Donly()
        {
            // arrange
            var mock = new Mock<IDictionary<string, object>>();

            mock.SetupGet( d => d.IsReadOnly ).Returns( true );
            mock.As<IReadOnlyDictionary<string, object>>();

            var dictionary = mock.Object;

            // act
            var result = dictionary.AsReadOnly();

            // assert
            result.Should().BeSameAs( dictionary );
        }

        [Fact]
        public void get_or_add_should_return_existing_item()
        {
            // arrange
            var expected = new object();
            var dictionary = new Dictionary<string, object>() { { "Test", expected } };

            // act
            var value = dictionary.GetOrAdd( "Test", () => new object() );

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void get_or_add_should_insert_and_return_new_item()
        {
            // arrange
            var expected = new object();
            var dictionary = new Dictionary<string, object>();

            // act
            var value = dictionary.GetOrAdd( "Test", () => expected );

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void skip_should_trim_dictionary()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key3"] = 3,
                ["Key4"] = 4,
                ["Key5"] = 5
            };

            // act
            var result = dictionary.Skip( "Key2", "Key4" );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key1", "Key3", "Key5" } );
        }

        [Fact]
        public void skip_should_trim_dictionary_with_comparer()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key3"] = 3,
                ["Key4"] = 4,
                ["Key5"] = 5
            };

            // act
            var result = dictionary.Skip( StringComparer.Ordinal, "Key2", "Key4" );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key1", "Key3", "Key5" } );
        }

        [Fact]
        public void take_should_trim_dictionary()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key3"] = 3,
                ["Key4"] = 4,
                ["Key5"] = 5
            };

            // act
            var result = dictionary.Take( "Key2", "Key4" );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key2", "Key4" } );
        }

        [Fact]
        public void take_should_trim_dictionary_with_comparer()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key3"] = 3,
                ["Key4"] = 4,
                ["Key5"] = 5
            };

            // act
            var result = dictionary.Take( StringComparer.Ordinal, "Key2", "Key4" );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key2", "Key4" } );
        }

        [Fact]
        public void union_should_merge_dictionaries()
        {
            // arrange
            var d1 = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key3"] = 3,
                ["Key5"] = 5
            };
            var d2 = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key4"] = 4,
            };

            // act
            var result = d1.Union( d2 );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key1", "Key2", "Key3", "Key4", "Key5" } );
        }

        [Fact]
        public void union_should_merge_dictionaries_with_comparer()
        {
            // arrange
            var d1 = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key3"] = 3,
                ["Key5"] = 5
            };
            var d2 = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key4"] = 4,
            };

            // act
            var result = d1.Union( d2, StringComparer.Ordinal );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key1", "Key2", "Key3", "Key4", "Key5" } );
        }

        [Fact]
        public void reduce_should_simplify_dictionary()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                { "KeyA", 1 },
                { "KeyB", 2 },
                { "KeyC", 1 },
                { "KeyD", 2 },
                { "KeyE", 3 }
            };

            // act
            var result = dictionary.Reduce( "KeyA", "KeyC", "KeyA" );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "KeyA", "KeyB", "KeyD", "KeyE" } );
            result["KeyA"].Should().Be( 1 );
        }

        [Fact]
        public void reduce_should_simplify_dictionary_with_selector()
        {
            // arrange
            var dictionary = new Dictionary<string, int>()
            {
                ["Key1"] = 1,
                ["Key2"] = 2,
                ["Key3"] = 3,
                ["Key4"] = 4,
                ["Key5"] = 5
            };

            // act
            var result = dictionary.Reduce( "Key4", "Key5", "Key9", ( v1, v2 ) => v1 + v2 );

            // assert
            result.Keys.OrderBy( key => key ).Should().Equal( new[] { "Key1", "Key2", "Key3", "Key9" } );
            result["Key9"].Should().Be( 9 );
        }
    }
}