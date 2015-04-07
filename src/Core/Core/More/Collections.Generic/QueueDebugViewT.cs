namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents a debugging class used to visualize an instance of the <see cref="IQueue{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the collection.</typeparam>
    public sealed class QueueDebugView<T>
    {
        private readonly IQueue<T> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueDebugView{T}"/> class.
        /// </summary>
        /// <param name="queue">The <see cref="IQueue{T}"/> instance to debug.</param>
        public QueueDebugView( IQueue<T> queue )
        {
            Contract.Requires<ArgumentNullException>( queue != null, "queue" );
            this.queue = queue;
        }

        /// <summary>
        /// Gets the debugger view of a specific <see cref="IQueue{T}"/> instance.
        /// </summary>
        /// <value>The debugger view of a specific <see cref="IQueue{T}"/> instance.</value>
        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public T[] Items
        {
            get
            {
                Contract.Ensures( Contract.Result<T[]>() != null );
                Contract.Assume( this.queue != null );
                return this.queue.ToArray();
            }
        }
    }
}
