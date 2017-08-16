namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a property validator.
    /// </summary>
    [ContractClass( typeof( IPropertyValidatorContract ) )]
    public interface IPropertyValidator
    {
        /// <summary>
        /// Gets the name of the property to validate.
        /// </summary>
        /// <value>The validated property name.</value>
        string PropertyName { get; }

        /// <summary>
        /// Validates the property against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate the current validator's property against.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>
        /// which describe any validation errors.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( object instance );

        /// <summary>
        /// Validates the property against the specified value.
        /// </summary>
        /// <param name="value">The value to validate the current validator's property against.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>
        /// which describe any validation errors.</returns>
        IReadOnlyList<IValidationResult> ValidateValue( object value );
    }
}