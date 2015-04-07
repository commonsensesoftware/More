namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Provides extension methods for the <see cref="IActivity"/> interface.
    /// </summary>
    [SuppressMessage( "Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IUI", Justification = "'UI' is correct, but interfaces should also be prefixed with 'I'." )]
    public static class IActivityExtensions
    {
        /// <summary>
        /// Gets a sequence of dependent tasks for the specified activity.
        /// </summary>
        /// <param name="activity">The extended <see cref="IActivity">activity</see> object.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of dependent activity <see cref="Type">types</see>.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static IEnumerable<Type> DependsOn( this IActivity activity )
        {
            Contract.Requires<ArgumentNullException>( activity != null, "activity" );

            var type = activity.GetType().GetTypeInfo();
            return type.GetCustomAttributes<DependsOnActivityAttribute>( false ).Select( a => a.ActivityType ).ToArray();
        }
    }
}
