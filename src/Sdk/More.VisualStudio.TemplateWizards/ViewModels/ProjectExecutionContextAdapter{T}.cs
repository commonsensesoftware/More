namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using static System.Linq.Expressions.Expression;

    sealed class ProjectExecutionContextAdapter<T> : IProjectExecutionContext
    {
        static readonly Action<T> dispose = CreateDispose();
        static readonly Func<T, string, string> getProviderServices = CreateGetProviderServices();
        readonly T instance;
        bool disposed;

        ~ProjectExecutionContextAdapter() => Dispose( false );

        public ProjectExecutionContextAdapter( T instance ) => this.instance = instance;

        static Action<T> CreateDispose()
        {
            Contract.Ensures( Contract.Result<Action<T>>() != null );

            var o = Parameter( typeof( T ), "o" );
            var m = Call( o, "Dispose", null );
            var l = Lambda<Action<T>>( m, o );

            Debug.WriteLine( l );

            return l.Compile();
        }

        static Func<T, string, string> CreateGetProviderServices()
        {
            Contract.Ensures( Contract.Result<Func<T, string, string>>() != null );

            var o = Parameter( typeof( T ), "o" );
            var s = Parameter( typeof( string ), "s" );
            var p = Property( o, "Executor" );
            var m = Call( p, "GetProviderServices", null, s );
            var l = Lambda<Func<T, string, string>>( m, o, s );

            Debug.WriteLine( l );

            return l.Compile();
        }

        void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            if ( instance != null )
            {
                dispose( instance );
            }
        }

        public string GetProviderServices( string invariantName ) => getProviderServices( instance, invariantName );

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}