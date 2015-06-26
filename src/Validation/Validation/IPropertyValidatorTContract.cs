namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IPropertyValidator<> ) )]
    internal abstract class IPropertyValidatorContract<T> : IPropertyValidator<T>
    {
        IReadOnlyList<IValidationResult> IPropertyValidator<T>.ValidateObject( T instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        string IPropertyValidator.PropertyName
        {
            get
            {
                return null;
            }
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance )
        {
            return null;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value )
        {
            return null;
        }
    }
}
