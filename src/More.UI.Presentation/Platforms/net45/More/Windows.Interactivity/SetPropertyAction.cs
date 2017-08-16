namespace More.Windows.Interactivity
{
    using System;
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
            Arg.NotNull( e, nameof( e ) );

            var target = TargetObject ?? AssociatedObject;

            if ( target == null && string.IsNullOrEmpty( PropertyName ) )
            {
                return;
            }

            var targetType = target.GetType();

            if ( targetObjectType == targetType && targetProperty != null )
            {
                targetProperty.SetValue( target, PropertyValue );
            }
            else
            {
                targetProperty = ReflectHelper.SetProperty( target, PropertyName, PropertyValue );
                targetObjectType = targetType;
            }
        }

        /// <summary>
        /// Overrides the default behavior when the trigger action is invoked.
        /// </summary>
        /// <param name="parameter">The parameter supplied by the trigger that invoked the action.</param>
        protected sealed override void Invoke( object parameter ) => Invoke( ( (EventArgs) parameter ) ?? EventArgs.Empty );
    }
}