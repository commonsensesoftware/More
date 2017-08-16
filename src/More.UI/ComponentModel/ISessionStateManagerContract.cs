namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( ISessionStateManager ) )]
    abstract class ISessionStateManagerContract : ISessionStateManager
    {
        IDictionary<string, object> ISessionStateManager.SessionState
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );
                return null;
            }
        }

        void ISessionStateManager.AddKnownType( Type sessionStateType ) =>
            Contract.Requires<ArgumentNullException>( sessionStateType != null, nameof( sessionStateType ) );

        Task ISessionStateManager.RestoreAsync( string sessionKey )
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task ISessionStateManager.SaveAsync()
        {
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }
    }
}