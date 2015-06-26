namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( Activity ) )]
    internal abstract class ActivityContract : Activity
    {
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null, "serviceProvider" );
        }
    }
}
