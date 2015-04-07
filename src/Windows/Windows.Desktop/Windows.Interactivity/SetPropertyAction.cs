namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Interactivity;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    public partial class SetPropertyAction : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// Occurs when the trigger action is invoked.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> for the event that triggered the action.</param>
        protected virtual void Invoke( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            // use target override or failover to associated object
            var target = this.TargetObject ?? this.AssociatedObject;

            // ensure there is a valid method to execute
            if ( target == null && string.IsNullOrEmpty( this.PropertyName ) )
                return;

            var targetType = target.GetType();

            // set property and return set value
            // note: set and capture the property on the first execution. reuse the resolved property on subsequent executions.
            if ( this.targetObjectType == targetType && this.targetProperty != null )
            {
                this.targetProperty.SetValue( target, this.PropertyValue );
            }
            else
            {
                this.targetProperty = ReflectHelper.SetProperty( target, this.PropertyName, this.PropertyValue );
                this.targetObjectType = targetType;
            }
        }

        /// <summary>
        /// Overrides the default behavior when the trigger action is invoked.
        /// </summary>
        /// <param name="parameter">The parameter supplied by the trigger that invoked the action.</param>
        protected sealed override void Invoke( object parameter )
        {
            this.Invoke( ( (EventArgs) parameter ) ?? EventArgs.Empty );
        }
    }
}
