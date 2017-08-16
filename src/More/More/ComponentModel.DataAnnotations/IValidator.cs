namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object that can be used to validate objects, properties, and methods.
    /// </summary>
    [ContractClass( typeof( IValidatorContract ) )]
    public interface IValidator
    {
        /// <summary>
        /// Creates and returns a new validation context.
        /// </summary>
        /// <param name="instance">The object to create the context for.</param>
        /// <param name="items">The dictionary of key/value pairs that is associated with this context.</param>
        /// <returns>A new validation context.</returns>
        IValidationContext CreateContext( object instance, IDictionary<object, object> items );

        /// <summary>
        /// Determines whether the specified object is valid using the validation context
        /// and validation results collection.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the object validates; otherwise, false.</returns>
        bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults );

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
        bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties );

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the property validates; otherwise, false.</returns>
        bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults );

        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        void ValidateObject( object instance, IValidationContext validationContext );

        /// <summary>
        /// Determines whether the specified object is valid using the validation context,
        /// and a value that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validateAllProperties">True to validate all properties; otherwise, false.</param>
        void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties );

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        void ValidateProperty( object value, IValidationContext validationContext );
    }
}