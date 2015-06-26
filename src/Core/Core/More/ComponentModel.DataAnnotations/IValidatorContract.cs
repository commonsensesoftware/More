namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IValidator ) )]
    internal abstract class IValidatorContract : IValidator
    {
        public IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Ensures( Contract.Result<IValidationContext>() != null );
            return null;
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
            return default( bool );
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
            return default( bool );
        }

        public bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
            return default( bool );
        }

        public void ValidateObject( object instance, IValidationContext validationContext )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
        }

        public void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
            Contract.Requires<ArgumentNullException>( instance != null, "instance" );
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
        }

        public void ValidateProperty( object value, IValidationContext validationContext )
        {
            Contract.Requires<ArgumentNullException>( validationContext != null, "validationContext" );
        }
    }
}
