namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using static Moq.Times;

    public class RepositoryTTest
    {
        [Fact]
        public void has_pending_changes_should_return_expected_value()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.SetupGet( u => u.HasPendingChanges ).Returns( true );

            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            var hasChanges = repository.HasPendingChanges;

            // assert
            hasChanges.Should().BeTrue();
        }

        [Fact]
        public void add_should_register_item_with_unit_of_work()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.Setup( u => u.RegisterNew( It.IsAny<object>() ) );

            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            repository.Add( item );

            // assert
            unitOfWork.Verify( u => u.RegisterNew( item ), Once() );
        }

        [Fact]
        public void remove_should_unregister_item_with_unit_of_work()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.Setup( u => u.RegisterRemoved( It.IsAny<object>() ) );

            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            repository.Remove( item );

            // arrange
            unitOfWork.Verify( u => u.RegisterRemoved( item ), Once() );
        }

        [Fact]
        public void update_should_register_item_with_unit_of_work()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.Setup( u => u.RegisterChanged( It.IsAny<object>() ) );

            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            repository.Update( item );

            // assert
            unitOfWork.Verify( u => u.RegisterChanged( item ), Once() );
        }

        [Fact]
        public async Task save_changes_async_should_commit_pending_changes()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.Setup( u => u.CommitAsync( It.IsAny<CancellationToken>() ) ).Returns( Task.FromResult( 0 ) );

            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            await repository.SaveChangesAsync();

            // assert
            unitOfWork.Verify( u => u.CommitAsync( CancellationToken.None ), Once() );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "HasPendingChanges" )]
        public void property_changed_should_bubble_up_from_unit_of_work( string propertyName )
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            var repository = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            repository.MonitorEvents();

            // act
            unitOfWork.Raise( u => u.PropertyChanged += null, new PropertyChangedEventArgs( propertyName ) );

            // assert
            repository.ShouldRaise( "PropertyChanged" ).WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == propertyName );
        }
    }
}