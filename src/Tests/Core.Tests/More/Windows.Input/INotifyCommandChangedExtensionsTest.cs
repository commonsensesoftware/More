namespace More.Windows.Input
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="INotifyCommandChangedExtensions"/>.
    /// </summary>
    public class INotifyCommandChangedExtensionsTest
    {
        [Fact( DisplayName = "raise can execute changed should not allow null commands" )]
        public void RaiseCanExecuteChangedShouldNotAllowNullCommands()
        {
            // arrange
            IEnumerable<INotifyCommandChanged> commands = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => commands.RaiseCanExecuteChanged() );

            // assert
            Assert.Equal( "commands", ex.ParamName );
        }

        [Fact( DisplayName = "raise can execute changed should invoke commands" )]
        public void RaiseCanExecuteChangedShouldInvokeCommands()
        {
            // arrange
            var mocks = new List<Mock<INotifyCommandChanged>>()
            {
                new Mock<INotifyCommandChanged>(),
                new Mock<INotifyCommandChanged>(),
                new Mock<INotifyCommandChanged>()
            };

            mocks.ForEach( mock => mock.Setup( m => m.RaiseCanExecuteChanged() ) );

            var commands = mocks.Select( m => m.Object ).ToArray();

            // act
            commands.RaiseCanExecuteChanged();

            // assert
            mocks.ForEach( mock => mock.Verify( m => m.RaiseCanExecuteChanged(), Times.Once() ) );
        }
    }
}
