namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Represents an adapter for the portable <see cref="IValidator"/> interface to the <see cref="Validator"/> class.
    /// </summary>
    public partial class ValidatorAdapter : IValidator
    {
        /// <summary>
        /// Determines whether the specified object is valid using the validation context
        /// and validation results collection.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the object validates; otherwise, false.</returns>
        public virtual bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( !validationContext.TryGetService( out context ) )
                return true;

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject( instance, context, results );

            if ( validationResults != null )
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );

            return valid;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context,
        /// validation results collection, and a value that specifies whether to validate
        /// all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <param name="validateAllProperties">True to validate all properties; if false, only required attributes are validated.</param>
        /// <returns>True if the object validates; otherwise, false.</returns>
        public virtual bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( !validationContext.TryGetService( out context ) )
                return true;

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject( instance, context, results, validateAllProperties );

            if ( validationResults != null )
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );

            return valid;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the property validates; otherwise, false.</returns>
        public virtual bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( !validationContext.TryGetService( out context ) )
                return true;

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty( value, context, results );

            if ( validationResults != null )
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );

            return valid;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        public virtual void ValidateObject( object instance, IValidationContext validationContext )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( validationContext.TryGetService( out context ) )
                Validator.ValidateObject( instance, context );
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context,
        /// and a value that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validateAllProperties">True to validate all properties; otherwise, false.</param>
        public virtual void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( validationContext.TryGetService( out context ) )
                Validator.ValidateObject( instance, context, validateAllProperties );
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        public virtual void ValidateProperty( object value, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, "validationContext" );

            ValidationContext context;

            if ( validationContext.TryGetService( out context ) )
                Validator.ValidateProperty( value, context );
        }
    }
}
