namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IMultivalueDictionary<,> ) )]
    internal abstract class IMultivalueDictionaryContract<TKey, TValue> : IMultivalueDictionary<TKey, TValue>
    {
        int IMultivalueDictionary<TKey, TValue>.IndexOf( TKey key )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            Contract.Ensures( Contract.Result<int>() >= -1 );
            return default( int );
        }

        void IMultivalueDictionary<TKey, TValue>.AddRange( TKey key, IEnumerable<TValue> values )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            Contract.Requires<ArgumentNullException>( values != null, "values" );
        }

        void IMultivalueDictionary<TKey, TValue>.Add( TKey key, TValue value )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
        }

        int IMultivalueDictionary<TKey, TValue>.RemoveRange( TKey key, IEnumerable<TValue> values )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            Contract.Requires<ArgumentNullException>( values != null, "values" );
            Contract.Ensures( Contract.Result<int>() >= 0 );
            return default( int );
        }

        int IMultivalueDictionary<TKey, TValue>.RemoveRange( IEnumerable<TKey> keys )
        {
            Contract.Requires<ArgumentNullException>( keys != null, "keys" );
            Contract.Ensures( Contract.Result<int>() >= 0 );
            return default( int );
        }

        bool IMultivalueDictionary<TKey, TValue>.Remove( TKey key, TValue value )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            return default( bool );
        }

        bool IMultivalueDictionary<TKey, TValue>.Set( TKey key, TValue value )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            return default( bool );
        }

        bool IMultivalueDictionary<TKey, TValue>.SetRange( TKey key, IEnumerable<TValue> values )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            Contract.Requires<ArgumentNullException>( values != null, "values" );
            return default( bool );
        }

        bool IMultivalueDictionary<TKey, TValue>.Contains( TKey key, TValue value )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            return default( bool );
        }

        int IMultivalueDictionary<TKey, TValue>.CountValues( TKey key )
        {
            Contract.Requires<ArgumentNullException>( key != null, "key" );
            Contract.Ensures( Contract.Result<int>() >= 0 );
            return default( int );
        }

        int IMultivalueDictionary<TKey, TValue>.CountAllValues()
        {
            Contract.Ensures( Contract.Result<int>() >= 0 );
            return default( int );
        }

        void IDictionary<TKey, ICollection<TValue>>.Add( TKey key, ICollection<TValue> value )
        {
        }

        bool IDictionary<TKey, ICollection<TValue>>.ContainsKey( TKey key )
        {
            return default( bool );
        }

        ICollection<TKey> IDictionary<TKey, ICollection<TValue>>.Keys
        {
            get
            {
                return default( ICollection<TKey> );
            }
        }

        bool IDictionary<TKey, ICollection<TValue>>.Remove( TKey key )
        {
            return default( bool );
        }

        bool IDictionary<TKey, ICollection<TValue>>.TryGetValue( TKey key, out ICollection<TValue> value )
        {
            value = default( ICollection<TValue> );
            return default( bool );
        }

        ICollection<ICollection<TValue>> IDictionary<TKey, ICollection<TValue>>.Values
        {
            get
            {
                return default( ICollection<ICollection<TValue>> );
            }
        }

        ICollection<TValue> IDictionary<TKey, ICollection<TValue>>.this[TKey key]
        {
            get
            {
                return default( ICollection<TValue> );
            }
            set
            {
            }
        }

        void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Add( KeyValuePair<TKey, ICollection<TValue>> item )
        {
        }

        void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Clear()
        {
        }

        bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Contains( KeyValuePair<TKey, ICollection<TValue>> item )
        {
            return default( bool );
        }

        void ICollection<KeyValuePair<TKey, ICollection<TValue>>>.CopyTo( KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex )
        {
        }

        int ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Count
        {
            get
            {
                return default( int );
            }
        }

        bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.IsReadOnly
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollection<KeyValuePair<TKey, ICollection<TValue>>>.Remove( KeyValuePair<TKey, ICollection<TValue>> item )
        {
            return default( bool );
        }

        IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>.GetEnumerator()
        {
            return default( IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
}
