namespace More.Windows.Interactivity
{
    using More.Windows.Data;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using global::Windows.ApplicationModel.DataTransfer;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents a behavior which mediates the contract with the Share contract.
    /// </summary>
    /// <example>The following example demonstrates how to share data from a page.
    /// <code lang="Xaml"><![CDATA[
    /// <Page
    ///  x:Class="MyPage"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:i="using:System.Windows.Interactivity"
    ///  xmlns:More="using:System.Windows.Interactivity">
    ///  <i:Interaction.Behaviors>
    ///   <More:ShareContractBehavior ShareRequest="{Binding InteractionRequests[Share]}" ShareCommand="{Binding Commands[Share]}" />
    ///  </i:Interaction.Behaviors>
    ///  <Grid>
    /// 
    ///  </Grid>
    /// </Page>
    /// ]]></code></example>
    [CLSCompliant( false )]
    public class ShareContractBehavior : System.Windows.Interactivity.Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the share request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ShareRequestProperty =
            DependencyProperty.Register( "ShareRequest", typeof( object ), typeof( ShareContractBehavior ), new PropertyMetadata( null, OnShareRequestChanged ) );

        /// <summary>
        /// Gets or sets the share command dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ShareCommandProperty =
            DependencyProperty.Register( "ShareCommand", typeof( ICommand ), typeof( ShareContractBehavior ), new PropertyMetadata( (object) null ) );

        private DataTransferManager dataTransferManager;

        private DataTransferManager DataTransferManager
        {
            get
            {
                return this.dataTransferManager;
            }
            set
            {
                if ( this.dataTransferManager == value )
                    return;

                if ( this.dataTransferManager != null )
                    this.dataTransferManager.DataRequested -= this.OnDataRequested;

                if ( ( this.dataTransferManager = value ) != null )
                    this.dataTransferManager.DataRequested += this.OnDataRequested;
            }
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Share flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Share flyout.</value>
        public IInteractionRequest ShareRequest
        {
            get
            {
                return (IInteractionRequest) this.GetValue( ShareRequestProperty );
            }
            set
            {
                this.SetValue( ShareRequestProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the command that is invoked when the Share flyout is shown.
        /// </summary>
        /// <value>The <see cref="ICommand">command</see> invoked when the Share flyout is shown.</value>
        /// <remarks>The parameter passed to the command will be an <see cref="IDataRequest"/>.</remarks>
        public ICommand ShareCommand
        {
            get
            {
                return (ICommand) this.GetValue( ShareCommandProperty );
            }
            set
            {
                this.SetValue( ShareCommandProperty, value );
            }
        }

        private static void OnShareRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (ShareContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
                oldRequest.Requested -= @this.OnShareRequested;

            if ( newRequest != null )
                newRequest.Requested += @this.OnShareRequested;
        }

        private void OnShareRequested( object sender, InteractionRequestedEventArgs e )
        {
            DataTransferManager.ShowShareUI();
        }

        private void OnDataRequested( DataTransferManager sender, DataRequestedEventArgs e )
        {
            var command = this.ShareCommand;

            if ( command == null )
                return;

            var request = new DataRequestAdapter( e.Request );

            if ( command.CanExecute( request ) )
                command.Execute( request );
        }

        private void OnAssociatedObjectUnloaded( object sender, RoutedEventArgs e )
        {
            this.Detach();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Unloaded += this.OnAssociatedObjectUnloaded;
            this.DataTransferManager = DataTransferManager.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.Unloaded -= this.OnAssociatedObjectUnloaded;
            this.DataTransferManager = null;
            base.OnDetaching();
        }
    }
}
