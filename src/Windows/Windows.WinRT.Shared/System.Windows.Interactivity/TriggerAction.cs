namespace System.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using global::Windows.UI.Xaml;
    using IAction = Microsoft.Xaml.Interactivity.IAction;

    /// <summary>
    /// Represents a triggered action.
    /// </summary>
    [CLSCompliant( false )]
    public abstract class TriggerAction : DependencyObject, IAction
    {
        /// <summary>
        /// Returns the requested <see cref="T:System.Windows.Input.Interaction">interaction</see> from the specified parameter.
        /// </summary>
        /// <typeparam name="TInteraction">The <see cref="Type">type</see> of <see cref="T:System.Windows.Input.Interaction">interaction</see> to retrieve.</typeparam>
        /// <param name="parameter">The parameter to retrieve the interaction from.</param>
        /// <returns>An <see cref="T:System.Windows.Input.Interaction">interaction</see> of type <typeparamref name="TInteraction"/> or
        /// <c>null</c> if the interaction cannot be retrieved.</returns>
        /// <remarks>The <see cref="T:System.Windows.Input.Interaction">interaction</see> is retrieved by converting the <paramref name="parameter"/>
        /// to <see cref="InteractionRequestedEventArgs"/> and extracting the <see cref="T:InteractionRequestedEventArgs.Interaction"/>, if possible.</remarks>
        protected static TInteraction GetRequestedInteraction<TInteraction>( object parameter ) where TInteraction : Interaction
        {
            var args = parameter as InteractionRequestedEventArgs;
            return args == null ? null : args.Interaction as TInteraction;
        }

        /// <summary>
        /// Executes the action asynchronously.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>A <see cref="Task">task</see> representing the operation.</returns>
        protected virtual Task ExecuteAsync( object sender, object parameter )
        {
            return Task.FromResult( 0 );
        }

        private async void ExecuteAsFireAndForget( object sender, object parameter )
        {
            await ExecuteAsync( sender, parameter );
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns <c>null</c>.</returns>
        public virtual object Execute( object sender, object parameter )
        {
            // because IAction.Execute has a return value, but is not designed for async; just return null
            ExecuteAsFireAndForget( sender, parameter );
            return null;
        }
    }
}
