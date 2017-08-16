namespace System.Collections.Generic
{
    using More;
    using More.Collections.Generic;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for various data structures such as Stacks, Queues, Trees).
    /// </summary>
    public static class DataStructureExtensions
    {
        /// <summary>
        /// Adapts a <see cref="Stack{T}">stack</see> to an <see cref="IStack{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of element in the <paramref name="stack"/>.</typeparam>
        /// <param name="stack">The <see cref="Stack{T}">stack</see> to adapt.</param>
        /// <returns>An <see cref="IStack{T}"/> object.</returns>
        public static IStack<T> Adapt<T>( this Stack<T> stack )
        {
            Arg.NotNull( stack, nameof( stack ) );
            Contract.Ensures( Contract.Result<IStack<T>>() != null );
            return new StackAdapter<T>( stack );
        }

        /// <summary>
        /// Adapts a <see cref="Queue{T}">queue</see> to an <see cref="IQueue{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of element in the <paramref name="queue"/>.</typeparam>
        /// <param name="queue">The <see cref="Queue{T}">queue</see> to adapt.</param>
        /// <returns>An <see cref="IQueue{T}"/> object.</returns>
        public static IQueue<T> Adapt<T>( this Queue<T> queue )
        {
            Arg.NotNull( queue, nameof( queue ) );
            Contract.Ensures( Contract.Result<IQueue<T>>() != null );
            return new QueueAdapter<T>( queue );
        }
    }
}