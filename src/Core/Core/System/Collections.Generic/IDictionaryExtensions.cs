namespace System.Collections.Generic
{
    using More;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension for the <see cref="IDictionary{TKey,TValue}"/> interface.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Returns a read-only copy of the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}">dictionary</see> to make read-only.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}">read-only dictionary</see>.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>( this IDictionary<TKey, TValue> dictionary )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Contract.Ensures( Contract.Result<IReadOnlyDictionary<TKey, TValue>>() != null );

            var readOnlyDictionary = dictionary as IReadOnlyDictionary<TKey, TValue>;
            return readOnlyDictionary == null ? new ReadOnlyDictionary<TKey, TValue>( dictionary ) : readOnlyDictionary;
        }

        /// <summary>
        /// Returns the value specified at <paramref name="key"/> or adds the value to the dictionary returned from the supplied factory method.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of item key.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see> object.</param>
        /// <param name="key">The key to use to return a value from the <paramref name="dictionary"/>.</param>
        /// <param name="newValue">The <see cref="Func{TResult}"/> called to add to the <paramref name="dictionary"/> and return.</param>
        /// <returns>If present, the value found at <paramref name="key"/>; otherwise the value from <paramref name="newValue"/> that has been added.</returns>
        /// <example>This example demonstrates how to get and optionally add an item to a dictionary as an atomic operation.
        /// <code lang="C#">
        /// <![CDATA[
        /// public static Employee GetByName( string name) {
        ///     return employees.GetOrAdd( name, () => new Employee( whatever ) );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// <para>The <paramref name="dictionary"/> parameter is <see langkeyword="null">null</see></para>
        /// <para>- or -</para>
        /// <para>The <paramref name="key">key</paramref> parmeter is <c>null</c>or <see cref="F:String.Empty"/></para>
        /// <para>- or -</para>
        /// <para>The <paramref name="newValue">new value</paramref> parameter is <see langkeyword="null">null</see></para>
        /// </exception>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "newValue", Justification = "False positive" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.StyleCop.CSharp.DocumentationRules", "SA1644:DocumentationHeadersMustNotContainBlankLines", Justification = "Example code often has blank lines." )]
        public static TValue GetOrAdd<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> newValue )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( newValue, "newValue" );

            TValue value;

            if ( dictionary.TryGetValue( key, out value ) )
                return value;

            value = newValue();
            dictionary.Add( key, value );

            return value;
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but skips the specified keys using the default comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to skip.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to skip key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Skip( "Key2", "Key4" );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Skip<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, params TKey[] keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Skip( keys, EqualityComparer<TKey>.Default );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but skips the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used by the returned dictionary and used to evaluate skipped keys.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to skip.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to skip key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Skip( StringComparer.Ordinal, "Key2", "Key4" );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Skip<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, params TKey[] keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( comparer, "comparer" );
            Arg.NotNull( keys, "keys" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Skip( keys, comparer );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but skips the specified keys using the default comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to skip.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to skip key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Skip( new []{ "Key2", "Key4" } );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Skip<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Skip( keys, EqualityComparer<TKey>.Default );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but skips the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to skip.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used by the returned dictionary and used to evaluate skipped keys.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to skip key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Skip( new []{ "Key2", "Key4" }, StringComparer.Ordinal );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Skip<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, IEqualityComparer<TKey> comparer )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );

            var keyList = keys.ToArray();
            var filtered = new Dictionary<TKey, TValue>( comparer );

            foreach ( var key in dictionary.Keys )
            {
                if ( keyList.Any( k => comparer.Equals( k, key ) ) )
                    continue;

                TValue value;

                if ( dictionary.TryGetValue( key, out value ) )
                    filtered.Add( key, value );
            }

            return filtered;
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but only copies the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to copy.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to take key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Take( "Key2", "Key4" );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Take<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, params TKey[] keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Take( keys, EqualityComparer<TKey>.Default );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but only copies the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used by the returned dictionary and used to evaluate copied keys.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to copy.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to take key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Take( StringComparer.Ordinal, "Key2", "Key4" );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Take<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, params TKey[] keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Take( keys, comparer );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but only copies the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to copy.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to take key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Take( new []{ "Key2", "Key4" } );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Take<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Take( keys, EqualityComparer<TKey>.Default );
        }

        /// <summary>
        /// Returns a copy of the specified dictionary, but only copies the specified keys using the provided comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="keys">The <see cref="IEnumerable{T}">sequence</see> of keys to copy.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used by the returned dictionary and used to evaluate copied keys.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to take key/value pairs from an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var original = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var trimmed = orignal.Take( new []{ "Key2", "Key4" }, StringComparer.Ordinal );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Take<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, IEqualityComparer<TKey> comparer )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( keys, "keys" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );

            var taken = new Dictionary<TKey, TValue>( comparer );

            foreach ( var key in keys )
            {
                TValue value;

                if ( dictionary.TryGetValue( key, out value ) )
                    taken.Add( key, value );
            }

            return taken;
        }

        /// <summary>
        /// Returns the set union of two dictionaries using the default comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="otherDictionary">The <see cref="IDictionary{TKey,TValue}">dictionary</see> whose keys and values produce a unique set.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to create a set union of key/value pairs between two dictionaries.
        /// <code lang="C#">
        /// <![CDATA[
        /// var dictionary = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var otherDictionary = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 0 },
        ///     { "Key3", 0 },
        ///     { "Key6", 6 }
        /// };
        /// 
        /// // contains Key1 - Key6 (keys that already exist on the left-hand are ignored from the right-hand side)
        /// var unioned = dictionary.Union( otherDictionary );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Union<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( otherDictionary, "otherDictionary" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Union( otherDictionary, EqualityComparer<TKey>.Default );
        }

        /// <summary>
        /// Returns the set union of two dictionaries using the specified comparer.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="otherDictionary">The <see cref="IDictionary{TKey,TValue}">dictionary</see> whose keys and values produce a unique set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used evaluate the unique set of keys between the two dictionaries and the resultant <see cref="IDictionary{TKey,TValue}"/>.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to create a set union of key/value pairs between two dictionaries.
        /// <code lang="C#">
        /// <![CDATA[
        /// var dictionary = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// var otherDictionary = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 0 },
        ///     { "Key3", 0 },
        ///     { "Key6", 6 }
        /// };
        /// 
        /// // contains Key1 - Key6 (keys that already exist on the left-hand are ignored from the right-hand side)
        /// var unioned = dictionary.Union( otherDictionary, StringComparer.Ordinal );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static IDictionary<TKey, TValue> Union<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary, IEqualityComparer<TKey> comparer )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( otherDictionary, "otherDictionary" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );

            var union = new Dictionary<TKey, TValue>( dictionary, comparer );
            TValue value;

            foreach ( var key in otherDictionary.Keys )
            {
                if ( !union.ContainsKey( key ) && otherDictionary.TryGetValue( key, out value ) )
                    union.Add( key, value );
            }

            return union;
        }

        /// <summary>
        /// Reduces the specified dictionary by combining two keys into a single, new key and removing the old keys.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="key1">The first key of type <typeparamref name="TKey"/> to reduce.</param>
        /// <param name="key2">The second key of type <typeparamref name="TKey"/> to reduce.</param>
        /// <param name="newKey">The key of type <typeparamref name="TKey"/> that contains the resultant reduction.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to reduce two key/value pairs to a single key/value pair in an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var dictionary = new Dictionary<string, int>()
        /// {
        ///     { "KeyA", 1 },
        ///     { "KeyB", 2 },
        ///     { "KeyC", 1 },
        ///     { "KeyD", 2 },
        ///     { "KeyE", 3 }
        /// };
        /// 
        /// // KeyA and KeyC are removed and a new KeyA is added with value 1 (from the original KeyA)
        /// var reduced = dictionary.Reduce( "KeyA", "KeyC", "KeyA" );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.StyleCop.CSharp.DocumentationRules", "SA1644:DocumentationHeadersMustNotContainBlankLines", Justification = "Example code often has blank lines." )]
        public static IDictionary<TKey, TValue> Reduce<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key1, TKey key2, TKey newKey )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( key1, "key1" );
            Arg.NotNull( key2, "key2" );
            Arg.NotNull( newKey, "newKey" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );
            return dictionary.Reduce( key1, key2, newKey, ( v1, v2 ) => v1 );
        }

        /// <summary>
        /// Reduces the specified dictionary by combining two keys into a new key and removing the old keys.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type">type</see> of key used by the dictionary.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value used by the dictionary.</typeparam>
        /// <param name="dictionary">The extended <see cref="IDictionary{TKey,TValue}">dictionary</see>.</param>
        /// <param name="key1">The first key of type <typeparamref name="TKey"/> to reduce.</param>
        /// <param name="key2">The second key of type <typeparamref name="TKey"/> to reduce.</param>
        /// <param name="newKey">The key of type <typeparamref name="TKey"/> that contains the resultant reduction.</param>
        /// <param name="selector">The <see cref="Func{T1,T2,TResult}"/> that selects the reduced value.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> object.</returns>
        /// <example>This example demonstrates how to reduce two key/value pairs to a single key/value pair in an existing dictionary.
        /// <code lang="C#">
        /// <![CDATA[
        /// var dictionary = new Dictionary<string, int>()
        /// {
        ///     { "Key1", 1 },
        ///     { "Key2", 2 },
        ///     { "Key3", 3 },
        ///     { "Key4", 4 },
        ///     { "Key5", 5 }
        /// };
        /// 
        /// // Key4 and Key5 are removed and Key9 is added
        /// var reduced = dictionary.Reduce( "Key4", "Key5", "Key9", ( v1, v2 ) => v1 + v2 );
        /// }
        /// ]]>
        /// </code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "4", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.StyleCop.CSharp.DocumentationRules", "SA1644:DocumentationHeadersMustNotContainBlankLines", Justification = "Example code often has blank lines." )]
        public static IDictionary<TKey, TValue> Reduce<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key1, TKey key2, TKey newKey, Func<TValue, TValue, TValue> selector )
        {
            Arg.NotNull( dictionary, "dictionary" );
            Arg.NotNull( key1, "key1" );
            Arg.NotNull( key2, "key2" );
            Arg.NotNull( newKey, "newKey" );
            Arg.NotNull( selector, "selector" );
            Contract.Ensures( Contract.Result<IDictionary<TKey, TValue>>() != null );

            // capture new value
            var value = selector( dictionary[key1], dictionary[key2] );

            // remove old keys and add/replace with new one
            dictionary.Remove( key1 );
            dictionary.Remove( key2 );
            dictionary[newKey] = value;

            return dictionary;
        }
    }
}
