using System.Diagnostics.Contracts;
namespace More.ComponentModel
{
    using System;
    using System.Composition;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    [ContractClassFor( typeof( ActivityManagerBase ) )]
    internal abstract class ActivityManagerBaseContract : ActivityManagerBase
    {
        protected ActivityManagerBaseContract()
            : base( Enumerable.Empty<ExportFactory<IActivity, ActivityDescriptor>>() )
        {
        }

        public override Task<Uri> RegisterAsync( IActivity activity )
        {
            Contract.Requires<ArgumentNullException>( activity != null, "activity" );
            return null;
        }
    }
}
