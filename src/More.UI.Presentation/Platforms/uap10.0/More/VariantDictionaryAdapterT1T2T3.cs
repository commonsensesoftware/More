namespace More
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    sealed class VariantDictionaryAdapter<TKey, TFrom, TTo> : IDictionary<TKey, TTo> where TFrom : TTo
    {
        readonly IDictionary<TKey, TFrom> adapted;

        internal VariantDictionaryAdapter( IDictionary<TKey, TFrom> dictionary ) => adapted = dictionary;

        public void Add( TKey key, TTo value ) => adapted.Add( key, (TFrom) value );

        public bool ContainsKey( TKey key ) => adapted.ContainsKey( key );

        public ICollection<TKey> Keys => adapted.Keys;

        public bool Remove( TKey key ) => adapted.Remove( key );

        public bool TryGetValue( TKey key, out TTo value )
        {
            value = default( TTo );

            if ( !adapted.TryGetValue( key, out var temp ) )
            {
                return false;
            }

            value = (TTo) temp;
            return true;
        }

        public ICollection<TTo> Values => adapted.Values.Cast<TTo>().ToArray();

        public TTo this[TKey key]
        {
            get => adapted[key];
            set => adapted[key] = (TFrom) value;
        }

        public void Add( KeyValuePair<TKey, TTo> item ) => adapted.Add( new KeyValuePair<TKey, TFrom>( item.Key, (TFrom) item.Value ) );

        public void Clear() => adapted.Clear();

        public bool Contains( KeyValuePair<TKey, TTo> item ) => adapted.ContainsKey( item.Key );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public void CopyTo( KeyValuePair<TKey, TTo>[] array, int arrayIndex )
        {
            Arg.NotNull( array, nameof( array ) );
            var temp = new KeyValuePair<TKey, TFrom>[array.Length];
            adapted.CopyTo( temp, arrayIndex );
            temp.Cast<TTo>().ToArray().CopyTo( array, arrayIndex );
        }

        public int Count => adapted.Count;

        public bool IsReadOnly => adapted.IsReadOnly;

        public bool Remove( KeyValuePair<TKey, TTo> item ) => adapted.Remove( item.Key );

        public IEnumerator<KeyValuePair<TKey, TTo>> GetEnumerator() => adapted.Select( p => new KeyValuePair<TKey, TTo>( p.Key, p.Value ) ).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}