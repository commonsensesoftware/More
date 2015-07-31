namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Represents a behavior triggered by a pre-defined event.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="DependencyObject">object</see> the behavior is associated with.</typeparam>
    [ContentProperty( "Actions" )]
    public abstract class Trigger<T> : System.Windows.Interactivity.Behavior<T> where T : DependencyObject
    {
        /// <summary>
        /// Represents a proxy that exposes the capabilities of a TriggerBase to a Behavior.
        /// </summary>
        /// <remarks>Not all platforms use triggers, but all platforms do use behaviors. This approach affords for
        /// cross-platform consistency in API and XAML.</remarks>
        private sealed class TriggerProxy : System.Windows.Interactivity.TriggerBase<T>
        {
            /// <summary>
            /// Executes the actions associated with the trigger.
            /// </summary>
            /// <param name="parameter">A user-defined parameter.</param>
            /// <remarks>The TriggerAction.CallInvoke method is internal and the TriggerAction.Invoke method is protected.
            /// To avoid using Refelction, this method is used as the proxy mechanism to trigger the execution of any
            /// associated actions.
            /// </remarks>
            internal void Execute( object parameter )
            {
                InvokeActions( parameter );
            }
        }

        private readonly TriggerProxy proxy = new TriggerProxy();

        /// <summary>
        /// Get a collection of actions associated with the trigger.
        /// </summary>
        /// <value>An <see cref="TriggerActionCollection">action collection</see>.</value>
        public System.Windows.Interactivity.TriggerActionCollection Actions
        {
            get
            {
                Contract.Ensures( Contract.Result<System.Windows.Interactivity.TriggerActionCollection>() != null );
                return proxy.Actions;
            }
        }

        /// <summary>
        /// Executes the trigger.
        /// </summary>
        /// <param name="sender">The sender of the event that triggered the behavior.</param>
        /// <param name="parameter">An optional parameter associated with the trigger. This parameter
        /// is typically the relevant event arguments.</param>
        protected virtual void Execute( object sender, object parameter )
        {
            proxy.Execute( parameter );
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            proxy.Attach( AssociatedObject );
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            proxy.Detach();
        }
    }
}
