namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class IUnitOfWorkExtensionsTest
    {
        [Fact]
        public async Task commit_changes_should_be_uncancellable()
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