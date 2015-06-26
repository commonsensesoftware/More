namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides metadata to indicate that an activity depends on one or more other activities.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false )]
    public sealed class DependsOnActivityAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnActivityAttribute"/> class.
        /// </summary>
        /// <param name="activityType">The dependent activity <see cref="Type">type</see>.</param>
        public DependsOnActivityAttribute( Type activityType )
        {
            Arg.NotNull( activityType, "activityType" );
            this.ActivityType = activityType;
        }

        /// <summary>
        /// Gets the type of the dependent activity.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type ActivityType
        {
            get;
            private set;
        }
    }
}
