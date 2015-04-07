namespace More.ComponentModel
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>( activityType != null, "activityType" );
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
