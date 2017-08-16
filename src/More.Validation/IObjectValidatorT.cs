namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines the behavior of a typed object validator.
    /// </summary>
    [ContractClass( typeof( IObjectValidatorContract<> ) )]
    public interface IObjectValidator<T> : IObjectValidator
    {
        /// <summary>
        /// Creates a validation builder for the specified property.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> property value to validate.</typeparam>
        /// <param name="propertyExpression">The <see cref="Expression{T}">expression</see> representing the property to validate.</param>
        /// <returns>A <see cref="IValidationBuilder{TObject,TValue}">property validation builder</see> for the specified <paramref name="propertyExpression"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generic expression" )]
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "This memeber is a method and is unambiguous." )]
        IValidationBuilder<T, TValue> Property<TValue>( Expression<Func<T, TValue>> propertyExpression );

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( T instance );

        /// <summary>
        /// Validates the requested properties against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyNames">A <see cref="IEnumerable{T}">sequence</see> of the names of the properties to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        IReadOnlyList<IValidationResult> ValidateObject( T instance, IEnumerable<string> propertyNames );
    }
}