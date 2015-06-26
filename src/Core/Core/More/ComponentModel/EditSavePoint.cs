namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an edit transaction savepoint.
    /// </summary>
    internal sealed class EditSavePoint : IEditSavepoint
    {
        private readonly IEditTransaction transaction;
        private readonly IReadOnlyDictionary<string, object> state;

        internal EditSavePoint( IEditTransaction transaction, IReadOnlyDictionary<string, object> state )
        {
            Contract.Requires( transaction != null );
            Contract.Requires( state != null );
            this.transaction = transaction;
            this.state = state;
        }

        public IEditTransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }

        public IReadOnlyDictionary<string, object> State
        {
            get
            {
                return this.state;
            }
        }
    }
}
