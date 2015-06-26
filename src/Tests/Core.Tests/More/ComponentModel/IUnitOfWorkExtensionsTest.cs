namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IUnitOfWorkExtensions"/>.
    /// </summary>
    public class IUnitOfWorkExtensionsTest
    {
        [Fact( DisplayName = "commit changes should be uncancellable" )]
        public async Task CommitChangesShouldBeUncancellable()
        {
            // arrange
            var unitOfWork = new Mock<IUnitOfWork<object>>();

            unitOfWork.Setup( u => u.CommitAsync( It.IsAny<CancellationToken>() ) ).Returns( Task.Run( (Action) DefaultAction.None ) );

            // act
            await unitOfWork.Object.CommitAsync();

            // assert
            unitOfWork.Verify( u => u.CommitAsync( CancellationToken.None ), Times.Once() );
        }
    }
}
