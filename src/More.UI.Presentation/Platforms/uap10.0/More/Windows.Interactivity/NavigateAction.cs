namespace More.Windows.Interactivity
{
    using global::Windows.UI.Xaml;
    using More.Windows.Input;
    using System;

    /// <summary>
    /// Represents an <see cref="System.Windows.Interactivity.TriggerAction">interactivity action</see> that can be used to show the <see cref="NavigateInteraction">interaction</see>
    /// from an <see cref="IInteractionRequest.Requested">interaction request</see>.
    /// </summary>
    [CLSCompliant( false )]
    public partial class NavigateAction : System.Windows.Interactivity.TriggerAction, IActionWithAssociatedObject
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>True if the action performed a navigation operation; otherwise, false.</returns>
        public override object Execute( object sender, object parameter ) => Execute( sender, parameter, null );

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <param name="associatedObject">The associated <see cref="DependencyObject">dependency object</see>.</param>
        /// <returns>True if the action performed a navigation operation; otherwise, false.</returns>
        public object Execute( object sender, object parameter, DependencyObject associatedObject )
        {
            var interaction = GetRequestedInteraction<NavigateInteraction>( parameter );

            if ( interaction == null )
            {
                return false;
            }

            var sourceObject = associatedObject ?? sender;

            if ( Util.TryResolveNavigationService( sourceObject, out var service ) )
            {
                return service.Navigate( interaction.Url, interaction.Content );
            }

            return false;
        }
    }
}