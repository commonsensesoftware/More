namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a user-interface (UI) activity manager.
    /// </summary>
    [ContractClass( typeof( IActivityManagerContract ) )]
    public interface IActivityManager
    {
        /// <summary>
        /// Gets a list of activity descriptors.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IActivityDescriptor">activity descriptors</see>.</value>
        IReadOnlyList<IActivityDescriptor> ActivityDescriptors { get; }

        /// <summary>
        /// Gets the activity with the specified identifier.
        /// </summary>
        /// <param name="activityId">The <see cref="Guid"/> representing the identifier of the activity to return.</param>
        /// <returns>An <see cref="IActivity"/> object.</returns>
        IActivity GetActivity( Guid activityId );

        /// <summary>
        /// Gets the current activity.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the current <see cref="IActivity">activity</see>
        /// or null if there isn't a current activity.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method is asynchronous and not suitable as a property." )]
        Task<IActivity> GetCurrentAsync();

        /// <summary>
        /// Registers the specified activity instance with the activity manager.
        /// </summary>
        /// <param name="activity">The <see cref="IActivity"/> to register.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a <see cref="Uri">URL</see> referencing the <see cref="IActivity">activity</see>.</returns>
        /// <remarks>The <see cref="Uri"/> returned represents the external Uniform Resource Locator (URL) that can be used to execute the activity.</remarks>
        Task<Uri> RegisterAsync( IActivity activity );

        /// <summary>
        /// Unregisters the specified activity instance from the activity manager.
        /// </summary>
        /// <param name="activity">The <see cref="IActivity"/> to unregister.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a value that indicates whether the operation succeeded.</returns>
        Task<bool> UnregisterAsync( IActivity activity );

        /// <summary>
        /// Unregisters the specified activity from the activity manager.
        /// </summary>
        /// <param name="activityId">A <see cref="Guid"/> representing the activity identifier.</param>
        /// <param name="activityInstanceId">A <see cref="Nullable{T}">nullable</see> <see cref="Guid"/> representing the activity instance identifier.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a value that indicates whether the operation succeeded.</returns>
        Task<bool> UnregisterAsync( Guid activityId, Guid? activityInstanceId );
    }
}