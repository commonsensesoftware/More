namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IUnitOfWorkFactory ) )]
    internal abstract class IUnitOfWorkFactoryContract : IUnitOfWorkFactory
    {
        ISpecification<Type> IUnitOfWorkFactory.Specification
        {
            get
            {
                Contract.Ensures( Contract.Result<ISpecification<Type>>() != null );
                return null;
            }
        }

        IUnitOfWork<TItem> IUnitOfWorkFactory.Create<TItem>()
        {
            Contract.Requires<InvalidOperationException>( ( (IUnitOfWorkFactory) this ).Specification.IsSatisfiedBy( typeof( TItem ) ) );
            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );
            return null;
        }

        IUnitOfWork<TItem> IUnitOfWorkFactory.GetCurrent<TItem>()
        {
            Contract.Requires<InvalidOperationException>( ( (IUnitOfWorkFactory) this ).Specification.IsSatisfiedBy( typeof( TItem ) ) );
            Contract.Ensures( Contract.Result<IUnitOfWork<TItem>>() != null );
            return null;
        }

        void IUnitOfWorkFactory.SetCurrent<TItem>( IUnitOfWork<TItem> unitOfWork )
        {
            Contract.Requires<InvalidOperationException>( ( (IUnitOfWorkFactory) this ).Specification.IsSatisfiedBy( typeof( TItem ) ) );
            Contract.Requires<ArgumentNullException>( unitOfWork != null, "unitOfWork" );
        }
    }
}
