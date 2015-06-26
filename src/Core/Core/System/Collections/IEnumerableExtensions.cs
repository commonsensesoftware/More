namespace System.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for non-generic enumerators.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to check for emptiness.</param>
        /// <returns>True if the sequence sequence contains any elements; otherwise, false.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static bool Any( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, "sequence" );

            var enumerator = sequence.GetEnumerator();

            try
            {
                return enumerator.MoveNext();
            }
            finally
            {
                var disposable = enumerator as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// Returns the number of elements in a sequence..
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable">sequence</see> that contains elements to be counted.</param>
        /// <returns>The number of elements in the input <paramref name="sequence"/>.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static int Count( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<int>() >= 0 );

            var collection = sequence as ICollection;

            if ( collection != null )
                return collection.Count;

            var enumerator = sequence.GetEnumerator();
            var count = 0;

            try
            {
                while ( enumerator.MoveNext() )
                    ++count;
            }
            finally
            {
                var disposable = enumerator as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }

            return count;
        }

        /// <summary>
        /// Returns the zero-based index of the element in the sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to search.</param>
        /// <param name="item">The <see cref="Object"/> item to locate.</param>
        /// <returns>The zero-based index the specified item or -1 if no match is found.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static int IndexOf( this IEnumerable sequence, object item )
        {
            Arg.NotNull( sequence, "sequence" );
            Contract.Ensures( Contract.Result<int>() >= -1 );

            return sequence.IndexOf( item, EqualityComparer<object>.Default );
        }

        /// <summary>
        /// Returns the zero-based index of the element in the sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable"/> to search.</param>
        /// <param name="item">The <see cref="Object"/> value to locate.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> used to compare items for equality.</param>
        /// <returns>The zero-based index the specified item or -1 if no match is found.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static int IndexOf( this IEnumerable sequence, object item, IEqualityComparer comparer )
        {
            Arg.NotNull( sequence, "sequence" );
            Arg.NotNull( comparer, "comparer" );
            Contract.Ensures( Contract.Result<int>() >= -1 );

            var enumerator = sequence.GetEnumerator();
            var index = 0;

            try
            {
                while ( enumerator.MoveNext() )
                {
                    var current = enumerator.Current;

                    if ( comparer.Equals( current, item ) )
                        return index;

                    index++;
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }

            return -1;
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <returns>The element at the specified position in the sequence sequence.</returns>
        /// <param name="sequence">An <see cref="IEnumerable" /> to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object ElementAt( this IEnumerable sequence, int index )
        {
            Arg.NotNull( sequence, "sequence" );
            Arg.GreaterThanOrEqualTo( index, 0, "index" );

            var list = sequence as IList;

            if ( list != null )
                return list[index];

            var enumerator = sequence.GetEnumerator();
            object value = null;

            try
            {
                while ( enumerator.MoveNext() )
                {
                    if ( index-- == 0 )
                    {
                        value = enumerator.Current;
                        break;
                    }
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }

            // if we didn't count down to -1, then the index is greater
            // than the length of the sequence
            if ( index != -1 )
                throw new ArgumentOutOfRangeException( "index" );

            return value;
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <param name="sequence">An <see cref="IEnumerable" /> to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified position in the sequence sequence or <c>null</c>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object ElementAtOrDefault( this IEnumerable sequence, int index )
        {
            Arg.NotNull( sequence, "sequence" );

            if ( index < 0 )
                return null;

            var list = sequence as IList;

            if ( list != null )
                return index >= list.Count ? null : list[index];

            var enumerator = sequence.GetEnumerator();
            object value = null;

            try
            {
                while ( enumerator.MoveNext() )
                {
                    if ( index-- == 0 )
                    {
                        value = enumerator.Current;
                        break;
                    }
                }
            }
            finally
            {
                var disposable = enumerator as IDisposable;

                if ( disposable != null )
                    disposable.Dispose();
            }

            return value;
        }
    }
}
