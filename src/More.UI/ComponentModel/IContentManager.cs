namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a content manager.
    /// </summary>
    [ContractClass( typeof( IContentManagerContract ) )]
    public interface IContentManager : INotifyCollectionChanged
    {
        /// <summary>
        /// Gets a sequence of items in the current content.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        IEnumerable<object> Content { get; }

        /// <summary>
        /// Sets the current content with the specified content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to set.</param>
        void SetContent( object content );

        /// <summary>
        /// Adds the specified content to the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to add.</param>
        void AddToContent( object content );

        /// <summary>
        /// Removes the specified content from the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to remove.</param>
        void RemoveFromContent( object content );

        /// <summary>
        /// Clears the current content.
        /// </summary>
        void ClearContent();
    }
}