namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a debugging class used to visualize an instance of the <see cref="IDictionary{TKey,TValue}"/> class.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of values in the dictionary.</typeparam>
    public sealed class DictionaryDebugView<TKey, TValue>
    {
        readonly IDictionary<TKey, TValue> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDebugView{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> instance to debug.</param>
        public DictionaryDebugView( IDictionary<TKey, TValue> dictionary )
        {
            Arg.NotNull( dictionary, nameof( dictionary ) );
            source = dictionary;
        }

        /// <summary>
        /// Gets the debugger view of a specific <see cref="IDictionary{TKey,TValue}"/> instance.
        /// </summary>
        /// <value>The debugger view of a specific <see cref="IDictionary{TKey,TValue}"/> instance.</value>
        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                Contract.Ensures( source != null );
                return source.ToArray();
            }
        }
    }
}