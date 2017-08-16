namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    sealed class GenericValidator : IValidator
    {
        public IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Contract.Ensures( Contract.Result<IValidationContext>() != null );
            return new GenericValidationContext( instance, items );
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );
            return true;
        }

        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );
            return true;
        }

        public bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );
            return true;
        }

        public void ValidateObject( object instance, IValidationContext validationContext )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );
        }

        public void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );
        }

        public void ValidateProperty( object value, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );
        }
    }
}