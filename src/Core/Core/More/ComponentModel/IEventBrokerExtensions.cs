namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Provides extension methods for the <see cref="IEventBroker"/> interface.
    /// </summary>
    public static class IEventBrokerExtensions
    {
        /// <summary>
        /// Subscribes to the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventBroker">The extended <see cref="IEventBroker"/> object.</param>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static void Subscribe<TEventArgs>( this IEventBroker eventBroker, string eventName, Action<string, object, TEventArgs> handler ) where TEventArgs : class
        {
            Arg.NotNull( eventBroker, nameof( eventBroker ) );
            Arg.NotNullOrEmpty( eventName, nameof( eventName ) );
            Arg.NotNull( handler, nameof( handler ) );

            eventBroker.Subscribe<TEventArgs>( eventName, handler, SynchronizationContext.Current );
        }
    }
}
