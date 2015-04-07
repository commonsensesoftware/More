namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents a debugging class used to visualize an instance of the <see cref="IStack{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the <see cref="IStack{T}">stack</see>.</typeparam>
    public sealed class StackDebugView<T>
    {
        private readonly IStack<T> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackDebugView{T}"/> class.
        /// </summary>
        /// <param name="stack">The <see cref="IStack{T}"/> instance to debug.</param>
        public StackDebugView( IStack<T> stack )
        {
            Contract.Requires<ArgumentNullException>( stack != null, "stack" );
            this.stack = stack;
        }

        /// <summary>
        /// Gets the debugger view of a specific <see cref="IStack{T}"/> instance.
        /// </summary>
        /// <value>The debugger view of a specific <see cref="IStack{T}"/> instance.</value>
        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public T[] Items
        {
            get
            {
                Contract.Ensures( this.stack != null );
                return this.stack.ToArray();
            }
        }
    }
}
