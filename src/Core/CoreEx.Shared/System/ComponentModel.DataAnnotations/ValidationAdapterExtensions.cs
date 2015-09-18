namespace System.ComponentModel.DataAnnotations
{
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides adapter extension methods for data annotations.
    /// </summary>
    public static class ValidationAdapterExtensions
    {
        /// <summary>
        /// Creates and returns an adapter for the specified validation context.
        /// </summary>
        /// <param name="context">The <see cref="ValidationContext">validation context</see> to create an adapter for.</param>
        /// <returns>A new <see cref="IValidationContext">validation context</see> that has been adapted from the original <paramref name="context"/>.</returns>
        public static IValidationContext Adapt( this ValidationContext context )
        {
            Arg.NotNull( context, nameof( context ) );
            Contract.Ensures( Contract.Result<IValidationContext>() != null );
            return new ValidationContextAdapter( context );
        }

        /// <summary>
        /// Creates and returns an adapter for the specified validation result.
        /// </summary>
        /// <param name="result">The <see cref="ValidationResult">validation result</see> to create an adapter for.</param>
        /// <returns>A new <see cref="IValidationResult">validation result</see> that has been adapted from the original <paramref name="result"/>.</returns>
        public static IValidationResult Adapt( this ValidationResult result )
        {
            Arg.NotNull( result, nameof( result ) );
            Contract.Ensures( Contract.Result<IValidationResult>() != null );
            return new ValidationResultAdapter( result );
        }
    }
}
