namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;

    /// <summary>
    /// Represents debugging class used to visualize an instance of the <see cref="SelectableItemCollectionDebugView{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of items in the collection.</typeparam>
    public sealed class SelectableItemCollectionDebugView<T>
    {
        private readonly SelectableItemCollection<T> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollectionDebugView{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="SelectableItemCollection{T}">collection</see> to debug.</param>
        public SelectableItemCollectionDebugView( SelectableItemCollection<T> collection )
        {
            Contract.Requires<ArgumentNullException>( collection != null, "collection" );
            this.source = collection;
        }

        /// <summary>
        /// Gets the debugger view of the items in the collection.
        /// </summary>
        /// <value>The debugger view of the items in the collection.</value>
        [DebuggerDisplay( @"Count = {Items.Length}" )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public SelectableItem<T>[] Items
        {
            get
            {
                Contract.Ensures( Contract.Result<SelectableItem<T>[]>() != null );
                return this.source.ToArray();
            }
        }

        /// <summary>
        /// Gets the debugger view of the selected items in the collection.
        /// </summary>
        /// <value>The debugger view of the selected items in the collection.</value>
        [DebuggerDisplay( @"Count = {SelectedItems.Length}" )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public SelectableItem<T>[] SelectedItems
        {
            get
            {
                Contract.Ensures( Contract.Result<SelectableItem<T>[]>() != null );
                return this.source.SelectedItems.ToArray();
            }
        }

        /// <summary>
        /// Gets the debugger view of the selected values in the collection.
        /// </summary>
        /// <value>The debugger view of the selected values in the collection.</value>
        [DebuggerDisplay( @"Count = {SelectedValues.Length}" )]
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is the convention for the debugger visualization system." )]
        public T[] SelectedValues
        {
            get
            {
                Contract.Ensures( Contract.Result<T[]>() != null );
                return this.source.SelectedValues.ToArray();
            }
        }
    }
}
