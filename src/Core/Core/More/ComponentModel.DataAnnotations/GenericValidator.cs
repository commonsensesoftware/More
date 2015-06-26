namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal sealed class GenericValidator : IValidator
    {
        public IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            Arg.NotNull( instance, "instance" );
            Contract.Ensures( Contract.Result<IValidationContext>() != null );
            return new GenericValidationContext( instance, items );
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );
            return true;
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );
            return true;
        }

        public bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( validationContext, "validationContext" );
            return true;
        }

        public void ValidateObject( object instance, IValidationContext validationContext )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );
        }

        public void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );
        }

        public void ValidateProperty( object value, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, "validationContext" );
        }
    }
}
