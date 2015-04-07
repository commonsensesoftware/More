namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using More.Windows.Printing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using global::Windows.Foundation;
    using global::Windows.Graphics.Printing;
    using global::Windows.Graphics.Printing.OptionDetails;
    using global::Windows.UI.ViewManagement;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;

    /// <summary>
    /// Represents a behavior which mediates the contract with the Print charm.
    /// </summary>
    [CLSCompliant( false )]
    public class PrintContractBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the print title dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register( "Title", typeof( string ), typeof( PrintContractBehavior ), new PropertyMetadata( null ) );

        /// <summary>
        /// Gets or sets the print request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty PrintRequestProperty =
            DependencyProperty.Register( "PrintRequest", typeof( object ), typeof( PrintContractBehavior ), new PropertyMetadata( null, OnPrintRequestChanged ) );

        /// <summary>
        /// Gets or sets the print command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty PrintCommandProperty =
            DependencyProperty.Register( "PrintCommand", typeof( ICommand ), typeof( PrintContractBehavior ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the print options command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SetPrintOptionsCommandProperty =
            DependencyProperty.Register( "SetPrintOptionsCommand", typeof( ICommand ), typeof( PrintContractBehavior ), new PropertyMetadata( (object) null ) );

        private Interaction lastInteraction;
        private PrintManager printManager;

        private PrintManager PrintManager
        {
            get
            {
                return this.printManager;
            }
            set
            {
                if ( this.printManager == value )
                    return;

                if ( this.printManager != null )
                    this.printManager.PrintTaskRequested -= this.OnPrintTaskRequested;

                this.printManager = value;

                if ( this.printManager != null )
                    this.printManager.PrintTaskRequested += this.OnPrintTaskRequested;
            }
        }

        /// <summary>
        /// Gets or sets the title of the print task.
        /// </summary>
        /// <value>The print task title.</value>
        public string Title
        {
            get
            {
                return (string) this.GetValue( TitleProperty );
            }
            set
            {
                this.SetValue( TitleProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Print flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Print flyout.</value>
        public IInteractionRequest PrintRequest
        {
            get
            {
                return (IInteractionRequest) this.GetValue( PrintRequestProperty );
            }
            set
            {
                this.SetValue( PrintRequestProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the print command.
        /// </summary>
        /// <value>The print <see cref="ICommand">command</see>.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="IPrintArea"/>.</remarks>
        public ICommand PrintCommand
        {
            get
            {
                return (ICommand) this.GetValue( PrintCommandProperty );
            }
            set
            {
                this.SetValue( PrintCommandProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the command to set print options.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> to set print options.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="IPrintOptionDetailsFactory"/>.</remarks>
        public ICommand SetPrintOptionsCommand
        {
            get
            {
                return (ICommand) this.GetValue( SetPrintOptionsCommandProperty );
            }
            set
            {
                this.SetValue( SetPrintOptionsCommandProperty, value );
            }
        }

        /// <summary>
        /// Creates a new panel that represents the root printing element.
        /// </summary>
        /// <returns>A <see cref="Panel"/> that represents the root printing element. The default implementation
        /// returns a <see cref="Canvas"/>.</returns>
        protected virtual Panel CreatePrintingRoot()
        {
            Contract.Ensures( Contract.Result<Panel>() != null );

            return new Canvas()
            {
                Opacity = 0d
            };
        }

        private void AddPrintingArea( Panel printingRoot )
        {
            Contract.Requires( printingRoot != null );

            var queue = new Queue<DependencyObject>();
            queue.Enqueue( this.AssociatedObject );
            Panel panel = null;

            // find the first container relative to the associated object,
            // which may be the associated object itself
            while ( queue.Count > 0 )
            {
                var current = queue.Dequeue();
                panel = current as Panel;

                if ( panel != null )
                    break;

                var count = VisualTreeHelper.GetChildrenCount( current );

                for ( var i = 0; i < count; i++ )
                    queue.Enqueue( VisualTreeHelper.GetChild( current, i ) );
            }

            // if we can't find a container, then we are unable to add the print surface
            if ( panel == null )
                return;

            // add print surface as the first child
            panel.Children.Insert( 0, printingRoot );
        }

        private void RemovePrintingArea( Panel printingRoot )
        {
            if ( printingRoot == null )
                return;

            var parent = printingRoot.Parent as Panel;

            if ( parent != null )
                parent.Children.Remove( printingRoot );
        }

        private static void OnPrintRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (PrintContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
                oldRequest.Requested -= @this.OnPrintRequested;

            if ( newRequest != null )
                newRequest.Requested += @this.OnPrintRequested;
        }

        private async void OnPrintRequested( object sender, InteractionRequestedEventArgs e )
        {
            // capture the interaction request and show the print charm
            this.lastInteraction = e.Interaction;
            await PrintManager.ShowPrintUIAsync();
        }

        private void OnPrintTaskRequested( PrintManager sender, PrintTaskRequestedEventArgs e )
        {
            string title = null;

            // setup based on the request interaction
            if ( this.lastInteraction != null )
            {
                title = this.lastInteraction.Title;
                this.lastInteraction = null;
            }

            // fallback to behavior settings
            if ( string.IsNullOrEmpty( title ) )
                title = this.Title ?? string.Empty;

            // create print task
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask( title, request => this.OnPrintTaskCreated( printTask, request ) );
        }

        private void OnPrintTaskCreated( PrintTask printTask, PrintTaskSourceRequestedArgs request )
        {
            Contract.Requires( printTask != null );
            Contract.Requires( request != null );

            var command = this.SetPrintOptionsCommand;

            if ( command != null )
            {
                // invoke command to setup print options
                var factory = new PrintTaskOptionDetailsWrapper( PrintTaskOptionDetails.GetFromPrintTaskOptions( printTask.Options ) );

                if ( command.CanExecute( factory ) )
                    command.Execute( factory );
            }

            // create the print area
            var printingRoot = this.CreatePrintingRoot();
            var printArea = new PrintAreaWrapper( printingRoot );
            command = this.PrintCommand;

            // add the print are element to the associated object
            this.AddPrintingArea( printingRoot );

            // execute the command to initiate printing
            if ( command.CanExecute( printArea ) )
                command.Execute( printArea );

            // remove the print area after printing is complete
            var task = printTask;
            TypedEventHandler<PrintTask, PrintTaskCompletedEventArgs> handler = null;
            handler = ( s, e ) =>
            {
                task.Completed -= handler;
                this.RemovePrintingArea( printingRoot );
            };
            printTask.Completed += handler;

            // set the document source to the print document used in the print area
            request.SetSource( printArea.PrintDocument.DocumentSource );
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.PrintManager = PrintManager.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            this.PrintManager = null;
            base.OnDetaching();
        }
    }
}
