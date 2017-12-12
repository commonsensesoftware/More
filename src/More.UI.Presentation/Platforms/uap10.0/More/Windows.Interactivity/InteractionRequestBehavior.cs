namespace More.Windows.Interactivity
{
    using global::Windows.ApplicationModel;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Markup;
    using Microsoft.Xaml.Interactivity;
    using More.Windows.Input;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;

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
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register( nameof( Actions ), typeof( ActionCollection ), typeof( InteractionRequestBehavior ), new PropertyMetadata( null ) );

        /// <summary>
        /// Gets the request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty RequestProperty =
            DependencyProperty.Register( nameof( Request ), typeof( object ), typeof( InteractionRequestBehavior ), new PropertyMetadata( null, OnRequestChanged ) );

        static void OnRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var behavior = (InteractionRequestBehavior) sender;
            var oldRequest = e.OldValue as IInteractionRequest;
            var newRequest = e.NewValue as IInteractionRequest;

            if ( oldRequest != null )
            {
                oldRequest.Requested -= behavior.OnRequested;
            }

            if ( newRequest != null )
            {
                newRequest.Requested += behavior.OnRequested;
            }
        }

        void OnRequested( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( DesignMode.DesignModeEnabled )
            {
                return;
            }

            var associatedObject = AssociatedObject;

            foreach ( var action in Actions.OfType<IAction>() )
            {
                if ( action is IActionWithAssociatedObject actionWithAssociatedObject )
                {
                    actionWithAssociatedObject.Execute( sender, e, associatedObject );
                }
                else
                {
                    action.Execute( sender, e );
                }
            }
        }

        /// <summary>
        /// Gets a collection of actions associated with the behavior.
        /// </summary>
        /// <value>An <see cref="ActionCollection">action collection</see>.</value>
        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection) GetValue( ActionsProperty );

                if ( actions == null )
                {
                    actions = new ActionCollection();
                    SetValue( ActionsProperty, actions );
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
            get => (IInteractionRequest) GetValue( RequestProperty );
            set => SetValue( RequestProperty, value );
        }
    }
}