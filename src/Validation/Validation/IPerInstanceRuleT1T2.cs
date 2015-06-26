namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the behavior of a rule that can create specialized variants for a given object instance.
    /// </summary>
    /// <typeparam name="TObject">The <see cref="Type">type</see> of instance.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
    public interface IPerInstanceRule<TObject, TValue> : IRule<Property<TValue>, IValidationResult>
    {
        /// <summary>
        /// Returns a specialized version of the rule for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to get the rule for.</param>
        /// <returns>A specialized <see cref="IRule{TInput,TOutput}">rule</see> for the provided <paramref name="instance"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        IRule<Property<TValue>, IValidationResult> GetPerInstance( TObject instance );
    }
}
