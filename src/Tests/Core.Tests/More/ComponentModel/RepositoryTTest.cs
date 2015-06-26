namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// Provides unit tests for <see cref="Repository{T}"/>.
    /// </summary>
    public class RepositoryTTest
    {
        [Fact( DisplayName = "has pending changes should return expected value" )]
        public void HasPendingChangesShouldReturnExpectedValue()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            
            unitOfWork.SetupGet( u => u.HasPendingChanges ).Returns( true );
            
            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            var hasChanges = target.HasPendingChanges;

            // assert
            Assert.True( hasChanges );
        }

        [Fact( DisplayName = "add should register item with unit of work" )]
        public void AddShouldRegisterNewUnitOfWorkItem()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            
            unitOfWork.Setup( u => u.RegisterNew( It.IsAny<object>() ) );

            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            target.Add( item );

            // assert
            unitOfWork.Verify( u => u.RegisterNew( item ), Times.Once() );
        }

        [Fact( DisplayName = "remove should unregister item with unit of work" )]
        public void RemoveShouldUnregisterUnitOfWorkItem()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            
            unitOfWork.Setup( u => u.RegisterRemoved( It.IsAny<object>() ) );

            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;
            
            // act
            target.Remove( item );

            // arrange
            unitOfWork.Verify( u => u.RegisterRemoved( item ), Times.Once() );
        }

        [Fact( DisplayName = "update should register item with unit of work" )]
        public void UpdateShouldUnregisterUnitOfWorkItem()
        {
            // arrange
            var item = new object();
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            
            unitOfWork.Setup( u => u.RegisterChanged( It.IsAny<object>() ) );

            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            target.Update( item );

            // assert
            unitOfWork.Verify( u => u.RegisterChanged( item ), Times.Once() );
        }

        [Fact( DisplayName = "save changes async should commit pending changes" )]
        public async Task SaveChangesAsyncShouldCommitPendingUnitOfWorkChanges()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            
            unitOfWork.Setup( u => u.CommitAsync( It.IsAny<CancellationToken>() ) ).Returns( Task.FromResult( 0 ) );

            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;

            // act
            await target.SaveChangesAsync();

            // assert
            unitOfWork.Verify( u => u.CommitAsync( CancellationToken.None ), Times.Once() );
        }

        [Theory( DisplayName = "property changed should bubble up from unit of work" )]
        [InlineData( null )]
        [InlineData( "" )]
        [InlineData( "HasPendingChanges" )]
        public void RepositoryShouldBubbleUnitOfWorkPropertyChange( string propertyName )
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();
            var target = new Mock<Repository<object>>( unitOfWork.Object ) { CallBase = true }.Object;
            var raised = false;

            target.PropertyChanged += ( s, e ) => raised = true;

            // act
            unitOfWork.Raise( u => u.PropertyChanged += null, new PropertyChangedEventArgs( propertyName ) );

            // assert
            Assert.True( raised );
        }
    }
}
