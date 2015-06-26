namespace More.Diagnostics.Tracing
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Extensions;

    /// <summary>
    /// Provides unit tests for <see cref="ITraceWriterExtensions"/>.
    /// </summary>
    public class ITraceWriterExtensionsTest
    {
        public static IEnumerable<object[]> ExtensionMethodsWithNullOperations
        {
            get
            {
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, new Exception() ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, new Exception(), "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEnd( "Category", TraceLevel.Info, null, null, null, () => { }, null, null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEndAsync( "Category", TraceLevel.Info, null, null, null, () => (Task) Task.FromResult<object>( null ), null, null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEndAsync( "Category", TraceLevel.Info, null, null, null, () => Task.FromResult<object>( null ), null, null ) ) };
            }
        }

        public static IEnumerable<object[]> ExtensionMethodsWithNullExceptions
        {
            get
            {
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, (Exception) null, "", new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, (Exception) null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, (Exception) null, "", new object[0] ) ) };
            }
        }

        public static IEnumerable<object[]> ExtensionMethodsWithNullMessageFormat
        {
            get
            {
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Debug( "Category", new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Error( "Category", new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Fatal( "Category", new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Info( "Category", new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Warn( "Category", new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.Trace( "Category", TraceLevel.Info, new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBegin( "Category", TraceLevel.Info, new Exception(), (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, (string) null, new object[0] ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceEnd( "Category", TraceLevel.Info, new Exception(), (string) null, new object[0] ) ) };
            }
        }

        public static IEnumerable<object[]> ExtensionMethodsWithNullExecuteActions
        {
            get
            {
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEnd( "Category", TraceLevel.Info, null, null, null, (Action) null, null, null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEndAsync( "Category", TraceLevel.Info, null, null, null, (Func<Task>) null, null, null ) ) };
                yield return new object[] { new Action<ITraceWriter>( t => t.TraceBeginEndAsync( "Category", TraceLevel.Info, null, null, null, (Func<Task<object>>) null, null, null ) ) };
            }
        }

        public static IEnumerable<object[]> TraceExtensionSampleRuns
        {
            get
            {
                var exception = new Exception();

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Debug( t, "Category", "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Debug, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Debug( t, "Category", exception ) ), TraceKind.Trace, "Category", TraceLevel.Debug, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Debug( t, "Category", exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Debug, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Error( t, "Category", "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Error, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Error( t, "Category", exception ) ), TraceKind.Trace, "Category", TraceLevel.Error, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Error( t, "Category", exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Error, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Fatal( t, "Category", "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Fatal, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Fatal( t, "Category", exception ) ), TraceKind.Trace, "Category", TraceLevel.Fatal, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Fatal( t, "Category", exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Fatal, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Info( t, "Category", "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Info, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Info( t, "Category", exception ) ), TraceKind.Trace, "Category", TraceLevel.Info, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Info( t, "Category", exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Info, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Warn( t, "Category", "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Warn, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Warn( t, "Category", exception ) ), TraceKind.Trace, "Category", TraceLevel.Warn, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Warn( t, "Category", exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Warn, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Trace( t, "Category", TraceLevel.Info, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Info, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Trace( t, "Category", TraceLevel.Info, exception ) ), TraceKind.Trace, "Category", TraceLevel.Info, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.Trace( t, "Category", TraceLevel.Info, exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Trace, "Category", TraceLevel.Info, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceBegin( t, "Category", TraceLevel.Info, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Begin, "Category", TraceLevel.Info, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceBegin( t, "Category", TraceLevel.Info, exception ) ), TraceKind.Begin, "Category", TraceLevel.Info, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceBegin( t, "Category", TraceLevel.Info, exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.Begin, "Category", TraceLevel.Info, exception, "This is a test." };

                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceEnd( t, "Category", TraceLevel.Info, "This is a {0}.", new object[] { "test" } ) ), TraceKind.End, "Category", TraceLevel.Info, null, "This is a test." };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceEnd( t, "Category", TraceLevel.Info, exception ) ), TraceKind.End, "Category", TraceLevel.Info, exception, null };
                yield return new object[] { new Action<ITraceWriter>( t => ITraceWriterExtensions.TraceEnd( t, "Category", TraceLevel.Info, exception, "This is a {0}.", new object[] { "test" } ) ), TraceKind.End, "Category", TraceLevel.Info, exception, "This is a test." };
            }
        }

        private static Mock<ITraceWriter> CreateTraceWriter( IList<TraceRecord> traceRecords )
        {
            var traceWriter = new Mock<ITraceWriter>();

            traceWriter.Setup( t => t.Trace( It.IsAny<string>(), It.IsAny<TraceLevel>(), It.IsAny<Action<TraceRecord>>() ) )
                       .Callback<string, TraceLevel, Action<TraceRecord>>(
                            ( category, level, action ) =>
                            {
                                var record = new TraceRecord( category, level );
                                
                                traceRecords.Add( record );

                                if ( action != null )
                                    action( record );
                            } );

            return traceWriter;
        }

        [Theory( DisplayName = "extension method should not allow null trace writer" )]
        [MemberData( "ExtensionMethodsWithNullOperations" )]
        public void ExtensionMethodsShouldThrowExceptionForNullTraceWriter( Action<ITraceWriter> test )
        {
            // arrange
            ITraceWriter traceWriter = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( traceWriter ) );

            // assert
            Assert.Equal( "traceWriter", ex.ParamName );
        }

        [Theory( DisplayName = "trace writer extension method should not allow null exception" )]
        [MemberData( "ExtensionMethodsWithNullExceptions" )]
        public void ExtensionMethodsShouldThrowExceptionForNullException( Action<ITraceWriter> test )
        {
            // arrange
            var traceWriter = new Mock<ITraceWriter>().Object;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( traceWriter ) );

            // assert
            Assert.Equal( "exception", ex.ParamName );
        }

        [Theory( DisplayName = "trace writer extension method should not allow null or empty message format" )]
        [MemberData( "ExtensionMethodsWithNullMessageFormat" )]
        public void ExtensionMethodsShouldThrowExceptionForNullMessageFormat( Action<ITraceWriter> test )
        {
            // arrange
            var traceWriter = new Mock<ITraceWriter>().Object;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( traceWriter ) );

            // assert
            Assert.Equal( "messageFormat", ex.ParamName );
        }

        [Theory( DisplayName = "trace writer extension method should not allow null execute action" )]
        [MemberData( "ExtensionMethodsWithNullExecuteActions" )]
        public void ExtensionMethodsShouldThrowExceptionForNullExecuteAction( Action<ITraceWriter> test )
        {
            // arrange
            var traceWriter = new Mock<ITraceWriter>().Object;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( traceWriter ) );

            // assert
            Assert.Equal( "execute", ex.ParamName );
        }

        [Theory( DisplayName = "trace should create expected trace record" )]
        [MemberData( "TraceExtensionSampleRuns" )]
        public void TraceShouldCreateExpectedTraceRecord( Action<ITraceWriter> test, TraceKind kind, string category, TraceLevel level, Exception exception, string message )
        {
            // arrange
            var traceWriter = new Mock<ITraceWriter>();
            TraceRecord actual = null;

            traceWriter.Setup( t => t.Trace( It.IsAny<string>(), It.IsAny<TraceLevel>(), It.IsAny<Action<TraceRecord>>() ) )
                       .Callback<string, TraceLevel, Action<TraceRecord>>( ( c, l, a ) => a( actual = new TraceRecord( Guid.NewGuid(), c, l ) ) );

            // act
            test( traceWriter.Object );

            // assert
            Assert.NotNull( actual );
            Assert.Equal( kind, actual.Kind );
            Assert.Equal( category, actual.Category );
            Assert.Equal( level, actual.Level );
            Assert.Equal( exception, actual.Exception );
            Assert.Equal( message, actual.Message );
        }

        [Fact( DisplayName = "trace begin/end should create expected trace records" )]
        public void TraceBeginEndShouldCreateExpectedTraceRecords()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Action>();
            var endTrace = new Mock<Action<TraceRecord>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            traceWriter.Object.TraceBeginEnd( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object );

            // assert
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
            errorTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
        }

        [Fact( DisplayName = "trace begin/end should handle exception" )]
        public void TraceBeginEndShouldHandleException()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var expected = new Exception();
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Action>();
            var endTrace = new Mock<Action<TraceRecord>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Callback( () => { throw expected; } );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = Assert.Throws<Exception>( () => traceWriter.Object.TraceBeginEnd( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object ) );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Error, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            Assert.Equal( expected, traceRecords[1].Exception );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
            errorTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
        }

        [Fact( DisplayName = "trace begin/end async should create expected trace records" )]
        public async Task TraceBeginEndAsyncShouldCreateExpectedTraceRecords()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task>>();
            var endTrace = new Mock<Action<TraceRecord>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.FromResult( 0 ) ); 
            endTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            await traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object );

            // assert
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
            errorTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
        }

        [Fact( DisplayName = "trace begin/end async should handle exception" )]
        public async Task TraceBeginEndAsyncShouldHandleException()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var expected = new Exception();
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task>>();
            var endTrace = new Mock<Action<TraceRecord>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.Run( () => { throw expected; } ) );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = await Assert.ThrowsAsync<Exception>( () => traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object ) );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Error, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            Assert.Equal( expected, traceRecords[1].Exception );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
            errorTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
        }

        [Fact( DisplayName = "trace begin/end async should handle cancellation" )]
        public async Task TraceBeginEndAsyncShouldHandleCancellation()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var expected = new OperationCanceledException();
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task>>();
            var endTrace = new Mock<Action<TraceRecord>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.Run( () => { throw expected; } ) );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = await Assert.ThrowsAsync<OperationCanceledException>( () => traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object ) );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Warn, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            Assert.Equal( "Cancelled", traceRecords[1].Message );
            Assert.Null( traceRecords[1].Exception );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
            errorTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
        }

        [Fact( DisplayName = "trace begin/end async with result should create expected trace records" )]
        public async Task TraceBeginEndAsyncWithResultShouldCreateExpectedTraceRecords()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task<object>>>();
            var endTrace = new Mock<Action<TraceRecord, object>>();
            var errorTrace = new Mock<Action<TraceRecord>>();
            var expected = new object();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.FromResult( expected ) );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>(), It.IsAny<object>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = await traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( traceRecords[1], expected ), Times.Once() );
            errorTrace.Verify( f => f( It.IsAny<TraceRecord>() ), Times.Never() );
        }

        [Fact( DisplayName = "trace begin/end async with result should handle exception" )]
        public async Task TraceBeginEndAsyncWithResultShouldHandleException()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var expected = new Exception();
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task<object>>>();
            var endTrace = new Mock<Action<TraceRecord, object>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.Run( new Func<object>( () => { throw expected; } ) ) );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>(), It.IsAny<object>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = await Assert.ThrowsAsync<Exception>( () => traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object ) );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Error, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            Assert.Equal( expected, traceRecords[1].Exception );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( It.IsAny<TraceRecord>(), It.IsAny<object>() ), Times.Never() );
            errorTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
        }

        [Fact( DisplayName = "trace begin/end async should handle cancellation" )]
        public async Task TraceBeginEndAsyncWithResultShouldHandleCancellation()
        {
            // arrange
            var category = "Category";
            var level = TraceLevel.Debug;
            var operatorName = "Operator";
            var operationName = "Operation";
            var expected = new OperationCanceledException();
            var traceRecords = new List<TraceRecord>();
            var traceWriter = CreateTraceWriter( traceRecords );
            var beginTrace = new Mock<Action<TraceRecord>>();
            var execute = new Mock<Func<Task<object>>>();
            var endTrace = new Mock<Action<TraceRecord, object>>();
            var errorTrace = new Mock<Action<TraceRecord>>();

            beginTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );
            execute.Setup( f => f() ).Returns( () => Task.Run( new Func<object>( () => { throw expected; } ) ) );
            endTrace.Setup( f => f( It.IsAny<TraceRecord>(), It.IsAny<object>() ) );
            errorTrace.Setup( f => f( It.IsAny<TraceRecord>() ) );

            // act
            var actual = await Assert.ThrowsAsync<OperationCanceledException>( () => traceWriter.Object.TraceBeginEndAsync( category, level, operatorName, operationName, beginTrace.Object, execute.Object, endTrace.Object, errorTrace.Object ) );

            // assert
            Assert.Equal( expected, actual );
            Assert.Equal( 2, traceRecords.Count );
            Assert.Equal( TraceKind.Begin, traceRecords[0].Kind );
            Assert.Equal( TraceLevel.Debug, traceRecords[0].Level );
            Assert.Equal( category, traceRecords[0].Category );
            Assert.Equal( operatorName, traceRecords[0].Operator );
            Assert.Equal( operationName, traceRecords[0].Operation );
            Assert.Equal( TraceKind.End, traceRecords[1].Kind );
            Assert.Equal( TraceLevel.Warn, traceRecords[1].Level );
            Assert.Equal( category, traceRecords[1].Category );
            Assert.Equal( operatorName, traceRecords[1].Operator );
            Assert.Equal( operationName, traceRecords[1].Operation );
            Assert.Equal( "Cancelled", traceRecords[1].Message );
            Assert.Null( traceRecords[1].Exception );
            beginTrace.Verify( f => f( traceRecords[0] ), Times.Once() );
            execute.Verify( f => f(), Times.Once() );
            endTrace.Verify( f => f( It.IsAny<TraceRecord>(), It.IsAny<object>() ), Times.Never() );
            errorTrace.Verify( f => f( traceRecords[1] ), Times.Once() );
        }
    }
}
