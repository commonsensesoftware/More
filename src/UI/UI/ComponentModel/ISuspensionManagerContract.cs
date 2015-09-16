namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( ISuspensionManager ) )]
    internal abstract class ISuspensionManagerContract : ISuspensionManager
    {
        IDictionary<string, object> ISuspensionManager.SessionState
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );
                return null;
            }
        }

        void ISuspensionManager.AddKnownType( Type sessionStateType )
        {
            Contract.Requires<ArgumentNullException>( sessionStateType != null, nameof( sessionStateType ) );
        }

        Task ISuspensionManager.RestoreAsync( string sessionKey )
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task ISuspensionManager.SaveAsync()
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }
    }
}
