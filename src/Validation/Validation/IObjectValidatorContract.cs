namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IObjectValidator ) )]
    internal abstract class IObjectValidatorContract : IObjectValidator
    {
        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( propertyNames != null, "propertyNames" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }
        IReadOnlyList<IValidationResult> IObjectValidator.ValidateProperty( string propertyName, object value )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );
            return null;
        }
    }
}
