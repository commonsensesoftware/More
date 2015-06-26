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
    /// Provides unit tests for <see cref="CancelableCommand"/>.
    /// </summary>
    public class CancelableCommandTest
    {
        public static IEnumerable<object[]> IdData
        {
            get
            {
                yield return new object[] { new Func<string, CancelableCommand>( id => new CancelableCommand( id, "Test", DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( id => new CancelableCommand( id, "Test", DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> NameData
        {
            get
            {
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( "1", name, DefaultAction.None ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( name, DefaultAction.None, p => true ) ) };
                yield return new object[] { new Func<string, CancelableCommand>( name => new CancelableCommand( "1", name, DefaultAction.None, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> ExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "Test", execute ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "1", "Test", execute ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "Test", execute, p => true ) ) };
                yield return new object[] { new Func<Action<CancelEventArgs>, CancelableCommand>( execute => new CancelableCommand( "1", "Test", execute, p => true ) ) };
            }
        }

        public static IEnumerable<object[]> CanExecuteMethodData
        {
            get
            {
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, CancelableCommand>( canExecute => new CancelableCommand( "Test", DefaultAction.None, canExecute ) ) };
                yield return new object[] { new Func<Func<CancelEventArgs, bool>, CancelableCommand>( canExecute => new CancelableCommand( "1", "Test", DefaultAction.None, canExecute ) ) };
            }
        }

        [Theory( DisplayName = "new named data item command should set id" )]
        [MemberData( "IdData" )]
        public void ConstructorShouldSetId( Func<string, CancelableCommand> @new )
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
        public void ConstructorShouldNotAllowNullName( Func<string, CancelableCommand> test )
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
        public void ConstructorShouldNotAllowEmptyName( Func<string, CancelableCommand> test )
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
        public void ConstructorShouldSetName( Func<string, CancelableCommand> @new )
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
        public void ConstructorShouldNotAllowNullExecuteMethod( Func<Action<CancelEventArgs>, CancelableCommand> test )
        {
            // arrange
            Action<CancelEventArgs> executeMethod = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( executeMethod ) );

            // assert
            Assert.Equal( "executeMethod", ex.ParamName );
        }

        [Theory( DisplayName = "new named item command should not allow null can execute method" )]
        [MemberData( "CanExecuteMethodData" )]
        public void ConstructorShouldNotAllowNullCanExecuteMethod( Func<Func<CancelEventArgs, bool>, CancelableCommand> test )
        {
            // arrange
            Func<object, bool> canExecuteMethod = null;

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
            var execute = new Mock<Action<CancelEventArgs>>();

            execute.Setup( f => f( It.IsAny<CancelEventArgs>() ) )
                   .Callback( ( CancelEventArgs e ) => e.Cancel = shouldCancel );

            var command = new CancelableCommand( "Cancel", execute.Object );
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
