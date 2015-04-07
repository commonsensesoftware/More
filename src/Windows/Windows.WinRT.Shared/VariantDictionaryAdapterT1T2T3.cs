namespace More
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal sealed class VariantDictionaryAdapter<TKey, TFrom, TTo> : IDictionary<TKey, TTo> where TFrom : TTo
    {
        private readonly IDictionary<TKey, TFrom> adapted;

        internal VariantDictionaryAdapter( IDictionary<TKey, TFrom> dictionary )
        {
            Contract.Requires( dictionary != null );
            this.adapted = dictionary;
        }

        public void Add( TKey key, TTo value )
        {
            this.adapted.Add( key, (TFrom) value );
        }

        public bool ContainsKey( TKey key )
        {
            return this.adapted.ContainsKey( key );
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.adapted.Keys;
            }
        }

        public bool Remove( TKey key )
        {
            return this.adapted.Remove( key );
        }

        public bool TryGetValue( TKey key, out TTo value )
        {
            value = default( TTo );

            TFrom temp;

            if ( !this.adapted.TryGetValue( key, out temp ) )
                return false;

            value = (TTo) temp;
            return true;
        }

        public ICollection<TTo> Values
        {
            get
            {
                return this.adapted.Values.Cast<TTo>().ToArray();
            }
        }

        public TTo this[TKey key]
        {
            get
            {
                return this.adapted[key];
            }
            set
            {
                this.adapted[key] = (TFrom) value;
            }
        }

        public void Add( KeyValuePair<TKey, TTo> item )
        {
            this.adapted.Add( new KeyValuePair<TKey, TFrom>( item.Key, (TFrom) item.Value ) );
        }

        public void Clear()
        {
            this.adapted.Clear();
        }

        public bool Contains( KeyValuePair<TKey, TTo> item )
        {
            return this.adapted.ContainsKey( item.Key );
        }

        public void CopyTo( KeyValuePair<TKey, TTo>[] array, int arrayIndex )
        {
            if ( array == null )
                throw new ArgumentNullException( "array" );

            var temp = new KeyValuePair<TKey, TFrom>[array.Length];
            this.adapted.CopyTo( temp, arrayIndex );
            temp.Cast<TTo>().ToArray().CopyTo( array, arrayIndex );
        }

        public int Count
        {
            get
            {
                return this.adapted.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.adapted.IsReadOnly;
            }
        }

        public bool Remove( KeyValuePair<TKey, TTo> item )
        {
            return this.adapted.Remove( item.Key );
        }

        public IEnumerator<KeyValuePair<TKey, TTo>> GetEnumerator()
        {
            return this.adapted.Select( p => new KeyValuePair<TKey, TTo>( p.Key, p.Value ) ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
