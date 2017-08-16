namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    [ContractClassFor( typeof( IEventBroker ) )]
    abstract class IEventBrokerContract : IEventBroker
    {
        void IEventBroker.Publish<TEventArgs>( string eventName, object eventSource, TEventArgs eventArgs )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), nameof( eventName ) );
            Contract.Requires<ArgumentNullException>( eventArgs != null, nameof( eventArgs ) );
        }

        void IEventBroker.Subscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler, SynchronizationContext context )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), nameof( eventName ) );
            Contract.Requires<ArgumentNullException>( handler != null, nameof( handler ) );
        }

        void IEventBroker.Unsubscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( eventName ), nameof( eventName ) );
            Contract.Requires<ArgumentNullException>( handler != null, nameof( handler ) );
        }
    }
}