namespace More.ComponentModel
{
    using System;

    /// <summary>
    /// Represents the snapshot strategy for an edited value.
    /// </summary>
    public interface IEditSnapshotStrategy
    {
        /// <summary>
        /// Returns the snapshot of an object.
        /// </summary>
        /// <param name="value">The object to get a snapshot of.</param>
        /// <returns>A snapshot of the specified object, which will most likely be implemented using cloning.</returns>
        object GetSnapshot( object value );
    }
}