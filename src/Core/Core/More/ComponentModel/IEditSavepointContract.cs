namespace More.ComponentModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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

        IReadOnlyDictionary<string, object> IEditSavepoint.State
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyDictionary<string, object>>() != null );
                return null;
            }
        }
    }
}
