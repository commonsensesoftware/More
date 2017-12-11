namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="IRule{TInput,TOutput}"/> interface.
    /// </summary>
    public static class IRuleExtensions
    {
        /// <summary>
        /// Returns a specialized version of the rule for the specified instance.
        /// </summary>
        /// <typeparam name="TObject">The <see cref="Type">type</see> of instance.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="rule">The <see cref="IRule{TInput,TOutput}">rule</see> to get a specialized version of.</param>
        /// <param name="instance">The instance to get the rule for.</param>
        /// <returns>A specialized <see cref="IRule{TInput,TOutput}">rule</see> for the provided <paramref name="instance"/>.</returns>
        /// <remarks>If the specified <paramref name="rule"/> does not implement the <see cref="IPerInstanceRule{TObject,TValue}"/>
        /// interface, then the rule does not support per-instance specializations and the original rule is returned.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        public static IRule<Property<TValue>, IValidationResult> GetPerInstance<TObject, TValue>( this IRule<Property<TValue>, IValidationResult> rule, TObject instance )
        {
            Arg.NotNull( rule, nameof( rule ) );
            Arg.NotNull( instance, nameof( instance ) );
            Contract.Ensures( Contract.Result<IRule<Property<TValue>, IValidationResult>>() != null );

            var instanceRule = rule as IPerInstanceRule<TObject, TValue>;
            return instanceRule == null ? rule : instanceRule.GetPerInstance( instance );
        }
    }
}