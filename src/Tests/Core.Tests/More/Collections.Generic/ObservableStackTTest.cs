namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObservableStack{T}"/>.
    /// </summary>
    public class ObservableStackTTest
    {
        [Fact( DisplayName = "new observable stack should initialize items" )]
        public void ConstructorShouldInitializeFromSequence()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };

            // act
            var target = new ObservableStack<string>( expected );

            // assert
            Assert.Equal( 3, target.Count );
            Assert.True( target.SequenceEqual( expected.Reverse() ) );
        }

        [Fact( DisplayName = "push should raise events" )]
        public void ShouldPushItemWithEvents()
        {
            // arrange
            var expected = "1";
            var target = new ObservableStack<string>();

            // act
            Assert.PropertyChanged( target, "Count", () => target.Push( expected ) );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( expected, target.Peek() );
        }

        [Fact( DisplayName = "pop should raise events" )]
        public void ShouldPopItemWithEvents()
        {
            // arrange
            var expected = "1";
            string actual = null;
            var target = new ObservableStack<string>();

            target.Push( expected );

            // act
            Assert.PropertyChanged( target, "Count", () => actual = target.Pop() );

            // assert
            Assert.Equal( 0, target.Count );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "pop should not be allowed when empty" )]
        public void ShouldNotPopItemWhenEmpty()
        {
            // arrange
            var target = new ObservableStack<string>();

            // act
            Assert.Throws<InvalidOperationException>( () => target.Pop() );

            // assert
        }

        [Fact( DisplayName = "peek should return last item" )]
        public void ShouldPeekItem()
        {
            // arrange
            var target = new ObservableStack<string>();

            target.Push( "2" );
            target.Push( "1" );
            target.Push( "3" );

            // act
            var actual = target.Peek();

            // assert
            Assert.Equal( "3", actual );
        }

        [Fact( DisplayName = "peek should not be allowed when empty" )]
        public void ShoulNotdPeekItemWhenEmpty()
        {
            // arrange
            var target = new ObservableStack<string>();

            // act
            Assert.Throws<InvalidOperationException>( () => target.Peek() );

            // assert
        }

        [Fact( DisplayName = "to array should return items in sequence" )]
        public void ShouldConvertToArray()
        {
            // arrange
            var target = new ObservableStack<string>();
            var expected = new[] { "3", "2", "1" };

            target.Push( "1" );
            target.Push( "2" );
            target.Push( "3" );

            // act
            var actual = target.ToArray();

            // assert
            Assert.True( actual.SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "trim should remove excess" )]
        public void ShouldTrimExcess()
        {
            // arrange
            var target = new ObservableStack<string>( 10 );

            target.Push( "1" );
            target.Push( "2" );
            target.Push( "3" );

            // act
            target.TrimExcess();

            // assert
            // no exception
        }

        [Theory( DisplayName = "contains should return expected result" )]
        [InlineData( "Two", true )]
        [InlineData( "Four", false )]
        [InlineData( null, false )]
        public void ShouldContainItem( string value, bool expected )
        {
            // arrange
            var target = new ObservableStack<string>();

            target.Push( "One" );
            target.Push( "Two" );
            target.Push( "Three" );

            // act
            var actual = target.Contains( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "copy should copy items" )]
        public void ShouldCopyToSizedArray()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var target = new ObservableStack<string>( expected );
            var actual = new string[2];

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected.Reverse() ) );
        }

        [Fact( DisplayName = "copy should copy items" )]
        public void ShouldCopyToLargeArray()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var target = new ObservableStack<string>( expected );
            var actual = new string[4];

            // act
            target.CopyTo( actual, 2 );

            // assert
            Assert.True( actual.Skip( 2 ).SequenceEqual( expected.Reverse() ) );
        }

        [Fact( DisplayName = "copy should copy items" )]
        public void ShouldCopyToWhenICollection()
        {
            // arrange
            var target = new ObservableStack<string>();
            var collection = (ICollection) target;
            var expected = new[] { "1", "2" };
            var actual = new string[2];

            target.Push( "1" );
            target.Push( "2" );

            // act
            collection.CopyTo( actual, 0 );

            // assert
            Assert.True( actual.SequenceEqual( expected.Reverse() ) );
        }

        [Fact( DisplayName = "clear should raise events" )]
        public void ShouldClearWithEvents()
        {
            // arrange
            var target = new ObservableStack<string>();

            target.Push( "1" );

            // act
            Assert.PropertyChanged( target, "Count", () => target.Clear() );

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "observable stack should not be synchronized" )]
        public void ShouldNotBeSynchronized()
        {
            // arrange
            var target = (ICollection) new ObservableStack<string>();

            // act

            // assert
            Assert.False( target.IsSynchronized );
            Assert.NotNull( target.SyncRoot );
        }

        [Fact( DisplayName = "observable stack should enumerate in sequence" )]
        public void ShouldEnumerate()
        {
            var target = new ObservableStack<string>();
            target.Push( "1" );
            target.Push( "2" );
            target.Push( "3" );

            var items1 = (System.Collections.Generic.IEnumerable<string>) target;

            foreach ( var item in items1 )
                Console.WriteLine( item );

            var items2 = (IEnumerable) target;

            foreach ( var item in items2 )
                Console.WriteLine( item );
        }

        [Fact( DisplayName = "observable stack should grow dynamically" )]
        public void ShouldGrowAutomatically()
        {
            var target = new ObservableStack<string>( 0 );
            target.Push( "1" );
            Assert.Equal( 1, target.Count );
        }
    }
}
