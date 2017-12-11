namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a dictionary which associates multiple values with a single key.
    /// When indexing a <see cref="IMultivalueDictionary{TKey,TValue}">multivalue dictionary</see>, each key returns
    /// a <see cref="ICollection{T}">collection</see> of values.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
    [ContractClass( typeof( IMultivalueDictionaryContract<,> ) )]
    public interface IMultivalueDictionary<TKey, TValue> : IDictionary<TKey, ICollection<TValue>>
    {
        /// <summary>
        /// Determines the index of a specific key in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <returns>The index of <paramref name="key"/> if found in the <see cref="IMultivalueDictionary{TKey,TValue}">dictionary</see>; otherwise, -1.</returns>
        int IndexOf( TKey key );

        /// <summary>
        /// Adds new values to a key.
        /// </summary>
        /// <param name="key">The key to add values to.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to add.</param>
        /// <remarks>If duplicate values are permitted, this method always adds new key/value pairs to the dictionary.
        /// If duplicate values are not permitted, and the <paramref name="key"/> already has a value equal to one of the
        /// <paramref name="values"/> associated with it, then that value is replaced, and the number of values associated with the
        /// <paramref name="key"/> is unchanged.</remarks>
        void AddRange( TKey key, IEnumerable<TValue> values );

        /// <summary>
        /// Adds a new value to a key.
        /// </summary>
        /// <param name="key">The key to add the value to.</param>
        /// <param name="value">The value to add.</param>
        /// <remarks>If duplicate values are permitted, this method always adds a new key/value pair to the dictionary.
        /// If duplicate values are not permitted, and the <paramref name="key"/> already has a value equal to the
        /// <paramref name="value"/> associated with it, then that value is replaced, and the number of values associated with the
        /// <paramref name="key"/> is unchanged.</remarks>
        void Add( TKey key, TValue value );

        /// <summary>
        /// Removes a sequence of values from a key. If the last value is removed from a key, then the key is also removed.
        /// </summary>
        /// <param name="key">The key to remove values from.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to remove.</param>
        /// <returns>The number of values that were removed.</returns>
        int RemoveRange( TKey key, IEnumerable<TValue> values );

        /// <summary>
        /// Remove all of the specified keys and their associated values.
        /// </summary>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of key values to remove.</param>
        /// <returns>The number of keys that were removed.</returns>
        int RemoveRange( IEnumerable<TKey> keys );

        /// <summary>
        /// Removes a given value from the values associated with a key. If the last value is removed from a key, then the key is also removed.
        /// </summary>
        /// <param name="key">The key to remove a value from.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns><see langkeyword="true">True</see> if the <paramref name="value"/> was removed; otherwise, <see langkeyword="false">false</see>.</returns>
        bool Remove( TKey key, TValue value );

        /// <summary>
        /// Replaces all of the values associated with <paramref name="key"/> with a single <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The replacement value.</param>
        /// <returns><see langkeyword="true">True</see> if the value was replaced for the specified <paramref name="key"/>;
        /// otherwise <see langkeyword="false">false</see>.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set", Justification = "Matches SetRange counterpart." )]
        bool Set( TKey key, TValue value );

        /// <summary>
        /// Replaces all values associated for the specified <paramref name="key"/> with a new collection of values. If the collection does not
        /// permit duplicate values, and <paramref name="values"/> has duplicate items, then only the distinct list of values is added.
        /// </summary>
        /// <param name="key">The key to set values for.</param>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of new values set.</param>
        /// <returns><see langkeyword="true">True</see> if any values were replaced for the specified <paramref name="key"/>;
        /// otherwise <see langkeyword="false">false</see>.</returns>
        bool SetRange( TKey key, IEnumerable<TValue> values );

        /// <summary>
        /// Determines if this dictionary contains a key/value pair equal to <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">The value to locate.</param>
        /// <returns>True if the dictionary has an associated <paramref name="value"/> with the specified <paramref name="key"/>.</returns>
        bool Contains( TKey key, TValue value );

        /// <summary>
        /// Returns the count of values associated with a key.
        /// </summary>
        /// <param name="key">The key to count values for.</param>
        /// <returns>If the <paramref name="key"/> exists, then the number of values associated with that <paramref name="key"/>.</returns>
        int CountValues( TKey key );

        /// <summary>
        /// Returns the total count of values in the collection.
        /// </summary>
        /// <returns>The total number of values associated with all keys in the dictionary.</returns>
        int CountAllValues();
    }
}