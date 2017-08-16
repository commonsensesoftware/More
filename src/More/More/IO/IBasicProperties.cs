namespace More.IO
{
    using System;

    /// <summary>
    /// Provides access to the basic properties, like the size of the item or the date the item was last modified, of the item (like a file or folder).
    /// </summary>
    public interface IBasicProperties : IStorageItemProperties
    {
        /// <summary>
        /// Gets the timestamp of the last time the file was modified.
        /// </summary>
        /// <value>The <see cref="DateTimeOffset">date and time</see> the item was modified.</value>
        DateTimeOffset DateModified { get; }

        /// <summary>
        /// Gets the most relevant date for the item.
        /// </summary>
        /// <value>The item's <see cref="DateTimeOffset">date</see>, such as when
        /// the item was created.</value>
        DateTimeOffset ItemDate { get; }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        long Size { get; }
    }
}