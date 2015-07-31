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
            Dispose( false );
        }

        internal NonInteractiveScope( TemplateWizardContext context, Action<TemplateWizardContext, bool> transition )
        {
            Contract.Requires( context != null );
            Contract.Requires( transition != null );

            this.context = context;
            restore = context.IsInteractive;
            this.transition = transition;
            this.transition( this.context, false );
        }

        private void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( !disposing )
                return;

            if ( restore && context != null && transition != null )
                transition( context, true );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
