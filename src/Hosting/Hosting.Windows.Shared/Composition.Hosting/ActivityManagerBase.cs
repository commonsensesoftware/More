namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a base implementation for an activity manager.
    /// </summary>
    public abstract class ActivityManagerBase : IActivityManager
    {
        private readonly IDictionary<Guid, ExportFactory<IActivity, ActivityDescriptor>> activities;
        private readonly IReadOnlyList<IActivityDescriptor> descriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityManagerBase"/> class.
        /// </summary>
        /// <param name="activities">The <see cref="IEnumerable{T}">sequence</see> of <see cref="ExportFactory{T,TMetadata}">exported</see>
        /// <see cref="IActivity">activities</see> to initialize the manager with.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        protected ActivityManagerBase( IEnumerable<ExportFactory<IActivity, ActivityDescriptor>> activities )
        {
            Contract.Requires<ArgumentNullException>( activities != null, "activities" );

            this.activities = activities.ToDictionary( a => new Guid( a.Metadata.Id ) );
            this.descriptors = activities.Select( a => a.Metadata ).ToArray();
        }

        /// <summary>
        /// Gets a list of activity descriptors.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IActivityDescriptor">activity descriptors</see>.</value>
        public IReadOnlyList<IActivityDescriptor> ActivityDescriptors
        {
            get
            {
                return this.descriptors;
            }
        }

        /// <summary>
        /// Gets the activity with the specified identifier.
        /// </summary>
        /// <param name="activityId">The <see cref="Guid"/> representing the identifier of the activity to return.</param>
        /// <returns>An <see cref="IActivity"/> object.</returns>
        public IActivity GetActivity( Guid activityId )
        {
            return this.activities[activityId].CreateExport().Value;
        }

        /// <summary>
        /// Gets the current activity.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the current <see cref="IActivity">activity</see>
        /// or null if there isn't a current activity.</returns>
        public abstract Task<IActivity> GetCurrentAsync();

        /// <summary>
        /// Registers the specified activity instance with the activity manager.
        /// </summary>
        /// <param name="activity">The <see cref="IActivity"/> to register.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a <see cref="Uri">URL</see> referencing the <see cref="IActivity">activity</see>.</returns>
        /// <remarks>The <see cref="Uri"/> returned represents the external Uniform Resource Locator (URL) that can be used to execute the activity.</remarks>
        public abstract Task<Uri> RegisterAsync( IActivity activity );

        /// <summary>
        /// Unregisters the specified activity instance from the activity manager.
        /// </summary>
        /// <param name="activity">The <see cref="IActivity"/> to unregister.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a value that indicates whether the operation succeeded.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public virtual Task<bool> UnregisterAsync( IActivity activity )
        {
            return this.UnregisterAsync( activity.Id, activity.InstanceId );
        }

        /// <summary>
        /// Unregisters the specified activity from the activity manager.
        /// </summary>
        /// <param name="activityId">A <see cref="Guid"/> representing the activity identifier.</param>
        /// <param name="activityInstanceId">A <see cref="Nullable{T}">nullable</see> <see cref="Guid"/> representing the activity instance identifier.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a value that indicates whether the operation succeeded.</returns>
        public abstract Task<bool> UnregisterAsync( Guid activityId, Guid? activityInstanceId );
    }
}
