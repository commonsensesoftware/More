namespace More.Windows.Interactivity
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    [ContentProperty( "Parameters" )]
    public partial class InvokeMethodAction : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        private void Invoke()
        {
            // ensure there is a valid method to execute
            if ( string.IsNullOrEmpty( this.MethodName ) )
            {
                this.ReturnValue = null;
                return;
            }

            // TriggerAction<T> is not a FrameworkElement, therefore, we need to ensure the Source of
            // parameter bindings by using the DataContext of the associated object
            this.EnsureDataBinding( this.AssociatedObject );

            // use target or failover to associated object
            var target = this.ReadLocalValue( TargetObjectProperty ) == DependencyProperty.UnsetValue ? this.AssociatedObject : this.TargetObject;
            var method = this.GetOrResolveMethod( target.GetType() );

            // invoke method and capture return value
            if ( method == null )
                this.ReturnValue = null;
            else
                this.ReturnValue = method.Invoke( target, this.Parameters );
        }

        /// <summary>
        /// Occurs when the trigger action is invoked.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> for the event that triggered the action.</param>
        protected virtual void Invoke( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );
            this.Invoke();
        }

        /// <summary>
        /// Overrides the default behavior when the trigger action is invoked.
        /// </summary>
        /// <param name="parameter">The parameter supplied by the trigger that invoked the action.</param>
        protected sealed override void Invoke( object parameter )
        {
            this.Invoke( ( parameter as EventArgs ) ?? EventArgs.Empty );
        }
    }
}
