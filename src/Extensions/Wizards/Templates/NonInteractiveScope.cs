namespace More.VisualStudio.Templates
{
    using System;
    using System.Diagnostics.Contracts;

    internal sealed class NonInteractiveScope : IDisposable
    {
        private readonly bool restore;
        private readonly Action<TemplateWizardContext, bool> transition;
        private readonly TemplateWizardContext context;
        private bool disposed;

        ~NonInteractiveScope()
        {
            this.Dispose( false );
        }

        internal NonInteractiveScope( TemplateWizardContext context, Action<TemplateWizardContext, bool> transition )
        {
            Contract.Requires( context != null );
            Contract.Requires( transition != null );

            this.context = context;
            this.restore = context.IsInteractive;
            this.transition = transition;
            this.transition( this.context, false );
        }

        private void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            if ( this.restore && this.context != null && this.transition != null )
                this.transition( this.context, true );
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
