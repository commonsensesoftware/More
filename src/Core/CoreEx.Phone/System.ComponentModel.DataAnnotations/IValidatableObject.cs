namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;

    /// <summary>
    /// Provides a way for an object to be invalidated.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.IValidatableObject.</remarks>
    public interface IValidatableObject
    {
        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>A collection that holds failed-validation information.</returns>
        /// <param name="validationContext">The validation context.</param>
        IEnumerable<ValidationResult> Validate( ValidationContext validationContext );
    }
}
