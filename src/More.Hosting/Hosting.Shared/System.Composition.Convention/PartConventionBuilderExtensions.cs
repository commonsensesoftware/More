namespace System.Composition.Convention
{
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using System;
    using static More.Composition.Boundary;

    /// <summary>
    /// Provides extension methods for the <see cref="PartConventionBuilder"/> class.
    /// </summary>
    [CLSCompliant( false )]
    public static class PartConventionBuilderExtensions
    {
        /// <summary>
        /// Marks the part as being shared within the Per-Request specified boundary.
        /// </summary>
        /// <param name="partBuilder">The extended <see cref="PartConventionBuilder"/>.</param>
        /// <returns>A <see cref="PartConventionBuilder">part builder</see> allowing further configuration of the part.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static PartConventionBuilder SharedPerRequest( this PartConventionBuilder partBuilder )
        {
            Arg.NotNull( partBuilder, nameof( partBuilder ) );
            Contract.Ensures( Contract.Result<PartConventionBuilder>() != null );
            return partBuilder.Shared( PerRequest );
        }
    }
}
