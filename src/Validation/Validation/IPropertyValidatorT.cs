namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a typed property validator.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to validate.</typeparam>
    [ContractClass( typeof( IPropertyValidatorContract<> ) )]
    public interface IPropertyValidator<in T> : IPropertyValidator
    {
        /// <summary>
        /// Validates the property against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate the current validator's property against.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>
        /// which describe any validation errors.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( T instance );
    }
}
