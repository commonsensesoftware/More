namespace System.Collections.Generic
{
    using More.Collections.Generic;
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Re-implementation of the <see cref="ReadOnlyObservableCollection{T}"/> class with support for <see cref="IList{T}"/> as the source.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of item in the collection.</typeparam>
        sealed class ReadOnlyObservableList<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            internal ReadOnlyObservableList( IList<T> list ) : base( list )
            {
                Contract.Requires( list != null );
                ( (INotifyCollectionChanged) Items ).CollectionChanged += ( s, e ) => OnCollectionChanged( e );
                ( (INotifyPropertyChanged) Items ).PropertyChanged += ( s, e ) => OnPropertyChanged( e );
            }

            void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( e != null );
                CollectionChanged?.Invoke( this, e );
            }

            void OnPropertyChanged( PropertyChangedEventArgs e )
            {
                Contract.Requires( e != null );
                PropertyChanged?.Invoke( this, e );
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
            public event PropertyChangedEventHandler PropertyChanged;
        }

        /// <summary>
        /// Returns an adapted list that is covariant and contravariant with the provided source list.
        /// </summary>
        /// <typeparam name="TFrom">The <see cref="Type">type</see> of item to make covariant.</typeparam>
        /// <typeparam name="TTo">The <see cref="Type">type</see> of convariant item.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> to make variant.</param>
        /// <returns>An <see cref="IList{T}"/> object.</returns>
        /// <remarks>If the provided list implements <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>, then
        /// an appropriate, observable adapter will be returned.</remarks>
        /// <example>This example demonstrates how to create a variant list from an existing list.
        /// <code lang="C#">
        /// <![CDATA[
        /// var sourceList = new List<string>();
        /// var variantList = sourceList.AsVariant<string, object>();
        /// ]]>
        /// </code>
        /// </example>
        /// <example>This example demonstrates how to create a variant observable list from an existing observable list.
        /// <code lang="C#">
        /// <![CDATA[
        /// var sourceList = new ObservableCollection<string>();
        /// var variantList = sourceList.AsVariant<string, object>();
        /// ]]>
        /// </code>
        /// </example>
        [Pure]
        public static IList<TTo> AsVariant<TFrom, TTo>( this IList<TFrom> list ) where TFrom : TTo
        {
            Arg.NotNull( list, nameof( list ) );
            Contract.Ensures( Contract.Result<IList<TTo>>() != null );

            if ( typeof( TFrom ).Equals( typeof( TTo ) ) )
            {
                return (IList<TTo>) list;
            }

            if ( ( list is INotifyCollectionChanged ) && ( list is INotifyPropertyChanged ) )
            {
                return new ObservableVariantListAdapter<TFrom, TTo>( list );
            }

            return new VariantListAdapter<TFrom, TTo>( list );
        }

        /// <summary>
        /// Returns a read-only copy of the specified list.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item in the list.</typeparam>
        /// <param name="list">The <see cref="IList{T}">list</see> to make read-only.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see>.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IReadOnlyList<TItem> AsReadOnly<TItem>( this IList<TItem> list )
        {
            Arg.NotNull( list, nameof( list ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<TItem>>() != null );

            if ( list is IReadOnlyList<TItem> readOnlyList )
            {
                return readOnlyList;
            }

            var observable = ( list is INotifyPropertyChanged ) && ( list is INotifyCollectionChanged );

            return observable ? new ReadOnlyObservableList<TItem>( list ) : new ReadOnlyCollection<TItem>( list );
        }

        /// <summary>
        /// Sorts the elements in a list.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <returns>An <see cref="IList{TItem}"/> object.</returns>
        /// <remarks>The sort operation is performed in-place.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IList<TItem> Sort<TItem>( this IList<TItem> list )
        {
            Arg.NotNull( list, nameof( list ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            return Sort( list, 0, list.Count, Comparer<TItem>.Default );
        }

        /// <summary>
        /// Sorts the elements in a list.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to compare items.</param>
        /// <returns>An <see cref="IList{TItem}"/> object.</returns>
        /// <remarks>The sort operation is performed in-place.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static IList<TItem> Sort<TItem>( this IList<TItem> list, IComparer<TItem> comparer )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            return Sort( list, 0, list.Count, comparer );
        }

        /// <summary>
        /// Sorts the elements in a list.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> that compares items.</param>
        /// <returns>An <see cref="IList{TItem}"/> object.</returns>
        /// <remarks>The sort operation is performed in-place.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public static IList<TItem> Sort<TItem>( this IList<TItem> list, Comparison<TItem> comparison )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparison, nameof( comparison ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            return Sort( list, 0, list.Count, new DynamicComparer<TItem>( ( x, y ) => comparison( x, y ) ) );
        }

        /// <summary>
        /// Sorts the elements in a list.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="index">The zero-based index where sorting begins.</param>
        /// <param name="count">The number of items to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> used to compare items.</param>
        /// <returns>An <see cref="IList{TItem}"/> object.</returns>
        /// <remarks>The sort operation is performed in-place.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Validated by code contract." )]
        public static IList<TItem> Sort<TItem>( this IList<TItem> list, int index, int count, IComparer<TItem> comparer )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThan( index, list.Count, nameof( index ) );
            Arg.GreaterThanOrEqualTo( count, 0, nameof( count ) );
            Arg.LessThanOrEqualTo( count, list.Count - index, nameof( count ) );
            Arg.GreaterThanOrEqualTo( checked(index + ( count - 1 )), 0, nameof( index ) );

            if ( list.Count == 0 )
            {
                return list;
            }

            Contract.Assume( list.Count >= 0 );
            ListHelper<TItem>.Sort( list, index, count, comparer );

            return list;
        }

        /// <summary>
        /// Searches a range of elements in a sorted list for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="item">The object to locate.</param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted collection, if <paramref name="item" /> is found;
        /// otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="ICollection{T}.Count"/>.
        /// </returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static int BinarySearch<TItem>( this IList<TItem> list, TItem item )
        {
            Arg.NotNull( list, nameof( list ) );
            return BinarySearch( list, 0, list.Count, item, Comparer<TItem>.Default );
        }

        /// <summary>
        /// Searches a range of elements in a sorted list for an element using the specified comparer and returns the zero-based index
        /// of the element.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="item">The object to locate.</param>
        /// <param name="comparer">The <see cref="IComparer{TItem}" /> implementation to use when comparing elements, or null to use the
        /// default comparer <see cref="Comparer{TItem}.Default" />.
        /// </param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted collection, if <paramref name="item" /> is found;
        /// otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="ICollection{TItem}.Count" />.
        /// </returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static int BinarySearch<TItem>( this IList<TItem> list, TItem item, IComparer<TItem> comparer )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            return BinarySearch( list, 0, list.Count, item, comparer );
        }

        /// <summary>
        /// Searches a range of elements in a sorted list for an element using the specified comparer and returns the zero-based index
        /// of the element.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="item">The object to locate.</param>
        /// <param name="comparison">The <see cref="Comparison{TItem}" /> implementation to use when comparing elements.
        /// </param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted collection, if <paramref name="item" /> is found;
        /// otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="ICollection{TItem}.Count" />.
        /// </returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static int BinarySearch<TItem>( this IList<TItem> list, TItem item, Comparison<TItem> comparison )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparison, nameof( comparison ) );
            return BinarySearch( list, 0, list.Count, item, new DynamicComparer<TItem>( ( x, y ) => comparison( x, y ) ) );
        }

        /// <summary>
        /// Searches a range of elements in a sorted list for an element using the specified comparer and returns the zero-based index
        /// of the element.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to sort.</typeparam>
        /// <param name="list">The extended <see cref="IList{TItem}"/> object.</param>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate.</param>
        /// <param name="comparer">The <see cref="IComparer{TItem}" /> implementation to use when comparing elements, or null to use the
        /// default comparer <see cref="Comparer{TItem}.Default" />.
        /// </param>
        /// <returns>The zero-based index of <paramref name="item" /> in the sorted collection, if <paramref name="item" /> is found;
        /// otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="ICollection{TItem}.Count" />.
        /// </returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "4", Justification = "Validated by code contract." )]
        public static int BinarySearch<TItem>( this IList<TItem> list, int index, int count, TItem item, IComparer<TItem> comparer )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThan( index, list.Count, nameof( index ) );
            Arg.GreaterThanOrEqualTo( count, 0, nameof( count ) );
            Arg.LessThanOrEqualTo( count, list.Count - index, nameof( count ) );
            Arg.GreaterThanOrEqualTo( checked(index + ( count - 1 )), 0, nameof( index ) );

            if ( list.Count == 0 )
            {
                return -1;
            }

            Contract.Assume( list.Count >= 0 );
            var result = ListHelper<TItem>.BinarySearch( list, index, count, item, comparer );

            return result;
        }

        static IList<TItem> InsertRange<TItem>( this IList<TItem> list, IEnumerable<TItem> sequence, int index, int? count )
        {
            Contract.Requires( list != null );
            Contract.Requires( sequence != null );
            Contract.Requires( index >= 0 );
            Contract.Requires( index <= list.Count );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );

            if ( count.HasValue && count.Value == 0 )
            {
                return list;
            }

            var itemCount = 0;
            var source = sequence.ToArray();
            var requestedCount = count ?? source.Length;

            foreach ( var item in source )
            {
                // we need to increment the index or the items will all be inserted at the
                // same position, which will reverse the order of the inserted sequence
                list.Insert( index++, item );

                if ( ++itemCount == requestedCount )
                {
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// Inserts a range of elements into a collection at the specified index.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be inserted.</typeparam>
        /// <param name="list">The extended <see cref="IList{T}"/> object.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of items to insert.</param>
        /// <param name="index">The zero-based index to insert the items.</param>
        /// <returns>An <see cref="IList{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsReadOnly", Justification = "Required for non Code Contract support." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IList<TItem> InsertRange<TItem>( this IList<TItem> list, IEnumerable<TItem> sequence, int index )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThanOrEqualTo( index, list.Count, nameof( index ) );
            return list.InsertRange( sequence, index, null );
        }

        /// <summary>
        /// Inserts a range of elements into a collection at the specified index.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to be inserted.</typeparam>
        /// <param name="list">The extended <see cref="IList{T}"/> object.</param>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of items to insert.</param>
        /// <param name="index">The zero-based index to insert the items.</param>
        /// <param name="count">The number of items from <paramref name="sequence"/> to be added.</param>
        /// <returns>An <see cref="IList{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IsReadOnly", Justification = "Required for non Code Contract support." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public static IList<TItem> InsertRange<TItem>( this IList<TItem> list, IEnumerable<TItem> sequence, int index, int count )
        {
            Arg.NotNull( list, nameof( list ) );
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IList<TItem>>() != null );
            Arg.GreaterThanOrEqualTo( index, 0, nameof( index ) );
            Arg.LessThanOrEqualTo( index, list.Count, nameof( index ) );
            Arg.GreaterThanOrEqualTo( count, 0, nameof( count ) );

            return list.InsertRange( sequence, index, new int?( count ) );
        }
    }
}