namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading;

    [ContractClassFor( typeof( IEventBroker ) )]
    internal abstract class IEventBrokerContract : IEventBroker
    {
        void IEventBroker.Publish<TEventArgs>( string eventName, object eventSource, TEventArgs eventArgs )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), "eventName" );
        }

        void IEventBroker.Subscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler, SynchronizationContext context )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), "eventName" );
            Contract.Requires<ArgumentNullException>( handler != null, "handler" );
            Contract.Requires<ArgumentNullException>( context != null, "context" );
        }

        void IEventBroker.Unsubscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), "eventName" );
            Contract.Requires<ArgumentNullException>( handler != null, "handler" );
        }
    }
}
