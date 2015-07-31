namespace More.VisualStudio
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Represents a managed service provider for interacting with Visual Studio.
    /// </summary>
    internal sealed class VisualStudioServiceProvider : ServiceContainer, IDisposable
    {
        private static readonly Guid IUnknownInterfaceId = new Guid( "{00000000-0000-0000-C000-000000000046}" );
        private IOleServiceProvider serviceProvider;

        private object GetService( Guid guid )
        {
            if ( serviceProvider == null )
                return null;

            // short circuit if possible
            if ( guid.Equals( Guid.Empty ) )
                return null;
            else if ( guid.Equals( typeof( IOleServiceProvider ).GUID ) )
                return serviceProvider;

            var service = IntPtr.Zero;
            var iid = IUnknownInterfaceId;

            // query visual studio provider for service
            if ( ( serviceProvider.QueryService( ref guid, ref iid, out service ) < 0 ) || ( service == IntPtr.Zero ) )
                return null;

            try
            {
                // unwrap unmanaged IUnknown to System.Object
                return Marshal.GetObjectForIUnknown( service );
            }
            finally
            {
                // always release unmanaged resource
                Marshal.Release( service );
            }
        }

        internal VisualStudioServiceProvider( IOleServiceProvider serviceProvider )
        {
            Contract.Requires( serviceProvider != null );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="VisualStudioServiceProvider"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            serviceProvider = null;
        }

        /// <summary>
        /// Returns the service returned with the specified service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or <c>null</c> if no match is found.</returns>
        public override object GetService( Type serviceType )
        {
            if ( serviceType == null )
                throw new ArgumentNullException( "serviceType" );

            return base.GetService( serviceType ) ?? GetService( serviceType.GUID );
        }
    }
}
