namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    internal sealed class SynchronizedWeakDelegate<T> : WeakDelegate<Action<string, object, T>> where T : class
    {
        private readonly SynchronizationContext context;

        internal SynchronizedWeakDelegate( Action<string, object, T> strongDelegate, SynchronizationContext context )
            : base( strongDelegate )
        {
            Contract.Requires( strongDelegate != null );
            Contract.Requires( context != null );

            this.context = context;
        }

        public override Action<string, object, T> CreateTypedDelegate()
        {
            var handler = base.CreateTypedDelegate();

            if ( handler == null )
                return null;

            return ( n, s, e ) => context.Post( state => handler( n, s, e ), null );
        }
    }
}
