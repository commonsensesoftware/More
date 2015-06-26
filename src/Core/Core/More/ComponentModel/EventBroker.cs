namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents an event broker.
    /// </summary>
    public class EventBroker : IEventBroker
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<string, List<WeakDelegate>> handlers = new Dictionary<string, List<WeakDelegate>>();

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to publish.</typeparam>
        /// <param name="eventName">The name of the event to publish.</param>
        /// <param name="eventSource">The source of the event.</param>
        /// <param name="eventArgs">The <typeparamref name="TEventArgs">event arguments</typeparamref>.</param>
        public virtual void Publish<TEventArgs>( string eventName, object eventSource, TEventArgs eventArgs ) where TEventArgs : class
        {
            Arg.NotNullOrEmpty( eventName, "eventName" );
            Arg.NotNull( eventArgs, "eventArgs" );

            List<WeakDelegate> list;

            if ( !this.handlers.TryGetValue( eventName, out list ) )
                return;

            var eventArgsType = typeof( TEventArgs );

            lock ( list )
            {
                for ( var i = list.Count - 1; i > -1; i-- )
                {
                    var handler = list[i];

                    // make sure the handler matches the published event
                    // note: the event name is not a guarantee
                    if ( !handler.IsCovariantWithMethod( typeof( string ), typeof( object ), eventArgsType ) )
                        continue;

                    var callback = handler.CreateDelegate();

                    // if the callback is null, then the source of the handler is dead;
                    // otherwise, invoke the defined callback
                    if ( callback == null )
                        list.RemoveAt( i );
                    else
                        callback.DynamicInvoke( eventName, eventSource, eventArgs );
                }
            }
        }

        /// <summary>
        /// Subscribes to the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        /// <param name="context">The <see cref="SynchronizationContext">synchronization context</see> to subscribe on.</param>
        public virtual void Subscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler, SynchronizationContext context ) where TEventArgs : class
        {
            Arg.NotNullOrEmpty( eventName, "eventName" );
            Arg.NotNull( handler, "handler" );

            WeakDelegate item =
                context == null ?
                new WeakDelegate<Action<string, object, TEventArgs>>( handler ) :
                new SynchronizedWeakDelegate<TEventArgs>( handler, context );
            List<WeakDelegate> list;

            if ( !this.handlers.TryGetValue( eventName, out list ) )
            {
                lock ( this.syncRoot )
                {
                    if ( !this.handlers.TryGetValue( eventName, out list ) )
                    {
                        // since the list didn't exist, add the item here to avoid a second lock
                        list = new List<WeakDelegate>();
                        list.Add( item );
                        this.handlers.Add( eventName, list );
                        return;
                    }
                }
            }

            lock ( list )
                list.Add( item );
        }

        /// <summary>
        /// Unsubscribes from the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        public virtual void Unsubscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler ) where TEventArgs : class
        {
            Arg.NotNullOrEmpty( eventName, "eventName" );
            Arg.NotNull( handler, "handler" );

            List<WeakDelegate> list;

            lock ( this.syncRoot )
            {
                if ( !this.handlers.TryGetValue( eventName, out list ) )
                    return;
            }

            lock ( list )
            {
                var item = list.FirstOrDefault( i => i.IsMatch( handler ) );

                if ( item != null )
                    list.Remove( item );
            }
        }
    }
}
