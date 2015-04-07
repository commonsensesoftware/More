namespace System.Collections.Generic
{
    using More;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;
    using global::System.Reflection;

    /// <summary>
    /// Re-implementation of the sort helper found in the BCL to support sorting and searching any IList{T} implementation.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to sort.</typeparam>
    internal static class ListHelper<T>
    {
        private static bool IsTypeComparable
        {
            get
            {
                return typeof( IComparable<T> ).GetTypeInfo().IsAssignableFrom( typeof( T ).GetTypeInfo() );
            }
        }

        private static void SwapByComparable( IList<T> keys, int a, int b )
        {
            Contract.Requires( keys != null, "keys" );
            Contract.Requires( a >= 0 && a < keys.Count, "a" );
            Contract.Requires( b >= 0 && b < keys.Count, "b" );
            if ( ( a != b ) && ( ( keys[a] == null ) || ( (IComparable<T>) keys[a] ).CompareTo( keys[b] ) > 0 ) )
            {
                T local = keys[a];
                keys[a] = keys[b];
                keys[b] = local;
            }
        }

        private static void SwapByComparer( IList<T> keys, IComparer<T> comparer, int a, int b )
        {
            Contract.Requires( keys != null, "keys" );
            Contract.Requires( comparer != null, "comparer" );
            Contract.Requires( a >= 0 && a < keys.Count, "a" );
            Contract.Requires( b >= 0 && b < keys.Count, "b" );
            if ( ( a != b ) && ( comparer.Compare( keys[a], keys[b] ) > 0 ) )
            {
                T local = keys[a];
                keys[a] = keys[b];
                keys[b] = local;
            }
        }

        private static void QuickSortByComparable( IList<T> keys, int left, int right )
        {
            Contract.Requires( keys != null, "keys" );
            Contract.Requires( left >= 0 && left <= keys.Count, "left" );
            Contract.Requires( right >= 0 && right <= keys.Count, "right" );
            int i;

        StepOne:

            i = left;
            var b = right;
            var j = i + ( ( b - i ) >> 1 );

            SwapByComparable( keys, i, j );
            SwapByComparable( keys, i, b );
            SwapByComparable( keys, j, b );

            var local = (IComparable<T>) keys[j];

        StepTwo:

            if ( local != null )
            {
                while ( local.CompareTo( keys[i] ) > 0 )
                    i++;

                while ( local.CompareTo( keys[b] ) < 0 )
                    b--;
            }
            else
            {
                while ( keys[b] != null )
                    b--;
            }

            if ( i <= b )
            {
                if ( i < b )
                {
                    T local2 = keys[i];
                    keys[i] = keys[b];
                    keys[b] = local2;
                }

                i++;
                b--;

                if ( i <= b )
                    goto StepTwo;
            }

            if ( ( b - left ) <= ( right - i ) )
            {
                if ( left < b )
                    QuickSortByComparable( keys, left, b );

                left = i;
            }
            else
            {
                if ( i < right )
                    QuickSortByComparable( keys, i, right );

                right = b;
            }

            if ( left >= right )
                return;

            goto StepOne;
        }

        private static void QuickSortByComparer( IList<T> keys, int left, int right, IComparer<T> comparer )
        {
            Contract.Requires( keys != null, "keys" );
            Contract.Requires( comparer != null, "comparer" );
            Contract.Requires( left >= 0 && left <= keys.Count, "left" );
            Contract.Requires( right >= 0 && right <= keys.Count, "right" );
            do
            {
                int a = left;
                int b = right;
                int i = a + ( ( b - a ) >> 1 );

                SwapByComparer( keys, comparer, a, i );
                SwapByComparer( keys, comparer, a, b );
                SwapByComparer( keys, comparer, i, b );

                T key = keys[i];

                do
                {
                    while ( comparer.Compare( keys[a], key ) < 0 )
                        a++;

                    while ( comparer.Compare( key, keys[b] ) < 0 )
                        b--;

                    if ( a > b )
                        break;

                    if ( a < b )
                    {
                        T temp = keys[a];
                        keys[a] = keys[b];
                        keys[b] = temp;
                    }

                    a++;
                    b--;
                }
                while ( a <= b );

                if ( ( b - left ) <= ( right - a ) )
                {
                    if ( left < b )
                        QuickSortByComparer( keys, left, b, comparer );

                    left = a;
                }
                else
                {
                    if ( a < right )
                        QuickSortByComparer( keys, a, right, comparer );

                    right = b;
                }
            }
            while ( left < right );
        }

        private static int BinarySearchByComparable( IList<T> list, int index, int length, IComparable<T> value )
        {
            Contract.Requires( list != null, "list" );
            Contract.Requires( index >= 0 && index < list.Count, "index" );
            Contract.Requires( ( length >= 0 ) && ( length <= ( list.Count - index ) ), "length" );
            Contract.Requires( index + ( length - 1 ) >= 0, "index" );
            int i = index;
            int j = ( index + length ) - 1;

            while ( i <= j )
            {
                var k = i + ( ( j - i ) >> 1 );
                int l;

                Contract.Assume( k >= 0 && k < list.Count );

                // 'else' branch is reversed to remove type constraint and optimize type casting
                if ( list[k] == null )
                    l = value == null ? 0 : -1;
                else
                    l = value == null ? 0 : -value.CompareTo( list[k] );

                if ( l == 0 )
                    return k;

                if ( l < 0 )
                    i = k + 1;
                else
                    j = k - 1;
            }

            return ~i;
        }

        private static int BinarySearchByComparer( IList<T> list, int index, int length, T value, IComparer<T> comparer )
        {
            Contract.Requires( list != null, "list" );
            Contract.Requires( comparer != null, "comparer" );
            Contract.Requires( index >= 0 && index < list.Count, "index" );
            Contract.Requires( ( length >= 0 ) && ( length <= ( list.Count - index ) ), "length" );
            Contract.Requires( index + ( length - 1 ) >= 0, "index" );
            var i = index;
            var j = ( index + length ) - 1;

            while ( i <= j )
            {
                var k = i + ( ( j - i ) >> 1 );
                Contract.Assume( k >= 0 && k < list.Count );
                var l = comparer.Compare( list[k], value );

                if ( l == 0 )
                    return k;

                if ( l < 0 )
                    i = k + 1;
                else
                    j = k - 1;
            }

            return ~i;
        }

        internal static int BinarySearch( IList<T> list, int index, int length, T value, IComparer<T> comparer )
        {
            Contract.Requires( list != null, "list" );
            Contract.Requires( comparer != null, "comparer" );
            Contract.Requires( index >= 0 && index < list.Count, "index" );
            Contract.Requires( ( length >= 0 ) && ( length <= ( list.Count - index ) ), "length" );
            Contract.Requires( index + ( length - 1 ) >= 0, "index" );

            try
            {
                if ( ( comparer == null ) || ( comparer == Comparer<T>.Default && IsTypeComparable ) )
                    return BinarySearchByComparable( list, index, length, (IComparable<T>) value );

                return BinarySearchByComparer( list, index, length, value, comparer );
            }
            catch ( Exception exception )
            {
                throw new InvalidOperationException( ExceptionMessage.IComparerFailed, exception );
            }
        }

        internal static void Sort( IList<T> keys, int index, int length, IComparer<T> comparer )
        {
            Contract.Requires( keys != null, "keys" );
            Contract.Requires( index >= 0 && index < keys.Count, "index" );
            Contract.Requires( ( length >= 0 ) && ( length <= ( keys.Count - index ) ), "length" );
            Contract.Requires( comparer != null, "comparer" );
            Contract.Requires( index + ( length - 1 ) >= 0, "index" );

            try
            {
                if ( ( comparer == null ) || ( comparer == Comparer<T>.Default && IsTypeComparable ) )
                    QuickSortByComparable( keys, index, index + ( length - 1 ) );
                else
                    QuickSortByComparer( keys, index, index + ( length - 1 ), comparer );
            }
            catch ( IndexOutOfRangeException )
            {
                throw new ArgumentException( string.Format( null, ExceptionMessage.BogusIComparer, default( T ), typeof( T ).Name ) );
            }
            catch ( Exception exception )
            {
                throw new InvalidOperationException( ExceptionMessage.IComparerFailed, exception );
            }
        }
    }
}
