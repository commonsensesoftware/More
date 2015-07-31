namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a concrete, read-only implementation of the <see cref="KeyedCollection{TKey,TItem}"/> class.
    /// </summary>
    /// <remarks>This type also supports change notification.</remarks>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of the keys.</typeparam>
    /// <typeparam name="TItem">The of <see cref="Type">type</see> items associated with the keys.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( CollectionDebugView<> ) )]
    public class ReadOnlyObservableKeyedCollection<TKey, TItem> : IList<TItem>, IReadOnlyList<TItem>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly KeyedCollection<TKey, TItem> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableKeyedCollection{TKey,TItem}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="KeyedCollection{TKey,TItem}">collection</see> to make read-only.</param>
        public ReadOnlyObservableKeyedCollection( KeyedCollection<TKey, TItem> collection )
        {
            Arg.NotNull( collection, nameof( collection ) );

            items = collection;

            var notifyProperty = collection as INotifyPropertyChanged;
            var notifyCollection = collection as INotifyCollectionChanged;

            if ( notifyProperty != null )
                notifyProperty.PropertyChanged += ( s, e ) => OnPropertyChanged( e );

            if ( notifyCollection != null )
                notifyCollection.CollectionChanged += ( s, e ) => OnCollectionChanged( e );
        }

        /// <summary>
        /// Gets the underlying items for the collection.
        /// </summary>
        /// <value>A <see cref="KeyedCollection{TKey,TItem}"/> object.</value>
        protected KeyedCollection<TKey, TItem> Items
        {
            get
            {
                Contract.Ensures( items != null ); 
                return items;
            }
        }

        /// <summary>
        /// Gets the item in the collection as the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to retrieve.</param>
        /// <returns>A <typeparamref name="TItem"/> object.</returns>
        public TItem this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Gets the item in the collection with the specified key.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey">key</typeparamref> of the item to return.</param>
        /// <returns>A <typeparamref name="TItem"/> object.</returns>
        public TItem this[TKey key]
        {
            get
            {
                return Items[key];
            }
        }

        private static bool IsCompatibleObject( object item )
        {
            return ( item is TItem ) || ( default( TItem ) != null && item == null );
        }

        /// <summary>
        /// Returns a value indicating whether the collection contains an item with the specified key.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey">key</typeparamref> of the item to evaluate.</param>
        /// <returns>True if the collection contains an item with the specified key; otherwise, false.</returns>
        public bool Contains( TKey key )
        {
            return Items.Contains( key );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            PropertyChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langkeyword="null">null</see>.</exception>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            CollectionChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Returns the index of the specified item in the collection.
        /// </summary>
        /// <param name="item">The <typeparamref name="TItem">item</typeparamref> in the collection to locate.</param>
        /// <returns>The zero-based item in the collection or -1 if no match is found.</returns>
        public int IndexOf( TItem item )
        {
            return Items.IndexOf( item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IList<TItem>.Insert( int index, TItem item )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IList<TItem>.RemoveAt( int index )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        TItem IList<TItem>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<TItem>.Add( TItem item )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<TItem>.Clear()
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        /// <summary>
        /// Returns a value indicating whether the collection contains the specified item.
        /// </summary>
        /// <param name="item">The <typeparamref name="TItem">item</typeparamref> to evaluate.</param>
        /// <returns>True if the collection contains the specified item; otherwise, false.</returns>
        public bool Contains( TItem item )
        {
            return Items.Contains( item );
        }

        /// <summary>
        /// Copies the items in the collection to the specified array.
        /// </summary>
        /// <param name="array">The array of type <typeparamref name="TItem"/> to copy the items into.</param>
        /// <param name="arrayIndex">The zero-based index where copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by the base implementation." )]
        public void CopyTo( TItem[] array, int arrayIndex )
        {
            Items.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the total number of items in the collection.
        /// </summary>
        /// <value>The total number of items in the collection.</value>
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool ICollection<TItem>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool ICollection<TItem>.Remove( TItem item )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        /// <summary>
        /// Returns an enumerator for the collection. 
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        public virtual IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        int IList.Add( object value )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        void IList.Clear()
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        bool IList.Contains( object value )
        {
            return IsCompatibleObject( value ) && Contains( (TItem) value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        int IList.IndexOf( object value )
        {
            return IsCompatibleObject( value ) ? IndexOf( (TItem) value ) : -1;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        void IList.Insert( int index, object value )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        bool IList.IsFixedSize
        {
            get
            {
                return ( (IList) Items ).IsFixedSize;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        bool IList.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        void IList.Remove( object value )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        void IList.RemoveAt( int index )
        {
            throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException( ExceptionMessage.CollectionReadOnly );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by the base implementation." )]
        void ICollection.CopyTo( Array array, int index )
        {
            ( (ICollection) Items ).CopyTo( array, index );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        bool ICollection.IsSynchronized
        {
            get
            {
                return ( (ICollection) Items ).IsSynchronized;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is for legacy support." )]
        object ICollection.SyncRoot
        {
            get
            {
                return ( (ICollection) Items ).SyncRoot;
            }
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either <c>null</c>or <see cref="F:String.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <remarks>The event handler receives an argument of type <see cref="NotifyCollectionChangedEventArgs"/>, which contains data that is related to this event.</remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
