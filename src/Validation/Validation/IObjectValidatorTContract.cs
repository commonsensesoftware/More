namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    [ContractClassFor( typeof( IObjectValidator<> ) )]
    internal abstract class IObjectValidatorContract<T> : IObjectValidator<T>
    {
        IValidationBuilder<T, TValue> IObjectValidator<T>.Property<TValue>( Expression<Func<T, TValue>> propertyExpression )
        {
            Contract.Requires<ArgumentNullException>( propertyExpression != null, "propertyExpression" );
            Contract.Ensures( Contract.Result<IValidationBuilder<T, TValue>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator<T>.ValidateObject( T instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator<T>.ValidateObject( T instance, IEnumerable<string> propertyNames )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( propertyNames != null, "propertyNames" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance )
        {
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateProperty( string propertyName, object value )
        {
            return null;
        }
    }
}
