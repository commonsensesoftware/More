namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( ISpecification<> ) )]
    internal abstract class ISpecificationContract<T> : ISpecification<T>
    {
        bool ISpecification<T>.IsSatisfiedBy( T item )
        {
            return default( bool );
        }

        ISpecification<T> ISpecification<T>.And( ISpecification<T> other )
        {
            Contract.Requires<ArgumentNullException>( other != null, "other" );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return default( ISpecification<T> );
        }

        ISpecification<T> ISpecification<T>.Or( ISpecification<T> other )
        {
            Contract.Requires<ArgumentNullException>( other != null, "other" );
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return default( ISpecification<T> );
        }

        ISpecification<T> ISpecification<T>.Not()
        {
            Contract.Ensures( Contract.Result<ISpecification<T>>() != null );
            return default( ISpecification<T> );
        }

        bool IRule<T, bool>.Evaluate( T item )
        {
            return default( bool );
        }
    }
}
