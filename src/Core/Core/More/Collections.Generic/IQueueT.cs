namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a queue data structure.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the queue.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an appropriate name." )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A queue is a well-known collection type." )]
    [ContractClass( typeof( IQueueContract<> ) )]
    public interface IQueue<T> : IEnumerable<T>, ICollection
    {
        /// <summary>
        /// Returns the item at the begining of the <see cref="IQueue{T}" /> without removing it.
        /// </summary>
        /// <remarks>This method is similar to the <see cref="M:IQueue{T}.Dequeue"/> method, but
        /// <see cref="M:IQueue{T}.Peek"/> does not modify the <see cref="IQueue{T}" />.</remarks>
        /// <returns>The item at the beginning of the <see cref="IQueue{T}" />.</returns>
        T Peek();

        /// <summary>
        /// Removes and returns the item at the beginning of the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <remarks>This method is similar to the <see cref="M:IQueue{T}.Peek"/> method, but
        /// <see cref="M:IQueue{T}.Peek"/> does not modify the <see cref="IQueue{T}" />.</remarks>
        /// <returns>The item removed from the beginning of the <see cref="IQueue{T}" />.</returns>
        T Dequeue();

        /// <summary>
        /// Adds an item to the end of the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="IQueue{T}"/>.</param>
        void Enqueue( T item );

        /// <summary>
        /// Removes all items from the <see cref="ObservableQueue{T}" />.
        /// </summary>
        void Clear();

        /// <summary>
        /// Copies the items of the <see cref="ObservableQueue{T}" /> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ObservableQueue{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <remarks>The items are copied onto the array in first-in-first-out (FIFO) order, similar to the order of the items returned
        /// by a succession of calls to <see cref="M:IQueue{T}.Dequeue"/>.</remarks>
        void CopyTo( T[] array, int arrayIndex );

        /// <summary>
        /// Determines whether the <see cref="ObservableQueue{T}"/> contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate in the <see cref="IQueue{T}" />. The item can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the <see cref="IQueue{T}" />; otherwise, <see langkeyword="false">false</see>.</returns>
        bool Contains( T item );
    }
}
