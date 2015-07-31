namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Provides unit tests for <see cref="IRepositoryExtensions"/>.
    /// </summary>
    public class IRepositoryExtensionsTest
    {
        private sealed class MockReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
        {
            private readonly IQueryable<T> query;

            internal MockReadOnlyRepository( IEnumerable<T> source )
            {
                query = source.AsQueryable();
            }

            public Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken )
            {
                return Task.FromResult( queryShaper( query ).AsEnumerable() );
            }

            public Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken )
            {
                return Task.FromResult( queryShaper( query ) );
            }
        }

        [Fact( DisplayName = "get all async should return all items" )]
        public async Task GetAllAsyncShouldReturnAllItems()
        {
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => Task.FromResult( s( Enumerable.Empty<object>().AsQueryable() ).AsEnumerable() ) );

            var target = mock.Object;
            var actual = await target.GetAllAsync();

            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Times.Once() );
        }

        [Fact( DisplayName = "find by async should return matching items" )]
        public async Task FindByAsyncShouldReturnMatchingItems()
        {
            var expected = new object();
            var items = new[] { expected };
            var query = items.AsQueryable();
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => Task.FromResult( s( query ).AsEnumerable() ) );

            var target = mock.Object;
            var actual = ( await target.FindByAsync( i => i.ToString() == "System.Object" ) ).ToList();

            Assert.Equal( 1, actual.Count );
            Assert.Equal( expected, actual[0] );
            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Times.Once() );
        }

        [Fact( DisplayName = "get single async should return expected item" )]
        public async Task GetSingleAsyncShouldReturnExpectedItem()
        {
            var expected = new object();
            var items = new[] { expected };
            var query = items.AsQueryable();
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => Task.FromResult( s( query ).AsEnumerable() ) );

            var target = mock.Object;
            var actual = await target.GetSingleAsync( i => i.ToString() == "System.Object" );

            Assert.Equal( expected, actual );
            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Times.Once() );
        }

        [Fact( DisplayName = "paginate async should return paged items" )]
        public async Task PaginateAsyncShouldReturnPagedItems()
        {
            var items = new[] { new object(), new object(), new object() };
            var target = new MockReadOnlyRepository<object>( items );
            var actual = await target.PaginateAsync( 1, 1 );

            Assert.Equal( 1, actual.Count );
            Assert.Equal( items[1], actual[0] );
            Assert.Equal( items.LongLength, actual.TotalCount );
        }

        [Fact( DisplayName = "paginate async with shaped query should return paged items" )]
        public async Task PaginateAsyncWithShaperShouldReturnPagedItems()
        {
            var items = new[] { new object(), new object(), new object(), new object(), new object() };
            var target = new MockReadOnlyRepository<object>( items );
            var actual = await target.PaginateAsync( q => q.OrderBy( o => o.GetHashCode() ), 1, 2 );

            Assert.Equal( 2, actual.Count );
            Assert.Equal( items.LongLength, actual.TotalCount );
        }

        [Fact( DisplayName = "save changes async should be uncancellable" )]
        public async Task SaveChangesAsyncShouldBeUncancellable()
        {
            var repository = new Mock<IRepository<object>>();
            repository.Setup( r => r.SaveChangesAsync( It.IsAny<CancellationToken>() ) ).Returns( Task.Run( (Action) DefaultAction.None ) );
            await repository.Object.SaveChangesAsync();
            repository.Verify( r => r.SaveChangesAsync( CancellationToken.None ), Times.Once() );
        }
    }
}
