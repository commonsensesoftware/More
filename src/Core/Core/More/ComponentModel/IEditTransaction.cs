namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Defines the behavior of an edit transaction.
    /// </summary>
    [ContractClass( typeof( IEditTransactionContract ) )] 
    public interface IEditTransaction
    {
        /// <summary>
        /// Begins an edit transaction.
        /// </summary>
        void Begin();

        /// <summary>
        /// Commits an edit transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Creates an edit savepoint.
        /// </summary>
        /// <returns>An <see cref="IEditSavepoint"/> object.</returns>
        /// <remarks>This method is intended to be invoked inside or outside of an edit transaction.</remarks>
        IEditSavepoint CreateSavepoint();

        /// <summary>
        /// Rolls back an edit transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rolls back an edit transaction to the specified savepoint.
        /// </summary>
        /// <param name="savepoint">The <see cref="IEditSavepoint">savepoint</see> to roll back to.</param>
        void Rollback( IEditSavepoint savepoint );
    }
}
