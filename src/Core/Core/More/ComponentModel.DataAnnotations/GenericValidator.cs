namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;

    internal sealed class GenericValidator : IValidator
    {
        public IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            return new GenericValidationContext( instance, items );
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            return true;
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            return true;
        }

        public bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            return true;
        }

        public void ValidateObject( object instance, IValidationContext validationContext )
        {
        }

        public void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
        }

        public void ValidateProperty( object value, IValidationContext validationContext )
        {
        }
    }
}
