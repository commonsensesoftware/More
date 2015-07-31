namespace More.VisualStudio
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class AppDomainExtensions
    {
        private class RemoteTaskCompletionSource<T> : MarshalByRefObject
        {
            private readonly TaskCompletionSource<T> source = new TaskCompletionSource<T>();

            public void SetResult( T result )
            {
                source.SetResult( result );
            }

            public void SetException( Exception[] exception )
            {
                source.SetException( exception );
            }

            public void SetCanceled()
            {
                source.SetCanceled();
            }

            public Task<T> Task
            {
                get
                {
                    Contract.Ensures( Contract.Result<Task<T>>() != null );
                    return source.Task;
                }
            }
        }

        private class RemoteWorker<T> : MarshalByRefObject
        {
            [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Cannot be static for remoting." )]
            public void Run( Func<Task<T>> function, RemoteTaskCompletionSource<T> taskCompletionSource )
            {
                Contract.Requires( function != null );
                Contract.Requires( taskCompletionSource != null );

                function().ContinueWith(
                    t =>
                    {
                        if ( t.IsFaulted )
                            taskCompletionSource.SetException( t.Exception.InnerExceptions.ToArray() );
                        else if ( t.IsCanceled )
                            taskCompletionSource.SetCanceled();
                        else
                            taskCompletionSource.SetResult( t.Result );
                    } );
            }

            [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Cannot be static for remoting." )]
            public void Run( Func<Task> function, RemoteTaskCompletionSource<T> taskCompletionSource )
            {
                Contract.Requires( function != null );
                Contract.Requires( taskCompletionSource != null );

                function().ContinueWith(
                    t =>
                    {
                        if ( t.IsFaulted )
                            taskCompletionSource.SetException( t.Exception.InnerExceptions.ToArray() );
                        else if ( t.IsCanceled )
                            taskCompletionSource.SetCanceled();
                        else
                            taskCompletionSource.SetResult( default( T ) );
                    } );
            }

            [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Cannot be static for remoting." )]
            public void Run<TArg>( Func<TArg, Task> function, TArg arg, RemoteTaskCompletionSource<T> taskCompletionSource )
            {
                Contract.Requires( function != null );
                Contract.Requires( taskCompletionSource != null );

                function( arg ).ContinueWith(
                    t =>
                    {
                        if ( t.IsFaulted )
                            taskCompletionSource.SetException( t.Exception.InnerExceptions.ToArray() );
                        else if ( t.IsCanceled )
                            taskCompletionSource.SetCanceled();
                        else
                            taskCompletionSource.SetResult( default( T ) );
                    } );
            }
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for future use." )]
        internal static Task<T> RunAsync<T>( this AppDomain appDomain, Func<Task<T>> function )
        {
            var tcs = new RemoteTaskCompletionSource<T>();
            var type = typeof( RemoteWorker<T> );
            var worker = (RemoteWorker<T>) appDomain.CreateInstanceAndUnwrap( type.Assembly.FullName, type.FullName );
            worker.Run( function, tcs );
            return tcs.Task;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reserved for future use." )]
        internal static Task RunAsync( this AppDomain appDomain, Func<Task> function )
        {
            var tcs = new RemoteTaskCompletionSource<object>();
            var type = typeof( RemoteWorker<object> );
            var worker = (RemoteWorker<object>) appDomain.CreateInstanceAndUnwrap( type.Assembly.FullName, type.FullName );
            worker.Run( function, tcs );
            return tcs.Task;
        }

        internal static Task RunAsync<T>( this AppDomain appDomain, Func<T, Task> function, T arg )
        {
            var tcs = new RemoteTaskCompletionSource<object>();
            var type = typeof( RemoteWorker<object> );
            var worker = (RemoteWorker<object>) appDomain.CreateInstanceAndUnwrap( type.Assembly.FullName, type.FullName );
            worker.Run( function, arg, tcs );
            return tcs.Task;
        }
    }
}
