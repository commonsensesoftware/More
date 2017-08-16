namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using static Moq.Times;
    using static System.Linq.Enumerable;
    using static System.Threading.Tasks.Task;

    public class IRepositoryExtensionsTest
    {
        [Fact]
        public async Task get_all_async_should_return_all_items()
        {
            // arrange
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => FromResult( s( Empty<object>().AsQueryable() ).AsEnumerable() ) );

            var repository = mock.Object;

            // act
            await repository.GetAllAsync();

            // assert
            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Once() );
        }

        [Fact]
        public async Task find_by_async_should_return_matching_items()
        {
            // arrange
            var expected = new object();
            var items = new[] { expected };
            var query = items.AsQueryable();
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => FromResult( s( query ).AsEnumerable() ) );

            var repository = mock.Object;

            // act
            var results = ( await repository.FindByAsync( i => i.ToString() == "System.Object" ) ).ToArray();

            // assert
            results.Should().Equal( expected );
            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Once() );
        }

        [Fact]
        public async Task get_single_async_should_return_expected_item()
        {
            // arrange
            var expected = new object();
            var items = new[] { expected };
            var query = items.AsQueryable();
            var mock = new Mock<IReadOnlyRepository<object>>();

            mock.Setup( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), It.IsAny<CancellationToken>() ) )
                .Returns<Func<IQueryable<object>, IQueryable<object>>, CancellationToken>( ( s, c ) => FromResult( s( query ).AsEnumerable() ) );

            var repository = mock.Object;

            // act
            var item = await repository.GetSingleAsync( i => i.ToString() == typeof( object ).FullName );

            // assert
            item.Should().BeSameAs( expected );
            mock.Verify( r => r.GetAsync( It.IsAny<Func<IQueryable<object>, IQueryable<object>>>(), CancellationToken.None ), Once() );
        }

        [Fact]
        public async Task paginate_async_should_return_paged_items()
        {
            // arrange
            var items = new[] { new object(), new object(), new object() };
            var repository = new MockReadOnlyRepository<object>( items );

            // act
            var page = await repository.PaginateAsync( 1, 1 );

            // assert
            page.Should().Equal( new[] { items[1] } );
            page.TotalCount.Should().Be( items.LongLength );
        }

        [Fact]
        public async Task paginate_async_with_shaped_query_should_return_paged_items()
        {
            // arrange
            var items = new List<object>() { new object(), new object(), new object(), new object(), new object() };
            var repository = new MockReadOnlyRepository<object>( items.ToArray() );

            items.Sort( ( o1, o2 ) => o1.GetHashCode().CompareTo( o2.GetHashCode() ) );

            // act
            var page = await repository.PaginateAsync( q => q.OrderBy( o => o.GetHashCode() ), 1, 2 );

            // assert
            page.Should().BeEquivalentTo( new[] { items[2], items[3] } );
            page.TotalCount.Should().Be( items.Count );
        }

        [Fact]
        public async Task save_changes_async_should_be_uncancellable()
        {
            // arrange
            var repository = new Mock<IRepository<object>>();

            repository.Setup( r => r.SaveChangesAsync( It.IsAny<CancellationToken>() ) ).Returns( Run( (Action) DefaultAction.None ) );

            // act
            await repository.Object.SaveChangesAsync();

            // assert
            repository.Verify( r => r.SaveChangesAsync( CancellationToken.None ), Once() );
        }

        sealed class MockReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
        {
            readonly IQueryable<T> query;

            internal MockReadOnlyRepository( IEnumerable<T> source ) => query = source.AsQueryable();

            public Task<IEnumerable<T>> GetAsync( Func<IQueryable<T>, IQueryable<T>> queryShaper, CancellationToken cancellationToken ) => FromResult( queryShaper( query ).AsEnumerable() );

            public Task<TResult> GetAsync<TResult>( Func<IQueryable<T>, TResult> queryShaper, CancellationToken cancellationToken ) => FromResult( queryShaper( query ) );
        }
    }
}