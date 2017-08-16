namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IPropertyValidator<> ) )]
    abstract class IPropertyValidatorContract<T> : IPropertyValidator<T>
    {
        IReadOnlyList<IValidationResult> IPropertyValidator<T>.ValidateObject( T instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        string IPropertyValidator.PropertyName => null;

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance ) => null;

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value ) => null;
    }
}