namespace More.ComponentModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IUnitOfWorkFactoryProvider ) )]
    abstract class IUnitOfWorkFactoryProviderContract : IUnitOfWorkFactoryProvider
    {
        IEnumerable<IUnitOfWorkFactory> IUnitOfWorkFactoryProvider.Factories
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<IUnitOfWorkFactory>>() != null );
                return null;
            }
        }
    }
}