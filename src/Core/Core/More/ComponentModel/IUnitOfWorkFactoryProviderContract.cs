namespace More.ComponentModel
{
    using global::System.Collections.Generic;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IUnitOfWorkFactoryProvider ) )]
    internal abstract class IUnitOfWorkFactoryProviderContract : IUnitOfWorkFactoryProvider
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
