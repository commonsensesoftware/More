namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an activity configuration.
    /// </summary>
    [ContractClass( typeof( IActivityConfigurationContract ) )]
    public interface IActivityConfiguration
    {
        /// <summary>
        /// Gets the sequence of dependent activity types the configured activity depends on.
        /// </summary>
        /// <value>The <see cref="IActivity">activity</see> <see cref="Type">types</see> the
        /// configured <see cref="IActivity">activity</see> depends on.</value>
        IEnumerable<Type> Dependencies
        {
            get;
        }

        /// <summary>
        /// Configures the specified activity.
        /// </summary>
        /// <param name="activity">The <see cref="IActivity"/> to configure.</param>
        void Configure( IActivity activity );

        /// <summary>
        /// Configures the activity with a dependency to another activity.
        /// </summary>
        /// <param name="activityType">The <see cref="IActivity">activity</see> <see cref="Type">type</see> the
        /// configured <see cref="IActivity">activity</see> depends on.</param>
        void DependsOn( Type activityType );
    }
}
