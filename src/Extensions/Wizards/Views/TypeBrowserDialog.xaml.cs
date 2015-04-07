namespace More.VisualStudio.Views
{
    using More.Composition;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    /// <summary>
    /// Represents a type browser dialog.
    /// </summary>
    public partial class TypeBrowserDialog : Window, IView<TypeBrowserViewModel>, IDisposable
    {
        private bool disposed;

        ~TypeBrowserDialog()
        {
            this.Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBrowserDialog"/> class.
        /// </summary>
        public TypeBrowserDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBrowserDialog"/> class.
        /// </summary>
        /// <param name="model">The associated <see cref="TypeBrowserViewModel">model</see>.</param>
        public TypeBrowserDialog( TypeBrowserViewModel model )
            : this()
        {
            this.DataContext = model;
        }

        /// <summary>
        /// Gets the model associated with the view.
        /// </summary>
        /// <value>The associated <see cref="TypeBrowserViewModel">model</see>.</value>
        public TypeBrowserViewModel Model
        {
            get
            {
                return this.DataContext as TypeBrowserViewModel;
            }
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="TypeBrowserDialog"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        [SuppressMessage( "Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required to resolve a known WPF bug." )]
        protected virtual void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            var dispatcher = this.Dispatcher;

            if ( dispatcher == null )
                return;

            // HACK: addresses unfixed issue reported:
            // https://connect.microsoft.com/VisualStudio/feedback/details/882742/unloading-an-appdomain-that-uses-a-net-dll-that-displays-a-wpf-dialog-causes-cannotunloadappdomainexception-exception-only-on-touchscreen-computers
            dispatcher.InvokeShutdown();
            GC.Collect();
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="TypeBrowserDialog"/> class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
