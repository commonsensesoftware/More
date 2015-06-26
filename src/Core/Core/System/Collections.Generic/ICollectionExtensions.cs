namespace System.Collections.Generic
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for the <see cref="ICollection{T}"/> interface.
    /// </summary>
    public static class ICollectionExtensions
    {
        private static ICollection<TItem> AddRange<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence, int? count, bool clearFirst )
        {
            Contract.Requires( collection != null );
            Contract.Requires( sequence != null );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );

            // ensure there is something to add
            if ( count.HasValue && count.Value == 0 )
                return collection;

            var itemCount = 0;
            var source = sequence.ToArray();
            var requestedCount = count ?? source.Length;

            if ( clearFirst )
                collection.Clear();

            foreach ( var item in source )
            {
                collection.Add( item );

                // add until the requested count is reached
                if ( ++itemCount == requestedCount )
                    break;
            }

            return collection;
        }

        /// <summary>
        /// Adds a range of elements to a collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be added.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of items to add.</param>
        /// <returns>The original <see cref="ICollection{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static ICollection<TItem> AddRange<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );
            return collection.AddRange( sequence, count: (int?) null, clearFirst: false );
        }

        /// <summary>
        /// Adds a range of elements to a collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be added.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of items to add.</param>
        /// <param name="count">The number of items from <paramref name="sequence"/> to be added.</param>
        /// <returns>The original <see cref="ICollection{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static ICollection<TItem> AddRange<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence, int count )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );
            Arg.GreaterThanOrEqualTo( count, 0, "count" );
            return collection.AddRange( sequence, new int?( count ), clearFirst: false );
        }

        /// <summary>
        /// Removes a range of elements from a collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be removed.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of items to remove.</param>
        /// <returns>The original <see cref="ICollection{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static ICollection<TItem> RemoveRange<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );

            foreach ( var item in sequence )
                collection.Remove( item );

            return collection;
        }

        /// <summary>
        /// Removes a range of elements from a collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be removed.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> sequence of items to remove.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare items.</param>
        /// <returns>The original <see cref="ICollection{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static ICollection<TItem> RemoveRange<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence, IEqualityComparer<TItem> comparer )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( sequence, "sequence" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );

            foreach ( var item in sequence )
            {
                var match = collection.FirstOrDefault( i => comparer.Equals( i, item ) );
                collection.Remove( match );
            }

            return collection;
        }

        /// <summary>
        /// Removes the first element in the collection that matches the specified item using the supplied comparer.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be removed.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="item">The item of type <typeparamref name="TItem"/> to remove.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare items.</param>
        /// <returns>True if the item is removed; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static bool Remove<TItem>( this ICollection<TItem> collection, TItem item, IEqualityComparer<TItem> comparer )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( comparer, "comparer" );

            var match = collection.FirstOrDefault( i => comparer.Equals( i, item ) );
            return collection.Remove( match );
        }

        /// <summary>
        /// Removes all elements in the collection that matches the specified item using the supplied comparer.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be removed.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="item">The item of type <typeparamref name="TItem"/> to remove.</param>
        /// <returns>The total number of items removed.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static int RemoveAll<TItem>( this ICollection<TItem> collection, TItem item )
        {
            Arg.NotNull( collection, "collection" );
            Contract.Ensures( Contract.Result<int>() >= 0 );
            return collection.RemoveAll( item, EqualityComparer<TItem>.Default );
        }

        /// <summary>
        /// Removes all elements in the collection that matches the specified item using the supplied comparer.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be removed.</typeparam>
        /// <param name="collection">The extended <see cref="ICollection{T}">collection</see>.</param>
        /// <param name="item">The item of type <typeparamref name="TItem"/> to remove.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare items.</param>
        /// <returns>The total number of items removed.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static int RemoveAll<TItem>( this ICollection<TItem> collection, TItem item, IEqualityComparer<TItem> comparer )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<int>() >= 0 );

            var preCount = collection.Count;
            var matches = collection.Where( i => comparer.Equals( i, item ) ).ToArray();
            var postCount = collection.RemoveRange( matches, comparer ).Count;

            return preCount - postCount;
        }

        /// <summary>
        /// Replaces all of the elements in a collection by clearing the collection and subsequently adding the elements from the supplied sequence.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be copied.</typeparam>
        /// <param name="collection">The <see cref="ICollection{T}"/> to extend.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to be copied.</param>
        /// <returns>The original <see cref="ICollection{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static ICollection<TItem> ReplaceAll<TItem>( this ICollection<TItem> collection, IEnumerable<TItem> sequence )
        {
            Arg.NotNull( collection, "collection" );
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<ICollection<TItem>>() != null );
            return collection.AddRange( sequence, count: null, clearFirst: true );
        }
    }
}
