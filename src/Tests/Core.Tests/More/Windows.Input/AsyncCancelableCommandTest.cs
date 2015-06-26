namespace More.Windows.Input
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="AsyncCancelableCommand"/>.
    /// </summary>
    public class AsyncCancelableCommandTest
    {
        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncCancelableCommand>( id => new AsyncCancelableCommand( id, "Test", p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( id => new AsyncCancelableCommand( id, "Test", p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( "1", name, p => Task.FromResult( 0 ) ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( name, p => Task.FromResult( 0 ), p => true ) ) };
                yield return new object[] { new Func<string, AsyncCancelableCommand>( name => new AsyncCancelableCommand( "1", name, p => Task.FromResult( 0 ), p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "Test", execute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand>( execute => new AsyncCancelableCommand( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand>( canExecute => new AsyncCancelableCommand( "Test", p => Task.FromResult( 0 ), canExecute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand>( canExecute => new AsyncCancelableCommand( "1", "Test", p => Task.FromResult( 0 ), canExecute ) ) };
            }
        }

        [Theory( DisplayName = "new named data item command should set id" )]
        [MemberData( "IdData" )]
        public void ConstructorShouldSetId( Func<string, AsyncCancelableCommand> @new )
        {
            // arrange
            var expected = "42";

            // act
            var command = @new( expected );
            var actual = command.Id;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new named item command should not allow null name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldNotAllowNullName( Func<string, AsyncCancelableCommand> test )
        {
            // arrange
            string name = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( name ) );

            // assert
            Assert.Equal( "name", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow empty name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldNotAllowEmptyName( Func<string, AsyncCancelableCommand> test )
        {
            // arrange
            var name = "";

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( name ) );

            // assert
            Assert.Equal( "name", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should set name" )]
        [MemberData( "NameData" )]
        public void ConstructorShouldSetName( Func<string, AsyncCancelableCommand> @new )
        {
            // arrange
            var expected = "Test";

            // act
            var command = @new( expected );
            var actual = command.Name;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new named item command should not allow null execute method" )]
        [MemberData( "ExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Func<CancelEventArgs, Task>, AsyncCancelableCommand> test )
        {
            // arrange
            Func<CancelEventArgs, Task> executeAsyncMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeAsyncMethod ) );

            // assert
            Assert.Equal( "executeAsyncMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow null can execute method" )]
        [MemberData( "CanExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod( Func<Func<CancelEventArgs, bool>, AsyncCancelableCommand> test )
        {
            // arrange
            Func<CancelEventArgs, bool> canExecuteMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( canExecuteMethod ) );

            // assert
            Assert.Equal( "canExecuteMethod", ex.ParamName );
        }

        [Theory( DisplayName = "execute should invoke callback" )]
        [InlineData( false, true )]
        [InlineData( true, false )]
        public void ExecuteShouldInvokeCallback( bool shouldCancel, bool expected )
        {
            // arrange
            var parameter = new CancelEventArgs();
            var execute = new Mock<Func<CancelEventArgs, Task>>();

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) )
                   .Callback( ( CancelEventArgs e ) => e.Cancel = shouldCancel )
                   .Returns( Task.FromResult( 0 ) );

            var command = new AsyncCancelableCommand( "Cancel", execute.Object );
            var actual = false;

            command.Executed += ( s, e ) => actual = true;

            // act
            command.Execute( parameter );

            // assert
            Assert.Equal( expected, actual );
            execute.Verify( f => f( parameter ), Times.Once() );
        }
    }
}
