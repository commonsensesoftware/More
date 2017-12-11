namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    [ContractClassFor( typeof( IObjectValidator<> ) )]
    abstract class IObjectValidatorContract<T> : IObjectValidator<T>
    {
        IValidationBuilder<T, TValue> IObjectValidator<T>.Property<TValue>( Expression<Func<T, TValue>> propertyExpression )
        {
            Contract.Requires<ArgumentNullException>( propertyExpression != null, nameof( propertyExpression ) );
            Contract.Ensures( Contract.Result<IValidationBuilder<T, TValue>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator<T>.ValidateObject( T instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator<T>.ValidateObject( T instance, IEnumerable<string> propertyNames )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Requires<ArgumentNullException>( propertyNames != null, nameof( propertyNames ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance ) => null;

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance, IEnumerable<string> propertyNames ) => null;

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateProperty( string propertyName, object value ) => null;
    }
}