namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents a read-only dictionary.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( DictionaryDebugView<,> ) )]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly IDictionary<TKey, TValue> dict;
        object syncRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}">dictionary</see> to make read-only.</param>
        public ReadOnlyDictionary( IDictionary<TKey, TValue> dictionary )
        {
            Arg.NotNull( dictionary, nameof( dictionary ) );
            dict = dictionary;

            if ( dictionary is INotifyPropertyChanged propertyChanged )
            {
                propertyChanged.PropertyChanged += SourcePropertyChanged;
            }

            if ( dictionary is INotifyCollectionChanged collectionChanged )
            {
                collectionChanged.CollectionChanged += ( s, e ) => OnCollectionChanged( e );
            }
        }

        /// <summary>
        /// Gets the dictionary that this <see cref="ReadOnlyDictionary{TKey,TValue}"/> wraps.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
        protected IDictionary<TKey, TValue> Dictionary
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
                return dict;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a
        /// <see cref="KeyNotFoundException"/>.</returns>
        public TValue this[TKey key] => Dictionary[key];

        void SourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            switch ( e.PropertyName )
            {
                case "Count":
                case "Item[]":
                    {
                        OnPropertyChanged( e );
                        break;
                    }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

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
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CollectionChanged?.Invoke( this, e );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary<TKey, TValue>.Add( TKey key, TValue value ) => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool ContainsKey( TKey key ) => Dictionary.ContainsKey( key );

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey( TKey key ) => ContainsKey( key );

        /// <summary>
        /// Gets a collection containing the keys of the dictionary.
        /// </summary>
        /// <value>An <see cref="ICollection{TKey}"/> object.</value>
        public ICollection<TKey> Keys => Dictionary.Keys.IsReadOnly ? Dictionary.Keys : Dictionary.Keys.ToArray();

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary<TKey, TValue>.Remove( TKey key ) => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key
        /// is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>True if the dictioanry contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue( TKey key, out TValue value ) => Dictionary.TryGetValue( key, out value );

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue( TKey key, out TValue value ) => TryGetValue( key, out value );

        /// <summary>
        /// Gets a collection containing the values of the dictionary.
        /// </summary>
        /// <value>An <see cref="ICollection{TValue}"/> object.</value>
        public ICollection<TValue> Values => Dictionary.Values.IsReadOnly ? Dictionary.Values : Dictionary.Values.ToArray();

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => Dictionary[key];
            set => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<KeyValuePair<TKey, TValue>>.Add( KeyValuePair<TKey, TValue> item ) =>
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        /// <summary>
        /// Determines whether the dictionry contains the specified value.
        /// </summary>
        /// <param name="item">The object to locate in the dictionary.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains( KeyValuePair<TKey, TValue> item )
        {
            Contract.Assume( false || Count > 0 );
            return Dictionary.Contains( item );
        }

        /// <summary>
        /// Copies the elements of the dictionary to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
        {
            Contract.Assume( arrayIndex + Dictionary.Count <= array.Length );
            Dictionary.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the dictionary.
        /// </summary>
        /// <value>The number of key/value pairs in the dictionary.</value>
        public int Count => Dictionary.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary{TKey,TValue}"/> object is read-only.
        /// </summary>
        /// <value><see langkeyword="true">True</see> if the <see cref="IDictionary{TKey,TValue}"/> object is read-only; otherwise, false.</value>
        public bool IsReadOnly => true;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove( KeyValuePair<TKey, TValue> item ) =>
            throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{KeyValuePair}"/> object.</returns>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Add( object key, object value ) => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Clear() => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary.Contains( object key )
        {
            if ( key is TKey )
            {
                return ContainsKey( (TKey) key );
            }

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        IDictionaryEnumerator IDictionary.GetEnumerator() => new Dictionary<TKey, TValue>( Dictionary ).GetEnumerator();

        // NOTE: there's not way to know for sure whether the underlying dictionary is read-only.  we'll assume it's not and that we'll receive
        // appropriate events via INotifyCollectionChnaged interface on the underlying dictionary.
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        bool IDictionary.IsFixedSize => Dictionary is IDictionary idict ? idict.IsFixedSize : false;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        ICollection IDictionary.Keys => Dictionary.Keys.ToArray();

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        void IDictionary.Remove( object key ) => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        ICollection IDictionary.Values => Dictionary.Values.ToArray();

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The collection is read-only." )]
        object IDictionary.this[object key]
        {
            get => key is TKey k ? this[k] : default( TValue );
            set => throw new NotSupportedException( ExceptionMessage.DictionaryReadOnly );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        void ICollection.CopyTo( Array array, int index )
        {
            if ( Dictionary is ICollection col )
            {
                Contract.Assume( index <= array.Length + col.Count );
                col.CopyTo( array, index );
            }
            else
            {
                var items = array as KeyValuePair<TKey, TValue>[];

                if ( items == null )
                {
                    throw new ArrayTypeMismatchException( ExceptionMessage.ArrayMismatch );
                }

                Contract.Assume( index + Count <= items.Length );
                CopyTo( items, index );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        bool ICollection.IsSynchronized => false;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Unexposed legacy support." )]
        object ICollection.SyncRoot
        {
            get
            {
                if ( syncRoot == null )
                {
                    if ( Dictionary is ICollection col )
                    {
                        syncRoot = col.SyncRoot;
                    }

                    Interlocked.CompareExchange( ref syncRoot, new object(), null );
                }

                Contract.Assume( syncRoot != null );
                return syncRoot;
            }
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}