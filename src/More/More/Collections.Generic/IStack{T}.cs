namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a stack data structure.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the stack.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an appropriate name." )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A stack is a well-known collection type." )]
    [ContractClass( typeof( IStackContract<> ) )]
#pragma warning disable CA1010 // Collections should implement generic interface
    public interface IStack<T> : IReadOnlyCollection<T>, ICollection
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        /// <summary>
        /// Returns the item at the top of the <see cref="IStack{T}" /> without removing it.
        /// </summary>
        /// <remarks>This method is similar to the <see cref="IStack{T}.Pop"/> method, but
        /// <see cref="IStack{T}.Peek"/> does not modify the <see cref="IStack{T}" />.</remarks>
        /// <returns>The item at the top of the <see cref="IStack{T}" />.</returns>
        T Peek();

        /// <summary>
        /// Removes and returns the item at the top of the <see cref="IStack{T}" />.
        /// </summary>
        /// <remarks>This method is similar to the <see cref="IStack{T}.Peek"/> method, but
        /// <see cref="IStack{T}.Peek"/> does not modify the <see cref="IStack{T}" />.</remarks>
        /// <returns>The item removed from the top of the <see cref="IStack{T}" />.</returns>
        T Pop();

        /// <summary>
        /// Adds an item to the top of the <see cref="IStack{T}" />.
        /// </summary>
        /// <param name="item">The item to push onto the <see cref="IStack{T}" />.</param>
        void Push( T item );

        /// <summary>
        /// Removes all items from the <see cref="IStack{T}" />.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the <see cref="ObservableStack{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The item to locate in the <see cref="IStack{T}" />. The value can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the <see cref="IStack{T}" />;
        /// otherwise, <see langkeyword="false">false</see>.</returns>
        bool Contains( T item );

        /// <summary>
        /// Copies the items of the <see cref="IStack{T}" /> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the items copied from the <see cref="IStack{T}" />.
        /// The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        void CopyTo( T[] array, int arrayIndex );
    }
}