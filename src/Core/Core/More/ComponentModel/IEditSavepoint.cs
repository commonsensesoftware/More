namespace More.ComponentModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts; 

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
        /// <value>A <see cref="IReadOnlyDictionary{TKey,TValue}">read-only dictionary</see>.</value>
        IReadOnlyDictionary<string, object> State
        {
            get;
        }
    }
}
