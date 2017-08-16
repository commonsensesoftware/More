namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using static FrozenItemPosition;
    using static System.Math;

    /// <summary>
    /// Represents an observable collection that supports freezing items to the start or end of the collection.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the collection.</typeparam>
    public class FrozenItemCollection<T> : ObservableCollection<T>
    {
        readonly ObservableCollection<T> frozenItems = new ObservableCollection<T>();
        readonly NonFrozenItemList unfrozenItems;
        FrozenItemPosition frozenItemPosition = End;

        /// <summary>
        /// Finalizes an instance of the <see cref="FrozenItemCollection{T}"/> class.
        /// </summary>
        ~FrozenItemCollection()
        {
            if ( frozenItems != null )
            {
                frozenItems.CollectionChanged -= OnFrozenItemsChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenItemCollection{T}"/> class.
        /// </summary>
        public FrozenItemCollection()
        {
            frozenItems.CollectionChanged += OnFrozenItemsChanged;
            unfrozenItems = new NonFrozenItemList( this );
        }

        /// <summary>
        /// Gets or sets the position of frozen items in the collection view.
        /// </summary>
        /// <value>One of the <see cref="FrozenItemPosition"/> values.</value>
        public FrozenItemPosition FrozenItemPosition
        {
            get => frozenItemPosition;
            set
            {
                if ( frozenItemPosition == value )
                {
                    return;
                }

                frozenItemPosition = value;
                OnPropertyChanged( "FrozenItemPosition" );
                RebuildCollection();
            }
        }

        /// <summary>
        /// Gets an observable collection of frozen items.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">observable collection</see> of frozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        public ObservableCollection<T> FrozenItems
        {
            get
            {
                Contract.Ensures( frozenItems != null );
                return frozenItems;
            }
        }

        /// <summary>
        /// Gets a read-only, observable collection of unfrozen items.
        /// </summary>
        /// <value>A read-only, <see cref="IList{T}">observable collection</see> of unfrozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        /// <remarks>The default <see cref="IList{T}">list</see> implementation also implements
        /// <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.</remarks>
        public IList<T> UnfrozenItems
        {
            get
            {
                Contract.Ensures( unfrozenItems != null );
                return unfrozenItems;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName ) =>
            OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

        /// <summary>
        /// Adjusts the specified index to the correct location within the collection.
        /// </summary>
        /// <param name="index">The ordinal, zero-based index of the requested location in the collection.</param>
        /// <returns>The adjusted, zero-based index of location in the collection, which accounts for frozen items.</returns>
        protected int AdjustIndex( int index )
        {
            // index can equal count during an insert
            Contract.Ensures( index >= 0 );
            Contract.Ensures( index <= Count );
            Arg.InRange( index, index, Count, nameof( index ) );

            if ( FrozenItemPosition == End )
            {
                return index - FrozenItems.Count;
            }

            return index;
        }

        void ProcessFrozenItemsFromBeginning( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        base.InsertItem( e.NewStartingIndex, (T) e.NewItems[0] );
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        base.RemoveItem( e.OldStartingIndex );
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        base.SetItem( e.NewStartingIndex, (T) e.NewItems[0] );
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        while ( Count > UnfrozenItems.Count )
                        {
                            RemoveAt( 0 );
                        }

                        break;
                    }
            }
        }

        void ProcessFrozenItemsFromEnd( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            var offset = Max( Count - FrozenItems.Count, 0 );

            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var count = e.NewItems == null ? 0 : e.NewItems.Count;
                        var index = offset + e.NewStartingIndex;

                        for ( var i = 0; i < count; i++ )
                        {
                            base.InsertItem( index++, (T) e.NewItems[i] );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var count = e.OldItems == null ? 0 : e.OldItems.Count;
                        var index = Max( ( offset - count ) + e.OldStartingIndex, 0 );

                        for ( var i = 0; i < count; i++ )
                        {
                            base.RemoveItem( index );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var count = e.NewItems == null ? 0 : e.NewItems.Count;
                        var index = offset + e.NewStartingIndex;

                        for ( var i = 0; i < count; i++ )
                        {
                            base.SetItem( index++, (T) e.NewItems[i] );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        while ( Count > UnfrozenItems.Count )
                        {
                            RemoveAt( Count - 1 );
                        }

                        break;
                    }
            }
        }

        void RebuildCollection()
        {
            var items = default( IEnumerable<T> );

            switch ( FrozenItemPosition )
            {
                case Beginning:
                    {
                        items = FrozenItems.Union( this.Take( Count ) ).ToList();
                        break;
                    }
                case End:
                    {
                        items = this.Skip( FrozenItems.Count ).Union( FrozenItems ).ToList();
                        break;
                    }
                default:
                    {
                        items = Items.ToList();
                        break;
                    }
            }

            base.ClearItems();
            items.ForEach( i => base.InsertItem( Count, i ) );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        void OnFrozenItemsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            switch ( FrozenItemPosition )
            {
                case Beginning:
                    {
                        ProcessFrozenItemsFromBeginning( e );
                        break;
                    }
                case End:
                    {
                        ProcessFrozenItemsFromEnd( e );
                        break;
                    }
            }
        }

        /// <summary>
        /// Overrides the default behavior when the collection is cleared.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            FrozenItems.ForEach( i => base.InsertItem( Count, i ) );
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted into the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="item">The <typeparamref name="T">item</typeparamref> to insert.</param>
        protected override void InsertItem( int index, T item )
        {
            var adjustedIndex = AdjustIndex( index );
            base.InsertItem( adjustedIndex, item );
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected override void RemoveItem( int index )
        {
            var adjustedIndex = AdjustIndex( index );
            base.RemoveItem( adjustedIndex );
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced in the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The replacement <typeparamref name="T">item</typeparamref>.</param>
        protected override void SetItem( int index, T item )
        {
            var adjustedIndex = AdjustIndex( index );
            base.SetItem( adjustedIndex, item );
        }

        sealed class NonFrozenItemList : IList<T>, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            readonly FrozenItemCollection<T> owner;

            ~NonFrozenItemList()
            {
                if ( owner != null )
                {
                    owner.CollectionChanged -= OnBubbleCollectionChanged;
                }
            }

            internal NonFrozenItemList( FrozenItemCollection<T> owner )
            {
                Contract.Requires( owner != null );

                this.owner = owner;
                this.owner.CollectionChanged += OnBubbleCollectionChanged;
            }
            int ToActualIndex( int index )
            {
                // index can equal count during an insert
                Contract.Requires( index >= 0 );
                Contract.Requires( index <= Count );
                Contract.Ensures( index >= 0 );
                Contract.Ensures( index <= Count );

                if ( owner.FrozenItemPosition == Beginning )
                {
                    return index + owner.FrozenItems.Count;
                }

                return index;
            }

            int ToRelativeIndex( int index )
            {
                // index can equal count during an insert
                Contract.Requires( index >= 0 );
                Contract.Requires( index <= owner.Count );
                Contract.Ensures( index >= 0 );
                Contract.Ensures( index <= owner.Count );

                if ( owner.FrozenItemPosition == Beginning )
                {
                    return Max( index - owner.FrozenItems.Count, 0 );
                }

                return index;
            }

            void OnPropertyChanged( string propertyName ) =>
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

            void OnCollectionChanged( NotifyCollectionChangedAction action ) =>
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( action ) );

            void OnCollectionChanged( NotifyCollectionChangedAction action, T item, int index ) =>
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( action, item, index ) );

            void OnCollectionChanged( NotifyCollectionChangedAction action, T newItem, T oldItem, int index ) =>
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( action, newItem, oldItem, index ) );

            void OnCollectionChanged( NotifyCollectionChangedEventArgs e ) => CollectionChanged?.Invoke( this, e );

            void OnOwnerItemsAdded( NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( e != null );

                var index = ToRelativeIndex( e.NewStartingIndex );

                if ( index >= Count || e.NewItems == null )
                {
                    return;
                }

                OnPropertyChanged( "Count" );
                OnPropertyChanged( "Item[]" );
                var args = new NotifyCollectionChangedEventArgs( e.Action, e.NewItems, index );
                OnCollectionChanged( args );
            }

            void OnOwnerItemsRemoved( NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( e != null );

                var index = ToRelativeIndex( e.OldStartingIndex );

                if ( e.OldItems == null || index > ( Count + e.OldItems.Count ) )
                {
                    return;
                }

                OnPropertyChanged( "Count" );
                OnPropertyChanged( "Item[]" );
                var args = new NotifyCollectionChangedEventArgs( e.Action, e.OldItems, index );
                OnCollectionChanged( args );
            }

            void OnOwnerItemsReplaced( NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( e != null );

                var index = ToRelativeIndex( e.NewStartingIndex );

                if ( index > Count || e.NewItems == null || e.OldItems == null || e.NewItems.Count != e.OldItems.Count )
                {
                    return;
                }

                OnPropertyChanged( "Item[]" );
                var args = new NotifyCollectionChangedEventArgs( e.Action, e.NewItems, e.OldItems, index );
                OnCollectionChanged( args );
            }

            void OnBubbleCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( e != null );

                switch ( e.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            OnOwnerItemsAdded( e );
                            break;
                        }
                    case NotifyCollectionChangedAction.Move:
                        {
                            OnPropertyChanged( "Item[]" );
                            OnCollectionChanged( e );
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            OnOwnerItemsRemoved( e );
                            break;
                        }

                    case NotifyCollectionChangedAction.Replace:
                        {
                            OnOwnerItemsReplaced( e );
                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            OnPropertyChanged( "Count" );
                            OnPropertyChanged( "Item[]" );
                            OnCollectionChanged( e );
                            break;
                        }
                }
            }

            public int IndexOf( T item )
            {
                var ordinalIndex = owner.IndexOf( item );
                var index = ordinalIndex < 0 ? ordinalIndex : ToRelativeIndex( ordinalIndex );
                return index;
            }

            public void Insert( int index, T item ) =>
                throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );

            public void RemoveAt( int index ) =>
                throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );

            public T this[int index]
            {
                get => owner[ToActualIndex( index )];
                set => throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );
            }

            T IReadOnlyList<T>.this[int index] => this[index];

            public void Add( T item ) =>
                throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );

            public void Clear() => throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );

            public bool Contains( T item )
            {
                var index = IndexOf( item );
                var found = index >= 0 && index < Count;
                return found;
            }

            [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by base implementation." )]
            public void CopyTo( T[] array, int arrayIndex ) => this.ToArray().CopyTo( array, arrayIndex );

            public int Count => Max( owner.Count - owner.FrozenItems.Count, 0 );

            public bool IsReadOnly => true;

            public bool Remove( T item ) => throw new NotSupportedException( ExceptionMessage.ReadOnlyCollection );

            public IEnumerator<T> GetEnumerator()
            {
                if ( owner.FrozenItemPosition == End )
                {
                    return owner.Take( Count ).GetEnumerator();
                }

                return owner.Skip( owner.FrozenItems.Count ).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}