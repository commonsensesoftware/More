namespace System.Composition.Convention
{
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using More.Composition;
    using System;

    /// <summary>
    /// Provides extension methods for the <see cref="ConventionBuilder"/> class.
    /// </summary>
    [CLSCompliant( false )]
    public static class ConventionBuilderExtensions
    {
        /// <summary>
        /// Defines a rule that applies to <see cref="Type">types</see> within the specified namespace.
        /// </summary>
        /// <param name="builder">The extended <see cref="ConventionBuilder">convention builder</see>.</param>
        /// <param name="namespace">The namespace to match. The value can be a fully-qualified namespace or a nested namespace.</param>
        /// <returns>A <see cref="PartConventionBuilder"/> that is used to specify the rule.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static PartConventionBuilder ForTypesInNamespace( this ConventionBuilder builder, string @namespace )
        {
            Arg.NotNull( builder, nameof( builder ) );
            Arg.NotNullOrEmpty( @namespace, nameof( @namespace ) );
            Contract.Ensures( Contract.Result<PartConventionBuilder>() != null );

            var specification = new NamespaceSpecification( @namespace );
            return builder.ForTypesMatching( specification.IsSatisfiedBy );
        }

        /// <summary>
        /// Defines a rule that applies to <see cref="Type">types</see> within the specified namespace.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> to which the rule applies.</typeparam>
        /// <param name="builder">The extended <see cref="ConventionBuilder">convention builder</see>.</param>
        /// <param name="namespace">The namespace to match. The value can be a fully-qualified namespace or a nested namespace.</param>
        /// <returns>A <see cref="PartConventionBuilder{T}"/> that is used to specify the rule.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static PartConventionBuilder<T> ForTypesInNamespace<T>( this ConventionBuilder builder, string @namespace )
        {
            Arg.NotNull( builder, nameof( builder ) );
            Arg.NotNullOrEmpty( @namespace, nameof( @namespace ) );
            Contract.Ensures( Contract.Result<PartConventionBuilder>() != null );

            var specification = new NamespaceSpecification( @namespace );
            return builder.ForTypesMatching<T>( specification.IsSatisfiedBy );
        }
    }
}
