namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IObjectValidator ) )]
    abstract class IObjectValidatorContract : IObjectValidator
    {
        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            Contract.Requires<ArgumentNullException>( instance != null, nameof( instance ) );
            Contract.Requires<ArgumentNullException>( propertyNames != null, nameof( propertyNames ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateProperty( string propertyName, object value )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), nameof( propertyName ) );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }
    }
}