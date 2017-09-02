namespace More.Windows.Input
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class INotifyCommandChangedExtensionsTest
    {
        [Fact]
        public void raise_can_execute_changed_should_not_allow_null_commands()
        {
            // arrange
            var commands = default( IEnumerable<INotifyCommandChanged> );

            // act
            Action raiseCanExecuteChanged = () => commands.RaiseCanExecuteChanged();

            // assert
            raiseCanExecuteChanged.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( commands ) );
        }

        [Fact]
        public void raise_can_execute_changed_should_invoke_commands()
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