namespace System.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ICommandExtensions"/>.
    /// </summary>
    public class ICommandExtensionsTest
    {
        [Fact( DisplayName = "can execute should not allow null command" )]
        public void CanExecuteShouldNotAllowNullCommand()
        {
            // arrange
            ICommand command = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => command.CanExecute() );

            // assert
            Assert.Equal( "command", ex.ParamName );
        }

        [Fact( DisplayName = "execute should not allow null command" )]
        public void ExecuteShouldNotAllowNullCommand()
        {
            // arrange
            ICommand command = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => command.Execute() );

            // assert
            Assert.Equal( "command", ex.ParamName );
        }
    }
}
