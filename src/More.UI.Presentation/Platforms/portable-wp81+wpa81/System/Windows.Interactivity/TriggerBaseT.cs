namespace System.Windows.Interactivity
{
    using Microsoft.Xaml.Interactivity;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;    
    using System.Linq;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Markup;

    /// <summary>
    /// Represents a behavior triggered by a pre-defined event.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="DependencyObject">object</see> the behavior is associated with.</typeparam>
    [CLSCompliant( false )]
    [ContentProperty( Name = "Actions" )]
    public abstract class TriggerBase<T> : Behavior<T> where T : DependencyObject
    {
        readonly Lazy<ActionCollection> actions = new Lazy<ActionCollection>( () => new ActionCollection() );

        /// <summary>
        /// Get a collection of actions associated with the trigger.
        /// </summary>
        /// <value>An <see cref="ActionCollection">action collection</see>.</value>
        public ActionCollection Actions
        {
            get
            {
                Contract.Ensures( Contract.Result<ActionCollection>() != null );
                return actions.Value;
            }
        }

        /// <summary>
        /// Executes the trigger.
        /// </summary>
        /// <param name="sender">The sender of the event that triggered the behavior.</param>
        /// <param name="parameter">An optional parameter associated with the trigger. This parameter
        /// is typically the relevant event arguments.</param>
        protected virtual void Execute( object sender, object parameter ) =>
            Interaction.ExecuteActions( sender, Actions, parameter );
    }
}