namespace More.VisualStudio
{
    using System;

    /// <summary>
    /// Defines the behavior of a project item name validator.
    /// </summary>
    public interface IProjectItemNameValidator
    {
        /// <summary>
        /// Returns a value indicating whether the specified item name is unique.
        /// </summary>
        /// <param name="itemName">The item name to evaluate.</param>
        /// <returns>True if the item name is unique; otherwise, false.</returns>
        bool IsItemNameUnique( string itemName );

        /// <summary>
        /// Returns a value indicating whether the specified connection string name is unique.
        /// </summary>
        /// <param name="name">The connection string name to evaluate.</param>
        /// <returns>True if the connection string name is unique; otherwise, false.</returns>
        bool IsConnectionStringNameUnique( string name );
    }
}