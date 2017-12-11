namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of a validation builder.
    /// </summary>
    /// <typeparam name="TObject">The <see cref="Type">type</see> of object.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
    [ContractClass( typeof( IValidationBuilderContract<,> ) )]
    public interface IValidationBuilder<TObject, TValue> : IPropertyValidator<TObject>
    {
        /// <summary>
        /// Applies a validation rule to the current validator.
        /// </summary>
        /// <param name="rule">The validation <see cref="IRule{T,TResult}">rule</see> to add.</param>
        /// <returns>The current <see cref="PropertyValidator{TObject,TValue}">validator</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics" )]
        IValidationBuilder<TObject, TValue> Apply( IRule<Property<TValue>, IValidationResult> rule );
    }
}