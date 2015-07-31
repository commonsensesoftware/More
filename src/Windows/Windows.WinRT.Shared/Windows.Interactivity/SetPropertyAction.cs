namespace More.Windows.Interactivity
{
    using System;
    using global::Windows.UI.Xaml;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    [CLSCompliant( false )]
    public partial class SetPropertyAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns null.</returns>
        public override object Execute( object sender, object parameter )
        {
            // use target override or failover to associated object
            var target = ReadLocalValue( TargetObjectProperty ) == DependencyProperty.UnsetValue ? sender : TargetObject;

            // ensure there is a valid method to execute
            if ( target == null && string.IsNullOrEmpty( PropertyName ) )
                return null;

            var targetType = target.GetType();
            var value = PropertyValue;

            // set property and return set value
            // note: set and capture the property on the first execution. reuse the resolved property on subsequent executions.
            if ( targetObjectType == targetType && targetProperty != null )
            {
                targetProperty.SetValue( target, value );
            }
            else
            {
                targetProperty = ReflectHelper.SetProperty( target, PropertyName, value );
                targetObjectType = targetType;
            }

            return value;
        }
    }
}
