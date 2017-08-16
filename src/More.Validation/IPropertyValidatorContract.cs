namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IPropertyValidator ) )]
    abstract class IPropertyValidatorContract : IPropertyValidator
    {
        string IPropertyValidator.PropertyName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return null;
            }
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value )
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }
    }
}