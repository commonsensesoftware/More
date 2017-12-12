namespace More.Windows.Interactivity
{
    using global::Windows.UI.Xaml;
    using Microsoft.Xaml.Interactivity;
    using System;

    /// <summary>
    /// Represents an <see cref="IAction">action</see> with an <see cref="DependencyObject">associated object</see>.
    /// </summary>
    [CLSCompliant( false )]
    public interface IActionWithAssociatedObject : IAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <param name="associatedObject">The associated <see cref="DependencyObject">dependency object</see>.</param>
        /// <returns>The result of the action.</returns>
        object Execute( object sender, object parameter, DependencyObject associatedObject );
    }
}