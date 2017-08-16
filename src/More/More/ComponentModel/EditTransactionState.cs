namespace More.ComponentModel
{
    using System;

    /// <summary>
    /// Represents the possible edit transaction states.
    /// </summary>
    public enum EditTransactionState
    {
        /// <summary>
        /// Indicates an edit transaction has not been started.  This is the initial state.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Indicates an edit transaction has been started.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates an edit transaction has been committed.
        /// </summary>
        Committed,

        /// <summary>
        /// Indicates an edit transaction was rolled back.
        /// </summary>
        RolledBack
    }
}