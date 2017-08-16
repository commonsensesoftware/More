namespace System.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using static More.ExceptionMessage;

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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static bool Any( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            var enumerator = sequence.GetEnumerator();

            try
            {
                return enumerator.MoveNext();
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Returns the number of elements in a sequence..
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable">sequence</see> that contains elements to be counted.</param>
        /// <returns>The number of elements in the input <paramref name="sequence"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static int Count( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<int>() >= 0 );

            if ( sequence is ICollection collection )
            {
                return collection.Count;
            }

            var enumerator = sequence.GetEnumerator();
            var count = 0;

            try
            {
                while ( enumerator.MoveNext() )
                {
                    ++count;
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
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
        public static int IndexOf( this IEnumerable sequence, object item ) => IndexOf( sequence, item, EqualityComparer<object>.Default );

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
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );

            var enumerator = sequence.GetEnumerator();
            var index = 0;

            try
            {
                while ( enumerator.MoveNext() )
                {
                    var current = enumerator.Current;

                    if ( comparer.Equals( current, item ) )
                    {
                        return index;
                    }

                    index++;
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
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
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );

            if ( sequence is IList list )
            {
                return list[index];
            }

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
                ( enumerator as IDisposable )?.Dispose();
            }

            if ( index != -1 )
            {
                throw new ArgumentOutOfRangeException( nameof( index ) );
            }

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
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( index < 0 )
            {
                return null;
            }

            if ( sequence is IList list )
            {
                return index >= list.Count ? null : list[index];
            }

            var enumerator = sequence.GetEnumerator();
            var value = default( object );

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
                ( enumerator as IDisposable )?.Dispose();
            }

            return value;
        }

        /// <summary>
        /// Returns the first element of a sequence or throws an exception if the sequence is empty.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the first element from.</param>
        /// <returns>The first element from the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object First( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list && list.Count > 0 )
            {
                return list[0];
            }

            var enumerator = sequence.GetEnumerator();

            try
            {
                if ( !enumerator.MoveNext() )
                {
                    throw new InvalidOperationException( NoElements );
                }

                return enumerator.Current;
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Returns the first element of a sequence or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the first element from.</param>
        /// <returns>The first element from the input sequence or <c>null</c> if the sequence contains no elements.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object FirstOrDefault( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list )
            {
                return list.Count == 0 ? null : list[0];
            }

            var enumerator = sequence.GetEnumerator();

            try
            {
                if ( enumerator.MoveNext() )
                {
                    return enumerator.Current;
                }

                return null;
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Returns the last element of a sequence or throws an exception if the sequence is empty.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the last element from.</param>
        /// <returns>The last element from the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object Last( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list )
            {
                var count = list.Count;

                if ( count > 0 )
                {
                    return list[count - 1];
                }
            }

            var enumerator = sequence.GetEnumerator();
            var last = default( object );

            try
            {
                if ( !enumerator.MoveNext() )
                {
                    throw new InvalidOperationException( NoElements );
                }

                do
                {
                    last = enumerator.Current;
                }
                while ( enumerator.MoveNext() );
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }

            return last;
        }

        /// <summary>
        /// Returns the last element of a sequence or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the last element from.</param>
        /// <returns>The last element from the input sequence or <c>null</c> if the sequence contains no elements.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object LastOrDefault( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list )
            {
                var count = list.Count;
                return count == 0 ? null : list[count - 1];
            }

            var enumerator = sequence.GetEnumerator();
            var last = default( object );

            try
            {
                if ( !enumerator.MoveNext() )
                {
                    return last;
                }

                do
                {
                    last = enumerator.Current;
                }
                while ( enumerator.MoveNext() );
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }

            return last;
        }

        /// <summary>
        /// Returns the only element of a sequence or throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the single element from.</param>
        /// <returns>The single element of the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object Single( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list )
            {
                switch ( list.Count )
                {
                    case 0:
                        throw new InvalidOperationException( NoElements );
                    case 1:
                        return list[0];
                    default:
                        throw new InvalidOperationException( MoreThanOneMatch );
                }
            }

            var enumerator = sequence.GetEnumerator();

            try
            {
                if ( !enumerator.MoveNext() )
                {
                    throw new InvalidOperationException( NoElements );
                }

                var current = enumerator.Current;

                if ( enumerator.MoveNext() )
                {
                    throw new InvalidOperationException( MoreThanOneMatch );
                }

                return current;
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Returns the only element of a sequence or a default value if the sequence is empty;
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to return the single element from.</param>
        /// <returns>A single element from the input sequence or <c>null</c> if the sequence contains no elements.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object SingleOrDefault( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );

            if ( sequence is IList list )
            {
                switch ( list.Count )
                {
                    case 0:
                        return null;
                    case 1:
                        return list[0];
                    default:
                        throw new InvalidOperationException( MoreThanOneMatch );
                }
            }

            var enumerator = sequence.GetEnumerator();

            try
            {
                if ( !enumerator.MoveNext() )
                {
                    return null;
                }

                var current = enumerator.Current;

                if ( enumerator.MoveNext() )
                {
                    throw new InvalidOperationException( MoreThanOneMatch );
                }

                return current;
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Creates an array from the specified sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static object[] ToArray( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<object[]>() != null );

            if ( sequence is ICollection collection )
            {
                var array = new object[collection.Count];
                collection.CopyTo( array, 0 );
                return array;
            }

            var enumerator = sequence.GetEnumerator();
            var items = new List<object>();

            try
            {
                if ( enumerator.MoveNext() )
                {
                    items.Add( enumerator.Current );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }

            return items.ToArray();
        }

        /// <summary>
        /// Creates a list from the specified sequence.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable">sequence</see> to create a list from.</param>
        /// <returns>A <see cref="List{T}">list</see> that contains elements from the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static List<object> ToList( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<List<object>>() != null );

            var enumerator = sequence.GetEnumerator();
            var list = new List<object>();

            try
            {
                while ( enumerator.MoveNext() )
                {
                    list.Add( enumerator.Current );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }

            return list;
        }

        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable">sequence</see> of values to reverse.</param>
        /// <returns>A sequence whose elements correspond to those of the input sequence in reverse order.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IEnumerable Reverse( this IEnumerable sequence )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable>() != null );

            var enumerator = sequence.GetEnumerator();
            var stack = new Stack<object>();

            try
            {
                while ( enumerator.MoveNext() )
                {
                    stack.Push( enumerator.Current );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }

            foreach ( object item in stack )
            {
                yield return item;
            }

            stack.Clear();
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable">sequence</see> to return values from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A <see cref="IEnumerable">sequence</see> that contains the elements that occur after
        /// the specified index in the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IEnumerable Skip( this IEnumerable sequence, int count )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable>() != null );

            var enumerator = sequence.GetEnumerator();
            var i = 0;

            try
            {
                if ( enumerator.MoveNext() )
                {
                    while ( i++ < count )
                    {
                        if ( !enumerator.MoveNext() )
                        {
                            yield break;
                        }
                    }

                    do
                    {
                        yield return enumerator.Current;
                    }
                    while ( enumerator.MoveNext() );
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable">sequence</see> to return values from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A <see cref="IEnumerable">sequence</see> that contains the specified number of elements
        /// from the start of the input sequence.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IEnumerable Take( this IEnumerable sequence, int count )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable>() != null );

            if ( count < 1 )
            {
                yield break;
            }

            var enumerator = sequence.GetEnumerator();
            var i = 0;

            try
            {
                while ( i++ < count && enumerator.MoveNext() )
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
            }
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default
        /// equality comparer for their type.
        /// </summary>
        /// <param name="first">A <see cref="IEnumerable">sequence</see> to compare to second.</param>
        /// <param name="second">A <see cref="IEnumerable">sequence</see> to compare to the first sequence.</param>
        /// <returns>True if the two source sequences are of equal length and their corresponding elements
        /// are equal according to the default equality comparer for their type; otherwise, false</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static bool SequenceEqual( this IEnumerable first, IEnumerable second ) => SequenceEqual( first, second, EqualityComparer<object>.Default );

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default
        /// equality comparer for their type.
        /// </summary>
        /// <param name="first">A <see cref="IEnumerable">sequence</see> to compare to second.</param>
        /// <param name="second">A <see cref="IEnumerable">sequence</see> to compare to the first sequence.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer">comparer</see> to use to compare elements.</param>
        /// <returns>True if the two source sequences are of equal length and their corresponding elements
        /// are equal according to the default equality comparer for their type; otherwise, false</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static bool SequenceEqual( this IEnumerable first, IEnumerable second, IEqualityComparer comparer )
        {
            Arg.NotNull( first, nameof( first ) );
            Arg.NotNull( second, nameof( second ) );

            comparer = comparer ?? EqualityComparer<object>.Default;

            var enumerator = first.GetEnumerator();
            var enumerator2 = default( IEnumerator );

            try
            {
                enumerator2 = second.GetEnumerator();

                while ( enumerator.MoveNext() )
                {
                    if ( !enumerator2.MoveNext() || !comparer.Equals( enumerator.Current, enumerator2.Current ) )
                    {
                        return false;
                    }
                }

                if ( enumerator2.MoveNext() )
                {
                    return false;
                }
            }
            finally
            {
                ( enumerator as IDisposable )?.Dispose();
                ( enumerator2 as IDisposable )?.Dispose();
            }

            return true;
        }
    }
}