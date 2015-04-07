namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IActivityConfiguration ) )]
    internal abstract class IActivityConfigurationContract : IActivityConfiguration
    {
        IEnumerable<Type> IActivityConfiguration.Dependencies
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<Type>>() != null );
                return null;
            }
        }

        void IActivityConfiguration.DependsOn( Type activityType )
        {
            Contract.Requires<ArgumentNullException>( activityType != null, "activityType" );
        }

        void IActivityConfiguration.Configure( IActivity activity )
        {
            Contract.Requires<ArgumentNullException>( activity != null, "activity" );
        }
    }
}
