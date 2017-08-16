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
        readonly object syncRoot = new object();
        readonly Dictionary<string, List<WeakDelegate>> handlers = new Dictionary<string, List<WeakDelegate>>();

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to publish.</typeparam>
        /// <param name="eventName">The name of the event to publish.</param>
        /// <param name="eventSource">The source of the event.</param>
        /// <param name="eventArgs">The <typeparamref name="TEventArgs">event arguments</typeparamref>.</param>
        public virtual void Publish<TEventArgs>( string eventName, object eventSource, TEventArgs eventArgs ) where TEventArgs : class
        {
            Arg.NotNullOrEmpty( eventName, nameof( eventName ) );
            Arg.NotNull( eventArgs, nameof( eventArgs ) );

            if ( !handlers.TryGetValue( eventName, out var list ) )
            {
                return;
            }

            var eventArgsType = typeof( TEventArgs );

            lock ( list )
            {
                for ( var i = list.Count - 1; i > -1; i-- )
                {
                    var handler = list[i];

                    if ( !handler.IsCovariantWithMethod( typeof( string ), typeof( object ), eventArgsType ) )
                    {
                        continue;
                    }

                    var callback = handler.CreateDelegate();

                    if ( callback == null )
                    {
                        list.RemoveAt( i );
                    }
                    else
                    {
                        callback.DynamicInvoke( eventName, eventSource, eventArgs );
                    }
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
            Arg.NotNullOrEmpty( eventName, nameof( eventName ) );
            Arg.NotNull( handler, nameof( handler ) );

            WeakDelegate item =
                context == null ?
                new WeakDelegate<Action<string, object, TEventArgs>>( handler ) :
                new SynchronizedWeakDelegate<TEventArgs>( handler, context );

            if ( !handlers.TryGetValue( eventName, out var list ) )
            {
                lock ( syncRoot )
                {
                    if ( !handlers.TryGetValue( eventName, out list ) )
                    {
                        handlers.Add( eventName, new List<WeakDelegate>() { item } );
                        return;
                    }
                }
            }

            lock ( list )
            {
                list.Add( item );
            }
        }

        /// <summary>
        /// Unsubscribes from the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> of event arguments to subscribe to.</typeparam>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        /// <param name="handler">The <see cref="Action{T1,T2,T3}">action</see> to perform when the evnet is raised.</param>
        public virtual void Unsubscribe<TEventArgs>( string eventName, Action<string, object, TEventArgs> handler ) where TEventArgs : class
        {
            Arg.NotNullOrEmpty( eventName, nameof( eventName ) );
            Arg.NotNull( handler, nameof( handler ) );

            var list = default( List<WeakDelegate> );

            lock ( syncRoot )
            {
                if ( !handlers.TryGetValue( eventName, out list ) )
                {
                    return;
                }
            }

            lock ( list )
            {
                var item = list.FirstOrDefault( i => i.IsMatch( handler ) );

                if ( item != null )
                {
                    list.Remove( item );
                }
            }
        }
    }
}