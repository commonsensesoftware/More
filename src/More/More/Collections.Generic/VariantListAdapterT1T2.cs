namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents an adapter class that makes the source <see cref="IList{T}">list</see> covariant and contravariant.
    /// </summary>
    /// <typeparam name="TFrom">The <see cref="Type">type</see> of item to make covariant.</typeparam>
    /// <typeparam name="TTo">The <see cref="Type">type</see> of contravariant item.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( CollectionDebugView<> ) )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is a specialized type that adapts to another list." )]
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a specialized type that adapts to another list." )]
    public class VariantListAdapter<TFrom, TTo> : IList<TTo> where TFrom : TTo
    {
        readonly IList<TFrom> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantListAdapter{TFrom,TTo}"/> class.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}">list</see> to enable type variance for.</param>
        public VariantListAdapter( IList<TFrom> list )
        {
            Arg.NotNull( list, nameof( list ) );
            items = list;
        }

        /// <summary>
        /// Gets the underlying list of covariant items.
        /// </summary>
        /// <value>The <see cref="IList{T}">list</see> of adapted items.</value>
        protected IList<TFrom> Items
        {
            get
            {
                Contract.Ensures( items != null );
                return items;
            }
        }

        /// <summary>
        /// Removes all of the items from the list.
        /// </summary>
        protected virtual void ClearItems()
        {
            Contract.Ensures( Items.Count == 0 );
            Items.Clear();
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The <typeparamref name="TFrom">item</typeparamref> to insert.</param>
        protected virtual void InsertItem( int index, TFrom item )
        {
            Contract.Ensures( Count == Contract.OldValue( Count ) + 1 );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThanOrEqualTo( index, Items.Count, nameof( index ) );
            Items.Insert( index, item );
        }

        /// <summary>
        /// Removes an item from the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected virtual void RemoveItem( int index )
        {
            Contract.Ensures( Count == Contract.OldValue( Count ) - 1 );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThanOrEqualTo( index, Items.Count, nameof( index ) );
            Items.RemoveAt( index );
        }

        /// <summary>
        /// Replaces an item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The replacement <typeparamref name="TFrom">item</typeparamref> to set.</param>
        protected virtual void SetItem( int index, TFrom item )
        {
            Contract.Ensures( Count == Contract.OldValue( Count ) );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThanOrEqualTo( index, Items.Count, nameof( index ) );
            Items[index] = item;
        }

        /// <summary>
        /// Moves an item from the specified old index to the new index.
        /// </summary>
        /// <param name="oldIndex">The zero-based index of the source item.</param>
        /// <param name="newIndex">The zero-based index of the destination in the list.</param>
        protected virtual void MoveItem( int oldIndex, int newIndex )
        {
            Arg.GreaterThanOrEqualTo( oldIndex, 0, nameof( oldIndex ) );
            Arg.LessThan( oldIndex, Items.Count, nameof( oldIndex ) );
            Arg.GreaterThanOrEqualTo( newIndex, 0, nameof( newIndex ) );
            Arg.LessThan( newIndex, Items.Count, nameof( newIndex ) );

            var item = Items[oldIndex];
            RemoveItem( oldIndex );
            InsertItem( newIndex, item );
        }

        /// <summary>
        /// Returns the zero-based index of the specified item in the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to get the index for.</param>
        /// <returns>The zero-based index of the specified <paramref name="item"/> or -1 if no match is found.</returns>
        public int IndexOf( TTo item ) => Items.IndexOf( (TFrom) item );

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to insert.</param>
        public void Insert( int index, TTo item ) => InsertItem( index, (TFrom) item );

        /// <summary>
        /// Removes an item from the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt( int index ) => RemoveItem( index );

        /// <summary>
        /// Gets or sets an item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get or set.</param>
        /// <returns>The <typeparamref name="TFrom">item</typeparamref> at the specified index</returns>
        public TTo this[int index]
        {
            get => Items[index];
            set => SetItem( index, (TFrom) value );
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to add.</param>
        public void Add( TTo item ) => InsertItem( Items.Count, (TFrom) item );

        /// <summary>
        /// Removes all of the items from the list.
        /// </summary>
        public void Clear() => ClearItems();

        /// <summary>
        /// Returns a value indicating whether the list contains the specified item.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to locate.</param>
        /// <returns>True if the list contains the specified <paramref name="item"/>; otherwise, false.</returns>
        public bool Contains( TTo item ) => Items.Contains( (TFrom) item );

        /// <summary>
        /// Copies the items in the list to an <see cref="Array">array</see>, starting at the specified <see cref="Array">array</see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the items copied
        /// from the list. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive" )]
        public void CopyTo( TTo[] array, int arrayIndex )
        {
            Arg.NotNull( array, nameof( array ) );
            var other = new TFrom[array.Length];
            Items.CopyTo( other, arrayIndex );
            other.Cast<TTo>().ToArray().CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the total number items in the list.
        /// </summary>
        /// <value>The number of items in the list.</value>
        public int Count => Items.Count;

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        /// <value>True if the list is read-only; otherwise, false.</value>
        public bool IsReadOnly => Items.IsReadOnly;

        /// <summary>
        /// Removes the specified item from the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to remove.</param>
        /// <returns>True if the <paramref name="item"/> was removed; otherwise, false.</returns>
        public bool Remove( TTo item )
        {
            var index = IndexOf( item );

            if ( index < 0 )
            {
                return false;
            }

            RemoveItem( index );
            return true;
        }

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the list.</returns>
        public virtual IEnumerator<TTo> GetEnumerator() => Items.Cast<TTo>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}