namespace More.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Manipulates storage items (files and folders) and their contents, and provides information about them.
    /// </summary>
    [ContractClass( typeof( IStorageItemContract ) )]
    public interface IStorageItem : IEquatable<IStorageItem>
    {
        /// <summary>
        /// Gets the date and time when the current item was created.
        /// </summary>
        /// <value>The date and time when the current item was created.</value>
        DateTimeOffset DateCreated { get; }

        /// <summary>
        /// Gets the name of the item including the file name extension if there is one.
        /// </summary>
        /// <value>The name of the item including the file name extension if there is one.</value>
        string Name { get; }

        /// <summary>
        /// Gets the full file-system path of the item, if the item has a path.
        /// </summary>
        /// <value>The full path of the item, if the item has a path in the user's file-system.</value>
        string Path { get; }

        /// <summary>
        /// Deletes the current item.
        /// </summary>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task DeleteAsync();

        /// <summary>
        /// Gets the basic properties of the current item (like a file or folder).
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the associated <see cref="IBasicProperties">basic properties</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an asynchronous, non-deterministic method." )]
        Task<IBasicProperties> GetBasicPropertiesAsync();

        /// <summary>
        /// Renames the current item.
        /// </summary>
        /// <param name="desiredName">The desired, new name of the item.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task RenameAsync( string desiredName );

        /// <summary>
        /// Gets the parent folder of the current storage item.
        /// </summary>
        /// <returns>When this method completes, it returns the parent folder as a <see cref="IFolder"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an asynchronous, non-deterministic method." )]
        Task<IFolder> GetParentAsync();
    }
}