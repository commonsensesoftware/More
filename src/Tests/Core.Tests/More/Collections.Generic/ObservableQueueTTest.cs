namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObservableQueue{T}"/>.
    /// </summary>
    public class ObservableQueueTTest
    {
        [Fact( DisplayName = "new observable queue should initialize items" )]
        public void ConstructorShouldCopySequence()
        {
            // arrange
            var expected = new[] { "1", "2", "3" };

            // act
            var target = new ObservableQueue<string>( expected );

            // assert
            Assert.Equal( 3, target.Count );
            Assert.True( expected.All( i => target.Contains( i ) ) );
        }

        [Fact( DisplayName = "enqueue should raise events" )]
        public void ShouldEnqueueItemWithEvents()
        {
            // arrange
            var expected = "1";
            var target = new ObservableQueue<string>();

            // act
            Assert.PropertyChanged( target, "Count", () => target.Enqueue( expected ) );

            // assert
            Assert.Equal( 1, target.Count );
            Assert.Equal( expected, target.Peek() );
        }

        [Fact( DisplayName = "dequeue should raise events" )]
        public void ShouldDequeueItemWithEvents()
        {
            // arrange
            var expected = "1";
            string actual = null;
            var target = new ObservableQueue<string>();

            target.Enqueue( expected );

            // act
            Assert.PropertyChanged( target, "Count", () => actual = target.Dequeue() );

            // assert
            Assert.Equal( 0, target.Count );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "dequeue should not be allowed when empty" )]
        public void ShouldNotDequeueItemWhenEmpty()
        {
            // arrange
            var target = new ObservableQueue<string>();

            // act
            Assert.Throws<InvalidOperationException>( () => target.Dequeue() );

            // assert
        }

        [Fact( DisplayName = "peek should return first item" )]
        public void ShouldPeekItem()
        {
            // arrange
            var target = new ObservableQueue<string>();

            target.Enqueue( "2" );
            target.Enqueue( "1" );
            target.Enqueue( "3" );

            // act
            var actual = target.Peek();

            // assert
            Assert.Equal( "2", actual );
        }

        [Fact( DisplayName = "peek should not be allowed when empty" )]
        public void ShoulNotPeekItemWhenEmpty()
        {
            // arrange
            var target = new ObservableQueue<string>();

            // act
            Assert.Throws<InvalidOperationException>( () => target.Peek() );

            // assert
        }

        [Fact( DisplayName = "to array should return items in seqence" )]
        public void ShouldConvertToArray()
        {
            // arrange
            var target = new ObservableQueue<string>();
            var expected = new[] { "1", "2", "3" };

            target.Enqueue( "1" );
            target.Enqueue( "2" );
            target.Enqueue( "3" );

            // act
            var actual = target.ToArray();

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "trim should remove excess" )]
        public void ShouldTrimExcess()
        {
            // arrange
            var target = new ObservableQueue<string>( 10 );
            
            target.Enqueue( "1" );
            target.Enqueue( "2" );
            target.Enqueue( "3" );

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
            var target = new ObservableQueue<string>();
            
            target.Enqueue( "One" );
            target.Enqueue( "Two" );
            target.Enqueue( "Three" );
            
            // act
            var actual = target.Contains( value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "copy to should copy items" )]
        public void ShouldCopyToSizedArray()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var target = new ObservableQueue<string>( expected );
            
            target.Enqueue( "1" );
            target.Enqueue( "2" );

            var actual = new string[2];

            // act
            target.CopyTo( actual, 0 );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "copy to should copy items" )]
        public void ShouldCopyToLargeArray()
        {
            // arrange
            var expected = new[] { "1", "2" };
            var target = new ObservableQueue<string>( expected );

            target.Enqueue( "1" );
            target.Enqueue( "2" );

            var actual = new string[4];

            // act
            target.CopyTo( actual, 2 );

            // assert
            Assert.True( actual.Skip( 2 ).SequenceEqual( expected ) );
        }

        [Fact( DisplayName = "copy to should copy items" )]
        public void ShouldCopyToWhenICollection()
        {
            // arrange
            var target = new ObservableQueue<string>();
            var collection = (ICollection) target;
            var expected = new[] { "1", "2" };
            var actual = new string[2];

            target.Enqueue( "1" );
            target.Enqueue( "2" );

            // act
            collection.CopyTo( actual, 0 );

            // assert
            Assert.True( expected.SequenceEqual( actual ) );
        }

        [Fact( DisplayName = "clear should raise events" )]
        public void ShouldClearWithEvents()
        {
            // arrange
            var target = new ObservableQueue<string>();

            target.Enqueue( "1" );

            // act
            Assert.PropertyChanged( target, "Count", target.Clear );

            // assert
            Assert.Equal( 0, target.Count );
        }

        [Fact( DisplayName = "observable queue should not be synchronized" )]
        public void ShouldNotBeSynchronized()
        {
            // arrange
            var target = (ICollection) new ObservableQueue<string>();

            // act

            // assert
            Assert.False( target.IsSynchronized );
            Assert.NotNull( target.SyncRoot );
        }

        [Fact( DisplayName = "observable queue should enumerate in sequence" )]
        public void ShouldEnumerate()
        {
            var target = new ObservableQueue<string>();

            target.Enqueue( "1" );
            target.Enqueue( "2" );
            target.Enqueue( "3" );

            var items1 = (IEnumerable<string>) target;

            foreach ( var item in items1 )
                Console.WriteLine( item );

            var items2 = (IEnumerable) target;

            foreach ( string item in items2 )
                Console.WriteLine( item );
        }

        [Fact( DisplayName = "observable queue should grow dynamically" )]
        public void ShouldGrowAutomatically()
        {
            var target = new ObservableQueue<string>( 3 );

            for ( var i = 0; i < 10; i++ )
                target.Enqueue( ( i + 1 ).ToString() );

            target.Clear();
            target.TrimExcess();
        }
    }
}
