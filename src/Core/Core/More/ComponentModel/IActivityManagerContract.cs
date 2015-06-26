namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the code contract definition for the <see cref="IActivityManager"/> interface.
    /// </summary>
    [ContractClassFor( typeof( IActivityManager ) )]
    internal abstract class IActivityManagerContract : IActivityManager
    {
        IActivity IActivityManager.GetActivity(Guid activityId)
        {
                Contract.Ensures( Contract.Result<IActivity>() != null );
                return default( IActivity );
        }

        IReadOnlyList<IActivityDescriptor> IActivityManager.ActivityDescriptors
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<IActivityDescriptor>>() != null );
                return default( IReadOnlyList<IActivityDescriptor> );
            }
        }

        Task<IActivity> IActivityManager.GetCurrentAsync()
        {
            Contract.Ensures( Contract.Result<Task<IActivity>>() != null );
            return null;
        }

        Task<Uri> IActivityManager.RegisterAsync( IActivity task )
        {
            Contract.Requires<ArgumentNullException>( task != null, "task" );
            Contract.Ensures( Contract.Result<Task<Uri>>() != null );
            return null;
        }

        Task<bool> IActivityManager.UnregisterAsync( IActivity task )
        {
            Contract.Requires<ArgumentNullException>( task != null, "task" );
            Contract.Ensures( Contract.Result<Task<bool>>() != null );
            return null;
        }

        Task<bool> IActivityManager.UnregisterAsync( Guid taskId, Guid? taskInstanceId )
        {
            Contract.Ensures( Contract.Result<Task<bool>>() != null );
            return null;
        }
    }
}
