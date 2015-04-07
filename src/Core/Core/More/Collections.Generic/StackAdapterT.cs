namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Provides an adapter for the <see cref="Stack{T}"/> class to the <see cref="IStack{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the stack.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is an adapter for a stack." )]
    public sealed class StackAdapter<T> : IStack<T>
    {
        private readonly Stack<T> stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackAdapter{T}"/> class.
        /// </summary>
        /// <param name="innerStack">The <see cref="Stack{T}">stack</see> to adapt to.</param>
        public StackAdapter( Stack<T> innerStack )
        {
            Contract.Requires<ArgumentNullException>( innerStack != null, "innerStack" );
            this.stack = innerStack;
        }

        /// <summary>
        /// Returns the object at the top of the <see cref="StackAdapter{T}">stack</see> without removing it.
        /// </summary>
        /// <returns>The object at the top of the <see cref="StackAdapter{T}" />.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="Pop"/> method, but <seealso cref="Peek"/> does not modify the <see cref="StackAdapter{T}">stack</see>.</para>
        /// <para>A <c>null</c>can be pushed onto the <see cref="StackAdapter{T}">stack</see> as a placeholder, if needed.</para>
        /// </remarks>
        public T Peek()
        {
            return this.stack.Peek();
        }

        /// <summary>
        /// Removes and returns the object at the top of the <see cref="StackAdapter{T}">stack</see>.
        /// </summary>
        /// <returns>The object removed from the top of the <see cref="StackAdapter{T}">stack</see>.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="Peek"/> method, but <seealso cref="Peek"/> does not modify the <see cref="StackAdapter{T}">stack</see>.</para>
        /// <para>A <c>null</c>could be popped from the <see cref="StackAdapter{T}">stack</see>.</para>
        /// </remarks>
        public T Pop()
        {
            return this.stack.Pop();
        }

        /// <summary>
        /// Inserts an object at the top of the <see cref="StackAdapter{T}">stack</see>.
        /// </summary>
        /// <param name="item">The object to push onto the <see cref="StackAdapter{T}">stack</see>. The value can be <see langkeyword="null">null</see>.</param>
        /// <remarks>
        /// <para>If the <see cref="Count">count</see> already equals the capacity, the capacity of the <see cref="StackAdapter{T}">stack</see>
        /// is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the
        /// new element is added.</para>
        /// <para>A <c>null</c>can be pushed onto the <seealso cref="StackAdapter{T}">stack</seealso> as a placeholder,
        /// if needed.</para>
        /// </remarks>
        public void Push( T item )
        {
            this.stack.Push( item );
        }

        /// <summary>
        /// Removes all objects from the <see cref="StackAdapter{T}">stack</see>.
        /// </summary>
        public void Clear()
        {
            this.stack.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="StackAdapter{T}">stack</see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="StackAdapter{T}">stack</see>. The value can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the <see cref="StackAdapter{T}">stack</see>;
        /// otherwise, <see langkeyword="false">false</see>.</returns>
        public bool Contains( T item )
        {
            return this.stack.Contains( item );
        }

        /// <summary>
        /// Copies the elements of the <see cref="StackAdapter{T}">stack</see> to an <see cref="Array">array</see>,
        /// starting at a particular <see cref="Array">array</see> index.
        /// </summary>
        /// <remarks>
        /// <para>The elements are copied onto the array in last-in-first-out (LIFO) order, similar to the order
        /// of the elements returned by a succession of calls to <see cref="Pop"/>.</para>
        /// </remarks>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the elements copied
        /// from the <see cref="StackAdapter{T}">stack</see>. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void CopyTo( T[] array, int arrayIndex )
        {
            this.stack.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="StackAdapter{T}">stack</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the <see cref="StackAdapter{T}">stack</see>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by the adapted object." )]
        void ICollection.CopyTo( Array array, int index )
        {
            ( (ICollection) this.stack ).CopyTo( array, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="StackAdapter{T}">stack</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="StackAdapter{T}">stack</see>.</value>
        public int Count
        {
            get
            {
                return this.stack.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ( (ICollection) this.stack ).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ( (ICollection) this.stack ).SyncRoot;
            }
        }
    }
}