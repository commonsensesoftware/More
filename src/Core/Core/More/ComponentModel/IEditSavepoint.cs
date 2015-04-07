namespace More.ComponentModel
{
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts; 

    /// <summary>
    /// Defines the behavior of an edit transaction savepoint.
    /// </summary>
    [ContractClass( typeof( IEditSavepointContract ) )] 
    public interface IEditSavepoint
    {
        /// <summary>
        /// Gets the transaction associated with the savepoint.
        /// </summary>
        /// <value>An <see cref="IEditTransaction"/> object.</value>
        IEditTransaction Transaction
        {
            get;
        }

        /// <summary>
        /// Gets the state saved in the savepoint.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
        IDictionary<string, object> State
        {
            get;
        }
    }
}
