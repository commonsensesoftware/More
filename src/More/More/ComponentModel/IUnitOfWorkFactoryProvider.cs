namespace More.ComponentModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object that can locate and provide unit of work factories.
    /// </summary>
    [ContractClass( typeof( IUnitOfWorkFactoryProviderContract ) )]
    public interface IUnitOfWorkFactoryProvider
    {
        /// <summary>
        /// Gets a sequence of unit of work factories supported by the provider.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}">sequence</see> of <see cref="IUnitOfWorkFactory">unit of work factories</see>.</value>
        IEnumerable<IUnitOfWorkFactory> Factories { get; }
    }
}