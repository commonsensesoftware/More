namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    internal sealed class ProjectExecutionContextAdapter<T> : IProjectExecutionContext
    {
        private static readonly Action<T> dispose = CreateDispose();
        private static readonly Func<T, string, string> getProviderServices = CreateGetProviderServices();
        private readonly T instance;
        private bool disposed;

        ~ProjectExecutionContextAdapter()
        {
            Dispose( false );
        }

        public ProjectExecutionContextAdapter( T instance )
        {
            Contract.Requires( instance != null );
            this.instance = instance;
        }

        private static Action<T> CreateDispose()
        {
            Contract.Ensures(Contract.Result<Action<T>>()!=null);

            var o = Expression.Parameter( typeof( T ), "o" );
            var m = Expression.Call( o, "Dispose", null );
            var l = Expression.Lambda<Action<T>>( m, o );

            Debug.WriteLine( l );

            return l.Compile();
        }

        private static Func<T, string, string> CreateGetProviderServices()
        {
            Contract.Ensures( Contract.Result<Func<T, string, string>>() != null );

            var o = Expression.Parameter( typeof( T ), "o" );
            var s = Expression.Parameter( typeof( string ), "s" );
            var p = Expression.Property( o, "Executor" );
            var m = Expression.Call( p, "GetProviderServices", null, s );
            var l = Expression.Lambda<Func<T, string, string>>( m, o, s );

            Debug.WriteLine( l );

            return l.Compile();
        }

        private void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( !disposing )
                return;

            if ( instance != null )
                dispose( instance );
        }

        public string GetProviderServices( string invariantName )
        {
            return getProviderServices( instance, invariantName );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
