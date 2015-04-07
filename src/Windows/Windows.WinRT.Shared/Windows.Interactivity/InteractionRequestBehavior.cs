namespace More.Windows.Interactivity
{
    using Microsoft.Xaml.Interactivity;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;
    using global::Windows.ApplicationModel;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Markup;

    /// <summary>
    /// Represents a behavior which responds to <see cref="IInteractionRequest">interaction requests</see>.
    /// </summary>
    [CLSCompliant( false )]
    [ContentProperty( Name = "Actions" )]
    public class InteractionRequestBehavior : System.Windows.Interactivity.Behavior<DependencyObject>
    {
        /// <summary>
        /// Gets the actions dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register( "Actions", typeof( ActionCollection ), typeof( InteractionRequestBehavior ), new PropertyMetadata( null ) );

        /// <summary>
        /// Gets the request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty RequestProperty =
            DependencyProperty.Register( "Request", typeof( object ), typeof( InteractionRequestBehavior ), new PropertyMetadata( null, OnRequestChanged ) );

        private static void OnRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (InteractionRequestBehavior) sender;
            var oldRequest = e.OldValue as IInteractionRequest;
            var newRequest = e.NewValue as IInteractionRequest;

            if ( oldRequest != null )
                oldRequest.Requested -= behavior.OnRequested;

            if ( newRequest != null )
                newRequest.Requested += behavior.OnRequested;
        }

        private void OnRequested( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( DesignMode.DesignModeEnabled )
                return;

            var associatedObject = this.AssociatedObject;

            foreach ( var action in this.Actions.OfType<IAction>() )
            {
                var actionWithAssociatedObject = action as IActionWithAssociatedObject;

                if ( actionWithAssociatedObject != null )
                    actionWithAssociatedObject.Execute( sender, e, associatedObject );
                else
                    action.Execute( sender, e );
            }
        }

        /// <summary>
        /// Get a collection of actions associated with the behavior.
        /// </summary>
        /// <value>An <see cref="ActionCollection">action collection</see>.</value>
        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection) this.GetValue( ActionsProperty );

                if ( actions == null )
                {
                    actions = new ActionCollection();
                    this.SetValue( ActionsProperty, actions );
                }

                return actions;
            }
        }

        /// <summary>
        /// Gets or sets the interaction request associated with the behavior.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> associated with the behavior.</value>
        public IInteractionRequest Request
        {
            get
            {
                return (IInteractionRequest) this.GetValue( RequestProperty );
            }
            set
            {
                this.SetValue( RequestProperty, value );
            }
        }
    }
}
