namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading;

    internal sealed class SynchronizedWeakDelegate
    {
        private readonly SynchronizationContext context;
        private readonly WeakDelegate weakDelegate;

        internal SynchronizedWeakDelegate( WeakDelegate weakDelegate, SynchronizationContext context )
        {
            Contract.Requires( weakDelegate != null );
            Contract.Requires( context != null );

            this.weakDelegate = weakDelegate;
            this.context = context;
        }

        internal bool IsMatch( Delegate handler )
        {
            return this.weakDelegate.IsMatch( handler );
        }

        internal bool IsMatch( Type eventArgsType )
        {
            return this.weakDelegate.IsCovariantWithMethod( typeof( string ), typeof( object ), eventArgsType );
        }

        internal Action<string, object, object> CreateCallback()
        {
            var handler = this.weakDelegate.CreateDelegate();

            if ( handler == null )
                return null;

            Action<string, object, object> callback = ( n, s, e ) =>
            {
                // this assumes IsMatch has been called to ensure the handler is covariant
                var args = new object[] { n, s, e };
                this.context.Post( state => handler.DynamicInvoke( (object[]) state ), args );
            };

            return callback;
        }
    }
}
