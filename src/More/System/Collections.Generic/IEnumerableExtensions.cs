namespace System.Collections.Generic
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to project the supplied <paramref name="action"/> over.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}">sequence</see>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static void ForEach<TItem>( this IEnumerable<TItem> sequence, Action<TItem> action )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( action, nameof( action ) );

            foreach ( var item in sequence )
            {
                action( item );
            }
        }

        /// <summary>
        /// Provides a fluent interface to the <see cref="ForEach{T}(IEnumerable{T},System.Action{T})">ForEach</see> method.
        /// </summary>
        /// <remarks>
        /// The supplied <paramref name="action"/> will be executed for each item while iterating the result. The entire <paramref name="sequence"/> is only
        /// evaluated if the iterator is completly executed across all members. Any exception thrown by <paramref name="action"/> will usually cease all
        /// evaluatation of the iterator, therefore, the supplied <see cref="Action{T}"/> should be side effect free on the original source sequence.
        /// </remarks>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to project the supplied <paramref name="action"/> over.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}">sequence</see>.</param>
        /// <returns>A new <see cref="IEnumerable{T}">sequence</see> with the same items as the source after evaluation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static IEnumerable<TItem> With<TItem>( this IEnumerable<TItem> sequence, Action<TItem> action )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( action, nameof( action ) );
            Contract.Ensures( Contract.Result<IEnumerable<TItem>>() != null );

            foreach ( var item in sequence )
            {
                action( item );
                yield return item;
            }
        }

        /// <summary>
        /// Searches for the specified <paramref name="item"/> and returns the index of the first occurrence within the <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to search.</param>
        /// <param name="item">The object to locate in <paramref name="sequence"/>.</param>
        /// <returns>The zero-based index the specified item or -1 if no match is found.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int IndexOf<TItem>( this IEnumerable<TItem> sequence, TItem item )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );
            return sequence.IndexOf( item, EqualityComparer<TItem>.Default );
        }

        /// <summary>
        /// Searches for the specified <paramref name="item"/> and returns the index of the first occurrence within the <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to search.</param>
        /// <param name="item">The object to locate in <paramref name="sequence"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{TItem}"/> used to compare items for equality.</param>
        /// <returns>The zero-based index the specified item or -1 if no match is found.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by code contract." )]
        public static int IndexOf<TItem>( this IEnumerable<TItem> sequence, TItem item, IEqualityComparer<TItem> comparer )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( comparer, nameof( comparer ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );

            var index = 0;

            foreach ( var element in sequence )
            {
                if ( comparer.Equals( element, item ) )
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements up to the limit specified in <paramref name="pageSize"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to return elements from.</param>
        /// <param name="pageNumber">The number pages of elements to skip before returning the remaining elements. The index is 0 based.</param>
        /// <param name="pageSize">The number of elements to return in a single page. The index is 1 based.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> that contains the specified number of elements from the start of the input sequence from
        /// <paramref name="pageNumber"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IEnumerable<TItem> Between<TItem>( this IEnumerable<TItem> sequence, int pageNumber, int pageSize )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable<TItem>>() != null );
            Arg.GreaterThanOrEqualTo( pageNumber, 0, nameof( pageNumber ) );
            Arg.GreaterThan( pageSize, 0, nameof( pageSize ) );

            var startIndex = pageNumber * pageSize;
            return sequence.Skip( startIndex ).Take( pageSize );
        }

        /// <summary>
        /// Calculates the number of subsets for a sequence based on a particular page size .
        /// </summary>
        /// <remarks>
        /// <para>This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that
        /// is required to perform the action. The query represented by this method is not executed until the object is enumerated either by calling
        /// its <seealso cref="IEnumerable{T}.GetEnumerator"/> method directly or by using <see langkeyword="foreach">foreach</see>.</para>
        /// </remarks>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to return elements from.</param>
        /// <param name="pageSize">The number of elements to evaludate in a single page. The index is 1 based.</param>
        /// <returns>A count that contains the calculated number of subsets that are contained in <paramref name="sequence"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static int PageCount<TItem>( this IEnumerable<TItem> sequence, int pageSize )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<int>() >= 0 );
            Arg.GreaterThan( pageSize, 0, nameof( pageSize ) );

            var count = sequence.Count();
            var pageCount = count / pageSize;
            return count % pageSize != 0 ? pageCount + 1 : pageCount;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first
        /// occurrence within the entire <see cref="IEnumerable{T}">sequence</see>.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="IEnumerable{T}">sequence</see> is searched forward starting at the first element and ending at the last element.</para>
        /// <para>The <see cref="Predicate{T}"/> is a delegate to a method that returns <see langkeyword="true">true</see> if
        /// the object passed to it matches the conditions defined in the delegate. The elements of the current <see cref="IEnumerable{T}">sequence</see> are
        /// individually passed to the <seealso cref="Predicate{T}"/> delegate.</para>
        /// <para>This method performs a linear search; therefore, this method is an O(n) operation, where n is
        /// <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})">Count</see>.</para>
        /// </remarks>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to project the supplied <paramref name="predicate"/> over.</param>
        /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>,
        /// if found; otherwise, –1.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public static int FindIndex<TItem>( this IEnumerable<TItem> sequence, Func<TItem, bool> predicate )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( predicate, nameof( predicate ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );
            return sequence.FindIndex( 0, null, predicate );
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first
        /// occurrence within range of elements in the <see cref="IEnumerable{T}">sequence</see> that extends from the specified index to the last element.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="IEnumerable{T}">sequence</see> is searched forward starting at the first element and ending at the last element.</para>
        /// <para>The <see cref="Predicate{T}"/> is a delegate to a method that returns <see langkeyword="true">true</see> if the object passed to it
        /// matches the conditions defined in the delegate. The elements of the current <see cref="IEnumerable{T}">sequence</see> are individually passed to the
        /// <seealso cref="Predicate{T}"/> delegate.</para>
        /// <para>This method performs a linear search; therefore, this method is an O(n) operation, where n is
        /// <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})">Count</see> - <paramref name="startIndex"/>.</para>
        /// </remarks>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to project the supplied <paramref name="predicate"/> over.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>,
        /// if found; otherwise, –1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="startIndex">start index</paramref> is greater than or equal to the length of the <paramref name="sequence"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        public static int FindIndex<TItem>( this IEnumerable<TItem> sequence, int startIndex, Func<TItem, bool> predicate )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( predicate, nameof( predicate ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );
            Arg.GreaterThanOrEqualTo( startIndex, 0, nameof( startIndex ) );
            return sequence.FindIndex( startIndex, null, predicate );
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first
        /// occurrence within the range of elements in the <see cref="IEnumerable{T}">sequence</see> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="IEnumerable{T}">sequence</see> is searched forward starting at the <paramref name="startIndex"/> and ending at <paramref name="count"/>.</para>
        /// <para>The <see cref="Predicate{T}"/> is a delegate to a method that returns <see langkeyword="true">true</see> if the object passed to it
        /// matches the conditions defined in the delegate. The elements of the current <see cref="IEnumerable{T}">sequence</see> are individually passed to the
        /// <seealso cref="Predicate{T}"/> delegate.</para>
        /// <para>This method performs a linear search; therefore, this method is an O(n) operation, where n is
        /// <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})">Count</see> - <paramref name="startIndex"/> or <paramref name="count"/>, whichever is less.</para>
        /// </remarks>
        /// <typeparam name="TItem">The type of each element in the sequence.</typeparam>
        /// <param name="sequence">The sequence to project the supplied <paramref name="predicate"/> over.</param>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>,
        /// if found; otherwise, –1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="startIndex">start index</paramref> is greater than or equal to the length of the <paramref name="sequence"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="count"/> greater than the length of the <paramref name="sequence"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Validated by a code contract." )]
        public static int FindIndex<TItem>( this IEnumerable<TItem> sequence, int startIndex, int count, Func<TItem, bool> predicate )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( predicate, nameof( predicate ) );
            Contract.Ensures( Contract.Result<int>() >= -1 );
            Arg.GreaterThanOrEqualTo( startIndex, 0, nameof( startIndex ) );
            Arg.GreaterThanOrEqualTo( count, 0, nameof( count ) );
            return sequence.FindIndex( startIndex, new int?( count ), predicate );
        }

        private static int FindIndex<TItem>( this IEnumerable<TItem> sequence, int startIndex, int? count, Func<TItem, bool> predicate )
        {
            Contract.Requires( sequence != null );
            Contract.Requires( startIndex >= 0 );
            Contract.Requires( predicate != null );
            Contract.Ensures( Contract.Result<int>() >= -1 );

            var list = sequence as IList<TItem> ?? sequence.ToArray();

            if ( startIndex >= list.Count )
            {
                throw new ArgumentOutOfRangeException( nameof( startIndex ) );
            }

            var size = count ?? list.Count;

            if ( size > list.Count )
            {
                throw new ArgumentOutOfRangeException( nameof( count ) );
            }

            var i = startIndex;
            var itemCount = startIndex + size;
            var iterator = list.Skip( startIndex ).Take( itemCount ).GetEnumerator();

            while ( iterator.MoveNext() )
            {
                if ( predicate( iterator.Current ) )
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        /// <summary>
        /// Projects a sequence of sequences and flattens the resulting sequences into one sequence, including the starting sequence as an item.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item in the sequence.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of sequences to flatten.</param>
        /// <returns>A flattened <see cref="IEnumerable{T}">sequence</see> of sequences.</returns>
        /// <remarks>The extended <paramref name="sequence"/> itself is included as the first item in the results.</remarks>
        /// <example>The following example demonstrates creating a hierarchy of nested sequences and then flattening it into
        /// a single sequence, including the root node, which is written to the standard console output.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        ///
        /// public static void Main( params string[] args )
        /// {
        ///     // create nested sequences of nodes
        ///     var node = new Node<string>( "1" )
        ///     {
        ///         new Node<string>( "1.1" )
        ///         {
        ///             new Node<string>( "1.1.1" )
        ///         },
        ///         new Node<string>( "1.2" )
        ///     };
        ///
        ///     // flatten all nodes, including the root, and write them to the console
        ///     node.SelfAndFlatten().ForEach( n => Console.WriteLine( n.Value ) );
        /// }
        /// ]]>
        /// </code>
        /// <para>
        /// Output
        /// ----------
        /// 1
        /// 1.1
        /// 1.1.1
        /// 1.2
        /// </para>
        /// </example>
        public static IEnumerable<TItem> SelfAndFlatten<TItem>( this IEnumerable<TItem> sequence ) where TItem : IEnumerable<TItem>
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable<TItem>>() != null );

            var self = new[] { (TItem) sequence };
            return self.Union( sequence.Flatten() );
        }

        /// <summary>
        /// Projects a sequence of sequences and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item in the sequence.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of sequences to flatten.</param>
        /// <returns>A flattened <see cref="IEnumerable{T}">sequence</see> of sequences.</returns>
        /// <remarks>The extended <paramref name="sequence"/> itself is not included in the results.</remarks>
        /// <example>The following example demonstrates creating a hierarchy of nested sequences and then flattening it into
        /// a single sequence, which is written to the standard console output.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        ///
        /// public static void Main( params string[] args )
        /// {
        ///     // create nested sequences of nodes
        ///     var node = new Node<string>( "1" )
        ///     {
        ///         new Node<string>( "1.1" )
        ///         {
        ///             new Node<string>( "1.1.1" )
        ///         },
        ///         new Node<string>( "1.2" )
        ///     };
        ///
        ///     // flatten all children and write them to the console
        ///     node.Flatten().ForEach( n => Console.WriteLine( n.Value ) );
        /// }
        /// ]]>
        /// </code>
        /// <para>
        /// Output
        /// ----------
        /// 1.1
        /// 1.1.1
        /// 1.2
        /// </para>
        /// </example>
        public static IEnumerable<TItem> Flatten<TItem>( this IEnumerable<TItem> sequence ) where TItem : IEnumerable<TItem>
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Contract.Ensures( Contract.Result<IEnumerable<TItem>>() != null );

            // NOTE: this could be achieved using the following, which would avoid the need to initialize a
            // a new array to union with the remaining sequences; however, the final sequence order will be
            // different from the other versions of Flatten, which could be confusing to consumers.
            //
            // return sequence.Union( sequence.SelectMany( i => i.Flatten() ) );
            return sequence.SelectMany( i => new[] { i }.Union( i.Flatten() ) );
        }

        /// <summary>
        /// Projects a sequence of sequences and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item in the sequence.</typeparam>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> of sequences to flatten.</param>
        /// <param name="selector">The selector <see cref="Func{T1,TResult}">function</see> used to project nested sequences.</param>
        /// <returns>A flattened <see cref="IEnumerable{T}">sequence</see> of sequences.</returns>
        /// <example>This example illustrates how to flatten a hierarchy of items where each item is not a collection itself. The
        /// related children can be traversed using the specified selector function.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        ///
        /// public class TreeNode<T>
        /// {
        ///     private readonly List<TreeNode<T>> children = new List<TreeNode<T>>();
        ///
        ///     public TreeNode()
        ///     {
        ///     }
        ///
        ///     public TreeNode( T value )
        ///     {
        ///         this.Value = value;
        ///     }
        ///
        ///     public T Value
        ///     {
        ///         get;
        ///         set;
        ///     }
        ///
        ///     public IList<TreeNode<T>> Children
        ///     {
        ///         get
        ///         {
        ///             return this.children;
        ///         }
        ///     }
        /// }
        ///
        /// public static void Main( params string[] args )
        /// {
        ///     var root = new TreeNode<string>( "1" )
        ///     {
        ///         Children =
        ///         {
        ///             new TreeNode<string>( "1.1" )
        ///             {
        ///                 Children =
        ///                 {
        ///                     new TreeNode<string>( "1.1.1" )
        ///                 }
        ///             },
        ///             new TreeNode<string>( "1.2" )
        ///         }
        ///     };
        ///     var nodes = new List<TreeNode<string>>();
        ///     nodes.Add( root );
        ///
        ///     // flatten all nodes in the list, their children, and write them to the console
        ///     nodes.Flatten( n => n.Children ).ForEach( n => Console.WriteLine( n.Value ) );
        /// }
        ///
        /// ]]>
        /// </code>
        /// <para>
        /// Output
        /// ----------
        /// 1
        /// 1.1
        /// 1.1.1
        /// 1.2
        /// </para>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public static IEnumerable<TItem> Flatten<TItem>( this IEnumerable<TItem> sequence, Func<TItem, IEnumerable<TItem>> selector )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( selector, nameof( selector ) );
            Contract.Ensures( Contract.Result<IEnumerable<TItem>>() != null );

            // NOTE: this could be achieved using the following, which would avoid the need to initialize a
            // a new array to union with the remaining sequences; however, the final sequence order will be
            // different from the other versions of Flatten, which could be confusing to developers.
            //
            // return sequence.Union( sequence.SelectMany( i => selector( i ).Flatten( selector ) ) );
            return sequence.SelectMany( i => new[] { i }.Union( selector( i ).Flatten( selector ) ) );
        }
    }
}