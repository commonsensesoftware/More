namespace More
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq;
    using global::System.Reflection;
    using global::System.Threading;

    /// <summary>
    /// Represents a class that can manage weak event handler references.
    /// </summary>
    /// <typeparam name="THandler">The <see cref="Type">type</see> of event <see cref="Delegate">delegate</see>.</typeparam>
    /// <typeparam name="TArgs">The <see cref="Type">type</see> of event arguments.</typeparam>
    /// <remarks>The <typeparamref name="TArgs"/> type constraint is not restricted to the <see cref="EventArgs"/> class because the
    /// event handlers for some implementations have event arguments that do not inherit from <see cref="EventArgs"/>; for example,
    /// routed events on some platforms.</remarks>
    [SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This class manages weak event handlers with thread synchronization support. Uses of this class are typically long running and do not align well with the prescribed dispose pattern. This class ensures the proper release of locks without requiring consumer to implement IDisposable as well." )]
    public class WeakEventManager<THandler, TArgs>
        where THandler : class
        where TArgs : class
    {
        private readonly object syncRoot = new object();
        private readonly List<WeakDelegate<THandler>> handlers = new List<WeakDelegate<THandler>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventManager{THandler,TArgs}"/> class.
        /// </summary>
        public WeakEventManager()
        {
            if ( !WeakDelegate.IsDelegateType( typeof( THandler ) ) )
                throw new InvalidOperationException( ExceptionMessage.InvalidDelegateType.FormatDefault( typeof( THandler ) ) );
        }

        /// <summary>
        /// Adds a handler for the managed event.
        /// </summary>
        /// <param name="handler">The <typeparamref name="THandler">delegate</typeparamref> representing handler to add.</param>
        public void AddHandler( THandler handler )
        {
            if ( handler != null )
            {
                lock ( this.syncRoot )
                    this.handlers.Add( new WeakDelegate<THandler>( handler ) );
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
                return false;

            lock ( this.syncRoot )
            {
                var item = this.handlers.FirstOrDefault( h => h.IsMatch( handler ) );

                if ( item != null )
                    return this.handlers.Remove( item );
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
            lock ( this.syncRoot )
            {
                for ( var i = this.handlers.Count - 1; i > -1; i-- )
                {
                    // create strong delegate from handler
                    var handler = this.handlers[i].CreateDelegate();

                    // if the handler is dead, remove it; otherwise, invoke it
                    if ( handler == null )
                        this.handlers.RemoveAt( i );
                    else
                        handler.DynamicInvoke( source, e );
                }
            }
        }
    }
}
