namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object validator.
    /// </summary>
    [ContractClass( typeof( IObjectValidatorContract ) )]
    public interface IObjectValidator
    {
        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( object instance );

        /// <summary>
        /// Validates the requested properties against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyNames">A <see cref="IEnumerable{T}">sequence</see> of the names of the properties to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( object instance, IEnumerable<string> propertyNames );

        /// <summary>
        /// Validates the specified value against the property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        IReadOnlyList<IValidationResult> ValidateProperty( string propertyName, object value );
    }
}