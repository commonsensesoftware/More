namespace More.Windows
{
    using System;
    using System.Diagnostics.Contracts;

    internal sealed class DeferralAdapter : IDisposable
    {
        private bool disposed;
        private Action complete;

        internal DeferralAdapter( Action complete )
        {
            Contract.Requires( complete != null );
            this.complete = complete;
        }

        public void Dispose()
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( this.complete != null )
            {
                this.complete();
                this.complete = null;
            }

            GC.SuppressFinalize( this );
        }
    }
}
