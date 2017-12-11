namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents the possible edit recovery models.
    /// </summary>
    /// <remarks>This enumeration is meant to be used with types that support <see cref="IEditableObject"/>,
    /// <see cref="IChangeTracking"/>, and <see cref="IRevertibleChangeTracking"/>.</remarks>
    public enum EditRecoveryModel
    {
        /// <summary>
        /// Indicates a simple edit recovery model.
        /// </summary>
        /// <remarks>This recovery model indicates that edits are only recoverable within the transaction scope started
        /// by <see cref="IEditableObject.BeginEdit"/>.  After changes are committed by calling <see cref="IEditableObject.EndEdit"/>,
        /// previous edits cannot be recovered.  <see cref="IEditableObject.CancelEdit"/> can be used to recover changes made
        /// within the current transaction scope.</remarks>
        Simple,

        /// <summary>
        /// Indicates a full edit recovery model.
        /// </summary>
        /// <remarks>This recovery model indicates that edits are recoverable up until the point that <see cref="IChangeTracking.AcceptChanges"/>
        /// is called.  Any number of transacted edit operations can occur using <see cref="IEditableObject.BeginEdit"/>, <see cref="IEditableObject.EndEdit"/>,
        /// and <see cref="IEditableObject.CancelEdit"/>.  Once <see cref="IChangeTracking.AcceptChanges"/> is called, the changes are permenantly
        /// committed until the next call to <see cref="IChangeTracking.AcceptChanges"/>.  <see cref="IRevertibleChangeTracking.RejectChanges"/> can be used
        /// to recover all changes made until the last time <see cref="IChangeTracking.AcceptChanges"/> was called, which could be the initial state of
        /// the object.</remarks>
        Full,
    }
}