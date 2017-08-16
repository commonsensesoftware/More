namespace More
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Represents a class that can manage weak event handler references.
    /// </summary>
    /// <typeparam name="THandler">The <see cref="Type">type</see> of event <see cref="Delegate">delegate</see>.</typeparam>
    /// <typeparam name="TArgs">The <see cref="Type">type</see> of event arguments.</typeparam>
    /// <remarks>The <typeparamref name="TArgs"/> type constraint is not restricted to the <see cref="EventArgs"/> class because the
    /// event handlers for some implementations have event arguments that do not inherit from <see cref="EventArgs"/>; for example,
    /// routed events on some platforms.</remarks>
    public class WeakEventManager<THandler, TArgs>
        where THandler : class
        where TArgs : class
    {
        readonly object syncRoot = new object();
        readonly List<WeakDelegate<THandler>> handlers = new List<WeakDelegate<THandler>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventManager{THandler,TArgs}"/> class.
        /// </summary>
        public WeakEventManager()
        {
            if ( !WeakDelegate.IsDelegateType( typeof( THandler ) ) )
            {
                throw new InvalidOperationException( ExceptionMessage.InvalidDelegateType.FormatDefault( typeof( THandler ) ) );
            }
        }

        /// <summary>
        /// Adds a handler for the managed event.
        /// </summary>
        /// <param name="handler">The <typeparamref name="THandler">delegate</typeparamref> representing handler to add.</param>
        public void AddHandler( THandler handler )
        {
            if ( handler != null )
            {
                lock ( syncRoot )
                {
                    handlers.Add( new WeakDelegate<THandler>( handler ) );
                }
            }
        }

        /// <summary>
        /// Removes a handler for the managed event.
        /// </summary>
        /// <param name="handler">The <typeparamref name="THandler">delegate</typeparamref> representing handler to remove.</param>
        /// <returns>True if the handler is removed; otherwise, false.</returns>
        public bool RemoveHandler( THandler handler )
        {
            if ( handler == null )
            {
                return false;
            }

            lock ( syncRoot )
            {
                var item = handlers.FirstOrDefault( h => h.IsMatch( handler ) );

                if ( item != null )
                {
                    return handlers.Remove( item );
                }
            }

            return false;
        }

        /// <summary>
        /// Raises the managed event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <typeparamref name="TArgs">event args</typeparamref>.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Purposefully named to match the .NET events conventions. REF: http://msdn.microsoft.com/en-us/library/edzehd2t(v=vs.110).aspx" )]
        [SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Raises an event using the weak event pattern." )]
        public void RaiseEvent( object source, TArgs e )
        {
            lock ( syncRoot )
            {
                for ( var i = handlers.Count - 1; i > -1; i-- )
                {
                    var handler = handlers[i].CreateDelegate();

                    if ( handler == null )
                    {
                        handlers.RemoveAt( i );
                    }
                    else
                    {
                        handler.DynamicInvoke( source, e );
                    }
                }
            }
        }
    }
}