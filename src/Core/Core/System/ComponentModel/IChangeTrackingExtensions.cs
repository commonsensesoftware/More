namespace System.ComponentModel
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for objects that support change notification
    /// <seealso cref="ValidatableObject"/>
    /// <seealso cref="IChangeTracking"/>
    /// <seealso cref="IRevertibleChangeTracking"/>.
    /// </summary>
    public static class IChangeTrackingExtensions
    {
        /// <summary>
        /// Returns a value indicating whether all items in the sequence are valid.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of <see cref="ValidatableObject">item</see> to evaluate.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{T}">sequence</see> of items to evaluate.</param>
        /// <returns>True if all of the items in the sequence are valid; otherwise, false.</returns>
        public static bool IsValid<TItem>( this IEnumerable<TItem> items ) where TItem : ValidatableObject
        {
            Arg.NotNull( items, nameof( items ) );
            return items.All( item => item.IsValid );
        }

        /// <summary>
        /// Returns a value indicating whether any items in the sequence have changed.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of <see cref="IChangeTracking">item</see> to evaluate.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{T}">sequence</see> of items to evaluate.</param>
        /// <returns>True if at least one item in the sequence has changed; otherwise, false.</returns>
        public static bool IsChanged<TItem>( this IEnumerable<TItem> items ) where TItem : IChangeTracking
        {
            Arg.NotNull( items, nameof( items ) );
            return items.Any( item => item.IsChanged );
        }

        /// <summary>
        /// Accepts any changes made to items in the specified sequence.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of <see cref="IChangeTracking">item</see> to accept the changes to.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{T}">sequence</see> of items to accept the changes to.</param>
        public static void AcceptChanges<TItem>( this IEnumerable<TItem> items ) where TItem : IChangeTracking
        {
            Arg.NotNull( items, nameof( items ) );
            items.ForEach( item => item.AcceptChanges() );
        }

        /// <summary>
        /// Rejects any changes made to items in the specified sequence.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of <see cref="IRevertibleChangeTracking">item</see> to reject the changes to.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{T}">sequence</see> of items to reject the changes to.</param>
        public static void RejectChanges<TItem>( this IEnumerable<TItem> items ) where TItem : IRevertibleChangeTracking
        {
            Arg.NotNull( items, nameof( items ) );
            items.ForEach( item => item.RejectChanges() );
        }
    }
}
