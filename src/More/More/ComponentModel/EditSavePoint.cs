namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an edit transaction savepoint.
    /// </summary>
    sealed class EditSavePoint : IEditSavepoint
    {
        internal EditSavePoint( IEditTransaction transaction, IReadOnlyDictionary<string, object> state )
        {
            Contract.Requires( transaction != null );
            Contract.Requires( state != null );

            Transaction = transaction;
            State = state;
        }

        public IEditTransaction Transaction { get; }

        public IReadOnlyDictionary<string, object> State { get; }
    }
}