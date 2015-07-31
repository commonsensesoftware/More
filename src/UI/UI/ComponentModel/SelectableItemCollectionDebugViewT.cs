namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
            Arg.NotNull( collection, nameof( collection ) );
            source = collection;
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
                return source.ToArray();
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
                return source.SelectedItems.ToArray();
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
                return source.SelectedValues.ToArray();
            }
        }
    }
}
