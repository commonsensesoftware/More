namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Defines the behavior of an event broker.
    /// </summary>
    [ContractClass( typeof( IEventBrokerContract ) )]
    public interface IEventBroker
    {
        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to publish.</typeparam>
        /// <param name="eventName">The name of the event to publish.</param>
        /// <param name="eventSource">The source of the event.</param>
        /// <param name="eventArgs">The <typeparamref name="TEventArgs">event arguments</typeparamref>.</param>
        void Publish<TEventArgs>( string eventName, object eventSource, TEventArgs eventArgs ) where TEventArgs : class;

        /// <summary>
        /// Subscribes to the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        /// <param name="context">The <see cref="SynchronizationContext">synchronization context</see> to subscribe on.</param>
        void Subscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler, SynchronizationContext context ) where TEventArgs : class;

        /// <summary>
        /// Unsubscribes from the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        void Unsubscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler ) where TEventArgs : class;
    }
}