namespace System.Collections.Generic
{
    using Moq;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IDictionaryExtensions"/>.
    /// </summary>
    public partial class IDictionaryExtensionsTest
    {
        [Fact( DisplayName = "as read-only should return self when read-only" )]
        public void AsReadOnlyShouldReturnSelfIfAlreadyReadOnly()
        {
            // arrange
            var mock = new Mock<IDictionary<string, object>>();

            mock.SetupGet( d => d.IsReadOnly ).Returns( true );
            mock.As<IReadOnlyDictionary<string, object>>();

            var target = mock.Object;

            // act
            var actual = target.AsReadOnly();

            // assert
            Assert.Same( target, actual );
        }

        [Fact( DisplayName = "get or add should return existing item" )]
        public void GetOrAddShouldReturnExistingValue()
        {
            // arrange
            var expected = new object();
            var target = new Dictionary<string, object>() { { "Test", expected } };
            
            // act
            var actual = target.GetOrAdd( "Test", () => new object() );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 1, target.Count );
            Assert.True( target.ContainsKey( "Test" ) );
        }

        [Fact( DisplayName = "get or add should insert and return new item" )]
        public void GetOrAddShouldInsertAndReturnNewItemIntoDictionary()
        {
            // arrange
            var expected = new object();
            var target = new Dictionary<string, object>();

            // act
            var actual = target.GetOrAdd( "Test", () => expected );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 1, target.Count );
            Assert.True( target.ContainsKey( "Test" ) );
        }

        [Fact( DisplayName = "skip should trim dictionary" )]
        public void SkipShouldTrimDictionary()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 },
                { "Key4", 4 },
                { "Key5", 5 }
            };

            // act
            var actual = target.Skip( "Key2", "Key4" );

            // assert
            Assert.Equal( 3, actual.Count );
            Assert.True( actual.ContainsKey( "Key1" ) );
            Assert.False( actual.ContainsKey( "Key2" ) );
            Assert.True( actual.ContainsKey( "Key3" ) );
            Assert.False( actual.ContainsKey( "Key4" ) );
            Assert.True( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "skip should trim dictionary with comparer" )]
        public void SkipShouldTrimDictionaryWithComparer()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 },
                { "Key4", 4 },
                { "Key5", 5 }
            };

            // act
            var actual = target.Skip( StringComparer.Ordinal, "Key2", "Key4" );

            // assert
            Assert.Equal( 3, actual.Count );
            Assert.True( actual.ContainsKey( "Key1" ) );
            Assert.False( actual.ContainsKey( "Key2" ) );
            Assert.True( actual.ContainsKey( "Key3" ) );
            Assert.False( actual.ContainsKey( "Key4" ) );
            Assert.True( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "take should trim dictionary" )]
        public void TakeShouldTrimDictionary()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 },
                { "Key4", 4 },
                { "Key5", 5 }
            };

            // act
            var actual = target.Take( "Key2", "Key4" );

            // assert
            Assert.Equal( 2, actual.Count );
            Assert.False( actual.ContainsKey( "Key1" ) );
            Assert.True( actual.ContainsKey( "Key2" ) );
            Assert.False( actual.ContainsKey( "Key3" ) );
            Assert.True( actual.ContainsKey( "Key4" ) );
            Assert.False( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "take should trim dictionary with comparer" )]
        public void TakeShouldTrimDictionaryWithComparer()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 },
                { "Key4", 4 },
                { "Key5", 5 }
            };

            // act
            var actual = target.Take( StringComparer.Ordinal, "Key2", "Key4" );

            // assert
            Assert.Equal( 2, actual.Count );
            Assert.False( actual.ContainsKey( "Key1" ) );
            Assert.True( actual.ContainsKey( "Key2" ) );
            Assert.False( actual.ContainsKey( "Key3" ) );
            Assert.True( actual.ContainsKey( "Key4" ) );
            Assert.False( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "union should merge dictionaries" )]
        public void UnionShouldMergeDictionaries()
        {
            // arrange
            var d1 = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key3", 3 },
                { "Key5", 5 }
            };
            var d2 = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key4", 4 }
            };

            // act
            var actual = d1.Union( d2 );

            // assert
            Assert.Equal( 5, actual.Count );
            Assert.True( actual.ContainsKey( "Key1" ) );
            Assert.True( actual.ContainsKey( "Key2" ) );
            Assert.True( actual.ContainsKey( "Key3" ) );
            Assert.True( actual.ContainsKey( "Key4" ) );
            Assert.True( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "union should merge dictionaries with comparer" )]
        public void UnionShouldMergeDictionariesWithComparer()
        {
            // arrange
            var d1 = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key3", 3 },
                { "Key5", 5 }
            };
            var d2 = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key4", 4 }
            };

            // act
            var actual = d1.Union( d2, StringComparer.Ordinal );

            // assert
            Assert.Equal( 5, actual.Count );
            Assert.True( actual.ContainsKey( "Key1" ) );
            Assert.True( actual.ContainsKey( "Key2" ) );
            Assert.True( actual.ContainsKey( "Key3" ) );
            Assert.True( actual.ContainsKey( "Key4" ) );
            Assert.True( actual.ContainsKey( "Key5" ) );
        }

        [Fact( DisplayName = "reduce should simplify dictionary" )]
        public void ReduceShouldSimplifyDictionaryValues()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "KeyA", 1 },
                { "KeyB", 2 },
                { "KeyC", 1 },
                { "KeyD", 2 },
                { "KeyE", 3 }
            };

            // act
            // KeyA and KeyC are removed and a new KeyA is added with value 1 (from the original KeyA)
            var actual = target.Reduce( "KeyA", "KeyC", "KeyA" );

            // assert
            Assert.Equal( 4, actual.Count );
            Assert.True( actual.ContainsKey( "KeyA" ) );
            Assert.True( actual.ContainsKey( "KeyB" ) );
            Assert.False( actual.ContainsKey( "KeyC" ) );
            Assert.True( actual.ContainsKey( "KeyD" ) );
            Assert.True( actual.ContainsKey( "KeyE" ) );
            Assert.Equal( 1, actual["KeyA"] );
        }

        [Fact( DisplayName = "reduce should simplify dictionary with selector" )]
        public void ReduceShouldSimplifyDictionaryValuesWithSelector()
        {
            // arrange
            var target = new Dictionary<string, int>()
            {
                { "Key1", 1 },
                { "Key2", 2 },
                { "Key3", 3 },
                { "Key4", 4 },
                { "Key5", 5 }
            };

            // act
            // Key4 and Key5 are removed and Key9 is added
            var actual = target.Reduce( "Key4", "Key5", "Key9", ( v1, v2 ) => v1 + v2 );

            // assert
            Assert.Equal( 4, actual.Count );
            Assert.True( actual.ContainsKey( "Key1" ) );
            Assert.True( actual.ContainsKey( "Key2" ) );
            Assert.True( actual.ContainsKey( "Key3" ) );
            Assert.False( actual.ContainsKey( "Key4" ) );
            Assert.False( actual.ContainsKey( "Key5" ) );
            Assert.True( actual.ContainsKey( "Key9" ) );
            Assert.Equal( 9, actual["Key9"] );
        }
    }
}
