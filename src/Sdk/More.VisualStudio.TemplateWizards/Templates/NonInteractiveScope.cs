namespace More.VisualStudio.Templates
{
    using System;
    using System.Diagnostics.Contracts;

    sealed class NonInteractiveScope : IDisposable
    {
        readonly bool restore;
        readonly Action<TemplateWizardContext, bool> transition;
        readonly TemplateWizardContext context;
        bool disposed;

        ~NonInteractiveScope() => Dispose( false );

        internal NonInteractiveScope( TemplateWizardContext context, Action<TemplateWizardContext, bool> transition )
        {
            Contract.Requires( context != null );
            Contract.Requires( transition != null );

            this.context = context;
            restore = context.IsInteractive;
            this.transition = transition;
            this.transition( this.context, false );
        }

        void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            if ( restore && context != null && transition != null )
            {
                transition( context, true );
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}