namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides an adapter for the <see cref="Queue{T}"/> class to the <see cref="IQueue{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the queue.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is an adapter for a queue." )]
    public sealed class QueueAdapter<T> : IQueue<T>
    {
        private readonly Queue<T> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAdapter{T}"/> class.
        /// </summary>
        /// <param name="innerQueue">The <see cref="Queue{T}">queue</see> to adapt to.</param>
        public QueueAdapter( Queue<T> innerQueue )
        {
            Arg.NotNull( innerQueue, "innerQueue" );
            this.queue = innerQueue;
        }

        /// <summary>
        /// Returns the object at the begining of the <see cref="QueueAdapter{T}">queue</see> without removing it.
        /// </summary>
        /// <returns>The object at the begining of the <see cref="QueueAdapter{T}">queue</see>.</returns>
        public T Peek()
        {
            if ( this.Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );

            Contract.EndContractBlock();
            return this.queue.Peek();
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="IQueue{T}">queue</see>.
        /// </summary>
        /// <returns>The object removed from the begining of the <see cref="IQueue{T}" />.</returns>
        public T Dequeue()
        {
            if ( this.Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );
            
            Contract.EndContractBlock();
            return this.queue.Dequeue();
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="IQueue{T}">queue</see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IQueue{T}">queue</see>.</param>
        public void Enqueue( T item )
        {
            this.queue.Enqueue( item );
        }

        /// <summary>
        /// Removes all objects from the <see cref="QueueAdapter{T}">queue</see>.
        /// </summary>
        public void Clear()
        {
            this.queue.Clear();
        }

        /// <summary>
        /// Copies the elements of the <see cref="QueueAdapter{T}">queue</see> to an <see cref="Array">array</see>,
        /// starting at a particular <seealso cref="Array">array</seealso> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the elements
        /// copied from the <see cref="QueueAdapter{T}">queue</see>. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>        
        /// <remarks>
        /// <para>The elements are copied onto the array in first-in-first-out (FIFO) order, similar to the order
        /// of the elements returned by a succession of calls to <see cref="Dequeue"/>.</para>
        /// </remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void CopyTo( T[] array, int arrayIndex )
        {
            Arg.NotNull( array, "array" );
            Arg.InRange( arrayIndex, 0, array.Length + this.Count, "arrayIndex" );

            this.queue.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Determines whether the <see cref="QueueAdapter{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="QueueAdapter{T}">queue</see>.
        /// The value can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the
        /// <see cref="QueueAdapter{T}">queue</see>; otherwise, <see langkeyword="false">false</see>.</returns>
        public bool Contains( T item )
        {
            return this.queue.Contains( item );
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="QueueAdapter{T}">queue</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the <see cref="QueueAdapter{T}">queue</see>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by the adapted object." )]
        void ICollection.CopyTo( Array array, int index )
        {
            ( (ICollection) this.queue ).CopyTo( array, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="QueueAdapter{T}">queue</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="QueueAdapter{T}">queue</see>.</value>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ( (ICollection) this.queue ).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ( (ICollection) this.queue ).SyncRoot;
            }
        }
    }
}
