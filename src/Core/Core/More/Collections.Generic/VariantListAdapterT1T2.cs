namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;    
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts; 
    using global::System.Linq;

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
        private readonly IList<TFrom> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantListAdapter{TFrom,TTo}"/> class.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}">list</see> to enable type variance for.</param>
        public VariantListAdapter( IList<TFrom> list )
        {
            Contract.Requires<ArgumentNullException>( list != null, "list" );
            this.items = list;
        }

        /// <summary>
        /// Gets the underlying list of covariant items.
        /// </summary>
        /// <value>The <see cref="IList{T}">list</see> of adapted items.</value>
        protected IList<TFrom> Items
        {
            get
            {
                Contract.Ensures( this.items != null );
                return this.items;
            }
        }

        /// <summary>
        /// Removes all of the items from the list.
        /// </summary>
        protected virtual void ClearItems()
        {
            Contract.Ensures( this.Items.Count == 0 ); 
            this.Items.Clear();
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The <typeparamref name="TFrom">item</typeparamref> to insert.</param>
        protected virtual void InsertItem( int index, TFrom item )
        {
            Contract.Requires<ArgumentOutOfRangeException>( index >= 0, "index" );
            Contract.Requires<ArgumentOutOfRangeException>( index <= this.Items.Count, "index" );
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) + 1 );
            this.Items.Insert( index, item );
        }

        /// <summary>
        /// Removes an item from the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected virtual void RemoveItem( int index )
        {
            Contract.Requires<ArgumentOutOfRangeException>( index >= 0, "index" );
            Contract.Requires<ArgumentOutOfRangeException>( index < this.Items.Count, "index" );
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) - 1 );
            this.Items.RemoveAt( index );
        }

        /// <summary>
        /// Replaces an item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The replacement <typeparamref name="TFrom">item</typeparamref> to set.</param>
        protected virtual void SetItem( int index, TFrom item )
        {
            Contract.Requires<ArgumentOutOfRangeException>( index >= 0, "index" );
            Contract.Requires<ArgumentOutOfRangeException>( index < this.Items.Count, "index" );
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) );
            this.Items[index] = item;
        }

        /// <summary>
        /// Moves an item from the specified old index to the new index.
        /// </summary>
        /// <param name="oldIndex">The zero-based index of the source item.</param>
        /// <param name="newIndex">The zero-based index of the destination in the list.</param>
        protected virtual void MoveItem( int oldIndex, int newIndex )
        {
            Contract.Requires<ArgumentOutOfRangeException>( oldIndex >= 0, "oldIndex" );
            Contract.Requires<ArgumentOutOfRangeException>( oldIndex < this.Items.Count, "oldIndex" );
            Contract.Requires<ArgumentOutOfRangeException>( newIndex >= 0, "newIndex" );
            Contract.Requires<ArgumentOutOfRangeException>( newIndex < this.Items.Count, "newIndex" );

            var item = this.Items[oldIndex];
            this.RemoveItem( oldIndex );
            this.InsertItem( newIndex, item );
        }

        /// <summary>
        /// Returns the zero-based index of the specified item in the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to get the index for.</param>
        /// <returns>The zero-based index of the specified <paramref name="item"/> or -1 if no match is found.</returns>
        public int IndexOf( TTo item )
        {
            return this.Items.IndexOf( (TFrom) item );
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to insert.</param>
        public void Insert( int index, TTo item )
        {
            this.InsertItem( index, (TFrom) item );
        }

        /// <summary>
        /// Removes an item from the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt( int index )
        {
            this.RemoveItem( index );
        }

        /// <summary>
        /// Gets or sets an item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get or set.</param>
        /// <returns>The <typeparamref name="TFrom">item</typeparamref> at the specified index</returns>
        public TTo this[int index]
        {
            get
            {
                return (TTo) this.Items[index];
            }
            set
            {
                this.SetItem( index, (TFrom) value );
            }
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to add.</param>
        public void Add( TTo item )
        {
            this.InsertItem( this.Items.Count, (TFrom) item );
        }

        /// <summary>
        /// Removes all of the items from the list.
        /// </summary>
        public void Clear()
        {
            this.ClearItems();
        }

        /// <summary>
        /// Returns a value indicating whether the list contains the specified item.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to locate.</param>
        /// <returns>True if the list contains the specified <paramref name="item"/>; otherwise, false.</returns>
        public bool Contains( TTo item )
        {
            return this.Items.Contains( (TFrom) item );
        }

        /// <summary>
        /// Copies the items in the list to an <see cref="Array">array</see>, starting at the specified <see cref="Array">array</see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the items copied
        /// from the list. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive" )]
        public void CopyTo( TTo[] array, int arrayIndex )
        {
            if ( array == null )
                throw new ArgumentNullException( "array" );

            var other = new TFrom[array.Length];
            this.Items.CopyTo( other, arrayIndex );
            other.Cast<TTo>().ToArray().CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the total number items in the list.
        /// </summary>
        /// <value>The number of items in the list.</value>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        /// <value>True if the list is read-only; otherwise, false.</value>
        public bool IsReadOnly
        {
            get
            {
                return this.Items.IsReadOnly;
            }
        }

        /// <summary>
        /// Removes the specified item from the list.
        /// </summary>
        /// <param name="item">The <typeparamref name="TTo">item</typeparamref> to remove.</param>
        /// <returns>True if the <paramref name="item"/> was removed; otherwise, false.</returns>
        public bool Remove( TTo item )
        {
            var index = this.IndexOf( item );

            if ( index < 0 )
                return false;

            this.RemoveItem( index );
            return true;
        }

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the list.</returns>
        public virtual IEnumerator<TTo> GetEnumerator()
        {
            return this.Items.Cast<TTo>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
