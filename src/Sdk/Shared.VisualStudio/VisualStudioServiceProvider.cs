namespace More.VisualStudio
{
    using System;
    using static System.Guid;
    using static System.IntPtr;
    using static System.Runtime.InteropServices.Marshal;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    sealed class VisualStudioServiceProvider : ServiceContainer, IDisposable
    {
        static readonly Guid IUnknownInterfaceId = new Guid( "{00000000-0000-0000-C000-000000000046}" );
        IOleServiceProvider serviceProvider;

        internal VisualStudioServiceProvider( IOleServiceProvider serviceProvider ) => this.serviceProvider = serviceProvider;

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            serviceProvider = null;
        }

        public override object GetService( Type serviceType )
        {
            if ( serviceType == null )
            {
                throw new ArgumentNullException( nameof( serviceType ) );
            }

            return base.GetService( serviceType ) ?? GetService( serviceType.GUID );
        }

        object GetService( Guid guid )
        {
            if ( serviceProvider == null )
            {
                return null;
            }

            if ( guid.Equals( Empty ) )
            {
                return null;
            }
            else if ( guid.Equals( typeof( IOleServiceProvider ).GUID ) )
            {
                return serviceProvider;
            }

            var service = Zero;
            var iid = IUnknownInterfaceId;

            if ( ( serviceProvider.QueryService( ref guid, ref iid, out service ) < 0 ) || ( service == Zero ) )
            {
                return null;
            }

            try
            {
                return GetObjectForIUnknown( service );
            }
            finally
            {
                Release( service );
            }
        }
    }
}