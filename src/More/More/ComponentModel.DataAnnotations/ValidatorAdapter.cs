namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Represents an adapter for the portable <see cref="IValidator"/> interface to the <see cref="Validator"/> class.
    /// </summary>
    public class ValidatorAdapter : IValidator
    {
        readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorAdapter"/> class.
        /// </summary>
        /// <remarks>This constructor uses the <see cref="ServiceProvider.Current">current service provider</see> for validation.</remarks>
        public ValidatorAdapter() => serviceProvider = ServiceProvider.Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorAdapter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> used for validation.</param>
        public ValidatorAdapter( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates and returns a new validation context.
        /// </summary>
        /// <param name="instance">The object to create the context for.</param>
        /// <param name="items">The dictionary of key/value pairs that is associated with this context.</param>
        /// <returns>A new validation context.</returns>
        public virtual IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            Arg.NotNull( instance, nameof( instance ) );

            var context = new ValidationContext( instance, items );
            context.InitializeServiceProvider( serviceProvider.GetService );
            return new ValidationContextAdapter( context );
        }

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
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( !validationContext.TryGetService( out ValidationContext context ) )
            {
                return true;
            }

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject( instance, context, results );

            if ( validationResults != null )
            {
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );
            }

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
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( !validationContext.TryGetService( out ValidationContext context ) )
            {
                return true;
            }

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject( instance, context, results, validateAllProperties );

            if ( validationResults != null )
            {
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );
            }

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
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( !validationContext.TryGetService( out ValidationContext context ) )
            {
                return true;
            }

            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateProperty( value, context, results );

            if ( validationResults != null )
            {
                validationResults.AddRange( results.Select( r => new ValidationResultAdapter( r ) ) );
            }

            return valid;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        public virtual void ValidateObject( object instance, IValidationContext validationContext )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( validationContext.TryGetService( out ValidationContext context ) )
            {
                Validator.ValidateObject( instance, context );
            }
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
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( validationContext.TryGetService( out ValidationContext context ) )
            {
                Validator.ValidateObject( instance, context, validateAllProperties );
            }
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        public virtual void ValidateProperty( object value, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( validationContext.TryGetService( out ValidationContext context ) )
            {
                Validator.ValidateProperty( value, context );
            }
        }
    }
}