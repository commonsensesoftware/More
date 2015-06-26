namespace More.Collections.Generic
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="PagedCollection{T}"/>.
    /// </summary>
    public class PagedCollectionTTest
    {
        [Fact( DisplayName = "new paged collection should set total count" )]
        public void ConstructorShouldSetTotalCount()
        {
            var expected = 3;
            var actual = new PagedCollection<object>( Enumerable.Empty<object>(), expected ).TotalCount;
            Assert.Equal( expected, actual );

            actual = new PagedCollection<object>( new ObservableCollection<object>(), expected ).TotalCount;
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new paged collection should copy items" )]
        public void ConstructorShouldCopyProvidedSequence()
        {
            var expected = new []{ new object(), new object(), new object() };
            var actual = new PagedCollection<object>( expected, 3 );
            Assert.True( actual.SequenceEqual( expected ) );

            actual = new PagedCollection<object>( new ObservableCollection<object>( expected ), 3 );
            Assert.True( actual.SequenceEqual( expected ) );
        }
    }
}
