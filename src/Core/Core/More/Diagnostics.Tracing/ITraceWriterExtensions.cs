namespace More.Diagnostics.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extension methods for the <see cref="ITraceWriter"/> interface.
    /// </summary>
    public static class ITraceWriterExtensions
    {
        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Debug"/> with the given message.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Debug( this ITraceWriter traceWriter, string category, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Debug, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Debug"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Debug( this ITraceWriter traceWriter, string category, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Trace( traceWriter, category, TraceLevel.Debug, exception );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Debug"/> with the given message and exception.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Debug( this ITraceWriter traceWriter, string category, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Debug, exception, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Error"/> with the given message.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Error( this ITraceWriter traceWriter, string category, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Error, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Error"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Error( this ITraceWriter traceWriter, string category, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Trace( traceWriter, category, TraceLevel.Error, exception );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Error"/> with the given message and exception.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Error( this ITraceWriter traceWriter, string category, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Error, exception, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Fatal"/> with the given message.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Fatal( this ITraceWriter traceWriter, string category, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Fatal, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Fatal"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Fatal( this ITraceWriter traceWriter, string category, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Trace( traceWriter, category, TraceLevel.Fatal, exception );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Fatal"/> with the given message and exception.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Fatal( this ITraceWriter traceWriter, string category, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Fatal, exception, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Info"/> with the given message.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Info( this ITraceWriter traceWriter, string category, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Info, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Info"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        public static void Info( this ITraceWriter traceWriter, string category, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Trace( traceWriter, category, TraceLevel.Info, exception );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Info"/> with the given message and exception.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Info( this ITraceWriter traceWriter, string category, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Info, exception, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace.  It may not be null.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Trace( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            traceWriter.Trace( category, level, traceRecord => traceRecord.Exception = exception );
        }

        /// <summary>
        /// Writes a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace. It may not be null.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Trace( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Exception = exception;
                    traceRecord.Message = messageFormat.FormatDefault( messageArguments );
                } );
        }

        /// <summary>
        /// Writes a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Trace( this ITraceWriter traceWriter, string category, TraceLevel level, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            traceWriter.Trace( category, level, traceRecord => traceRecord.Message = messageFormat.FormatDefault( messageArguments ) );
        }

        /// <summary>
        /// Writes the beginning of a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace.  It may not be null.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceBegin( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Exception = exception;
                } );
        }

        /// <summary>
        /// Writes the beginning a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace. It may not be null.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceBegin( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Exception = exception;
                    traceRecord.Message = messageFormat.FormatDefault( messageArguments );
                } );
        }

        /// <summary>
        /// Writes beginning a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceBegin( this ITraceWriter traceWriter, string category, TraceLevel level, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Message = messageFormat.FormatDefault( messageArguments );
                } );
        }

        /// <summary>
        /// Writes the end of a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace.  It may not be null.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceEnd( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.End;
                    traceRecord.Exception = exception;
                } );
        }

        /// <summary>
        /// Writes the end of a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="exception">The <see cref="Exception"/> to trace. It may not be null.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceEnd( this ITraceWriter traceWriter, string category, TraceLevel level, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.End;
                    traceRecord.Exception = exception;
                    traceRecord.Message = messageFormat.FormatDefault( messageArguments );
                } );
        }

        /// <summary>
        /// Writes the end of a single <see cref="TraceRecord"/> to the given <see cref="ITraceWriter"/> if the trace writer
        /// is enabled for the given <paramref name="category"/> and <paramref name="level"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message. It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void TraceEnd( this ITraceWriter traceWriter, string category, TraceLevel level, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.End;
                    traceRecord.Message = messageFormat.FormatDefault( messageArguments );
                } );
        }

        /// <summary>
        /// Traces both a begin and an end trace around a specified operation.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The logical category of the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> of the trace.</param>
        /// <param name="operatorName">The name of the object performing the operation. It may be null.</param>
        /// <param name="operationName">The name of the operation being performed. It may be null.</param>
        /// <param name="beginTrace">The <see cref="Action"/> to invoke prior to performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <param name="execute">An <see cref="Action"/> that performs the operation.</param>
        /// <param name="endTrace">The <see cref="Action"/> to invoke after successfully performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <param name="errorTrace">The <see cref="Action"/> to invoke if an error was encountered performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "6", Justification = "Validated by a code contract." )]
        public static void TraceBeginEnd(
            this ITraceWriter traceWriter,
            string category,
            TraceLevel level,
            string operatorName,
            string operationName,
            Action<TraceRecord> beginTrace,
            Action execute,
            Action<TraceRecord> endTrace,
            Action<TraceRecord> errorTrace )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( execute, "execute" );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Operator = operatorName;
                    traceRecord.Operation = operationName;

                    if ( beginTrace != null )
                        beginTrace( traceRecord );
                } );

            try
            {
                execute();

                traceWriter.Trace(
                    category,
                    level,
                    traceRecord =>
                    {
                        traceRecord.Kind = TraceKind.End;
                        traceRecord.Operator = operatorName;
                        traceRecord.Operation = operationName;

                        if ( endTrace != null )
                            endTrace( traceRecord );
                    } );
            }
            catch ( Exception exception )
            {
                traceWriter.TraceError( exception, category, operatorName, operationName, errorTrace );
                throw;
            }
        }

        /// <summary>
        /// Traces both a begin and an end trace around a specified asynchronous operation.
        /// </summary>
        /// <remarks>The end trace will occur when the asynchronous operation completes, either success or failure.</remarks>
        /// <typeparam name="TResult">The type of result produced by the <see cref="Task"/>.</typeparam>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The logical category of the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> of the trace.</param>
        /// <param name="operatorName">The name of the object performing the operation. It may be null.</param>
        /// <param name="operationName">The name of the operation being performed. It may be null.</param>
        /// <param name="beginTrace">The <see cref="Action"/> to invoke prior to performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <param name="execute">An <see cref="Func{Task}"/> that returns the <see cref="Task"/> that will perform the operation.</param>
        /// <param name="endTrace">The <see cref="Action"/> to invoke after successfully performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. The result of the completed task will also
        /// be passed to this action. This action may be null.</param>
        /// <param name="errorTrace">The <see cref="Action"/> to invoke if an error was encountered performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <returns>The <see cref="Task"/> returned by the operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Nested generic required for this method." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "6", Justification = "Validated by a code contract." )]
        public static Task<TResult> TraceBeginEndAsync<TResult>(
            this ITraceWriter traceWriter,
            string category,
            TraceLevel level,
            string operatorName,
            string operationName,
            Action<TraceRecord> beginTrace,
            Func<Task<TResult>> execute,
            Action<TraceRecord, TResult> endTrace,
            Action<TraceRecord> errorTrace )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( execute, "execute" );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Operator = operatorName;
                    traceRecord.Operation = operationName;

                    if ( beginTrace != null )
                        beginTrace( traceRecord );
                } );

            try
            {
                var task = execute();

                // if the operation returned a null task, we cannot do any further tracing
                if ( task == null )
                    return Task.Run<TResult>( () => default( TResult ) );

                return traceWriter.TraceBeginEndAsyncCore( category, level, operatorName, operationName, endTrace, errorTrace, task );
            }
            catch ( Exception exception )
            {
                traceWriter.TraceError( exception, category, operatorName, operationName, errorTrace );
                throw;
            }
        }

        private static async Task<TResult> TraceBeginEndAsyncCore<TResult>(
            this ITraceWriter traceWriter,
            string category,
            TraceLevel level,
            string operatorName,
            string operationName,
            Action<TraceRecord, TResult> endTrace,
            Action<TraceRecord> errorTrace,
            Task<TResult> task )
        {
            Contract.Requires( traceWriter != null );
            Contract.Requires( task != null );
            Contract.Ensures( Contract.Result<Task<TResult>>() != null );

            try
            {
                var result = await task;

                traceWriter.Trace(
                    category,
                    level,
                    traceRecord =>
                    {
                        traceRecord.Kind = TraceKind.End;
                        traceRecord.Operator = operatorName;
                        traceRecord.Operation = operationName;

                        if ( endTrace != null )
                            endTrace( traceRecord, result );
                    } );

                return result;
            }
            catch ( OperationCanceledException )
            {
                traceWriter.Trace(
                        category,
                        TraceLevel.Warn,
                        traceRecord =>
                        {
                            traceRecord.Kind = TraceKind.End;
                            traceRecord.Operator = operatorName;
                            traceRecord.Operation = operationName;
                            traceRecord.Message = SR.TraceCancelledMessage;

                            if ( errorTrace != null )
                                errorTrace( traceRecord );
                        } );

                throw;
            }
            catch ( Exception exception )
            {
                traceWriter.TraceError( exception, category, operatorName, operationName, errorTrace );
                throw;
            }
        }

        /// <summary>
        /// Traces both a begin and an end trace around a specified asynchronous operation.
        /// </summary>
        /// <remarks>The end trace will occur when the asynchronous operation completes, either success or failure.</remarks>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/>.</param>
        /// <param name="category">The logical category of the trace.</param>
        /// <param name="level">The <see cref="TraceLevel"/> of the trace.</param>
        /// <param name="operatorName">The name of the object performing the operation. It may be null.</param>
        /// <param name="operationName">The name of the operation being performed. It may be null.</param>
        /// <param name="beginTrace">The <see cref="Action"/> to invoke prior to performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <param name="execute">An <see cref="Func{Task}"/> that returns the <see cref="Task"/> that will perform the operation.</param>
        /// <param name="endTrace">The <see cref="Action"/> to invoke after successfully performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <param name="errorTrace">The <see cref="Action"/> to invoke if an error was encountered performing the operation, 
        /// allowing the given <see cref="TraceRecord"/> to be filled in. It may be null.</param>
        /// <returns>The <see cref="Task"/> returned by the operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "6", Justification = "Validated by a code contract." )]
        public static Task TraceBeginEndAsync(
            this ITraceWriter traceWriter,
            string category,
            TraceLevel level,
            string operatorName,
            string operationName,
            Action<TraceRecord> beginTrace,
            Func<Task> execute,
            Action<TraceRecord> endTrace,
            Action<TraceRecord> errorTrace )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( execute, "execute" );
            Contract.Ensures( Contract.Result<Task>() != null );

            traceWriter.Trace(
                category,
                level,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.Begin;
                    traceRecord.Operator = operatorName;
                    traceRecord.Operation = operationName;

                    if ( beginTrace != null )
                        beginTrace( traceRecord );
                } );

            try
            {
                var task = execute();

                // if the operation returned a null Task, we cannot do any further tracing
                if ( task == null )
                    return Task.Run( (Action) DefaultAction.None );

                return traceWriter.TraceBeginEndAsyncCore( category, level, operatorName, operationName, endTrace, errorTrace, task );
            }
            catch ( Exception exception )
            {
                traceWriter.TraceError( exception, category, operatorName, operationName, errorTrace );
                throw;
            }
        }

        private static async Task TraceBeginEndAsyncCore(
            this ITraceWriter traceWriter,
            string category,
            TraceLevel level,
            string operatorName,
            string operationName,
            Action<TraceRecord> endTrace,
            Action<TraceRecord> errorTrace,
            Task task )
        {
            Contract.Requires( traceWriter != null );
            Contract.Requires( task != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            try
            {
                await task;
                traceWriter.Trace(
                        category,
                        level,
                        traceRecord =>
                        {
                            traceRecord.Kind = TraceKind.End;
                            traceRecord.Operator = operatorName;
                            traceRecord.Operation = operationName;

                            if ( endTrace != null )
                                endTrace( traceRecord );
                        } );
            }
            catch ( OperationCanceledException )
            {
                traceWriter.Trace(
                        category,
                        TraceLevel.Warn,
                        traceRecord =>
                        {
                            traceRecord.Kind = TraceKind.End;
                            traceRecord.Operator = operatorName;
                            traceRecord.Operation = operationName;
                            traceRecord.Message = SR.TraceCancelledMessage;

                            if ( errorTrace != null )
                                errorTrace( traceRecord );
                        } );

                throw;
            }
            catch ( Exception exception )
            {
                traceWriter.TraceError( exception, category, operatorName, operationName, errorTrace );
                throw;
            }
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Warn"/> with the given message.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/></param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="messageFormat">The string to use to format a message.  It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Warn( this ITraceWriter traceWriter, string category, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Warn, messageFormat, messageArguments );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Warn"/> with the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/></param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace</param>
        public static void Warn( this ITraceWriter traceWriter, string category, Exception exception )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Trace( traceWriter, category, TraceLevel.Warn, exception );
        }

        /// <summary>
        /// Writes a <see cref="TraceRecord"/> at <see cref="TraceLevel.Warn"/> with the given message and exception.
        /// </summary>
        /// <param name="traceWriter">The <see cref="ITraceWriter"/></param>
        /// <param name="category">The category for the trace.</param>
        /// <param name="exception">The exception to trace</param>
        /// <param name="messageFormat">The string to use to format a message.  It may not be null.</param>
        /// <param name="messageArguments">Optional list of arguments for the <paramref name="messageFormat"/>.</param>
        public static void Warn( this ITraceWriter traceWriter, string category, Exception exception, string messageFormat, params object[] messageArguments )
        {
            Arg.NotNull( traceWriter, "traceWriter" );
            Arg.NotNull( exception, "exception" );
            Arg.NotNull( messageFormat, "messageFormat" );
            Trace( traceWriter, category, TraceLevel.Warn, exception, messageFormat, messageArguments );
        }

        private static void TraceError(
            this ITraceWriter traceWriter,
            Exception exception,
            string category,
            string operatorName,
            string operationName,
            Action<TraceRecord> errorTrace )
        {
            Contract.Requires( traceWriter != null );

            traceWriter.Trace(
                category,
                TraceLevel.Error,
                traceRecord =>
                {
                    traceRecord.Kind = TraceKind.End;
                    traceRecord.Operator = operatorName;
                    traceRecord.Operation = operationName;
                    traceRecord.Exception = exception;

                    if ( errorTrace != null )
                        errorTrace( traceRecord );
                } );
        }
    }
}