namespace More.Windows
{
    using System;
    using System.Diagnostics.Contracts;

    sealed class DeferralAdapter : IDisposable
    {
        bool disposed;
        Action complete;

        internal DeferralAdapter( Action complete ) => this.complete = complete;

        public void Dispose()
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;
            complete?.Invoke();
            complete = null;
            GC.SuppressFinalize( this );
        }
    }
}