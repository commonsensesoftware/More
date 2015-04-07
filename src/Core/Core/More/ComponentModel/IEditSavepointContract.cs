namespace More.ComponentModel
{
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IEditSavepoint ) )]
    internal abstract class IEditSavepointContract : IEditSavepoint
    {
        IEditTransaction IEditSavepoint.Transaction
        {
            get
            {
                Contract.Ensures( Contract.Result<IEditTransaction>() != null );
                return null;
            }
        }

        IDictionary<string, object> IEditSavepoint.State
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );
                return null;
            }
        }
    }
}
