namespace More.Windows.Interactivity
{
    using System;
    using System.Windows;
    using System.Windows.Markup;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    [ContentProperty( nameof( Parameters ) )]
    public partial class InvokeMethodAction : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        /// <summary>
        /// Occurs when the trigger action is invoked.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> for the event that triggered the action.</param>
        protected virtual void Invoke( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            if ( string.IsNullOrEmpty( MethodName ) )
            {
                ReturnValue = null;
                return;
            }

            EnsureDataBinding( AssociatedObject );

            var target = ReadLocalValue( TargetObjectProperty ) == DependencyProperty.UnsetValue ? AssociatedObject : TargetObject;
            var method = GetOrResolveMethod( target.GetType() );

            if ( method == null )
            {
                ReturnValue = null;
            }
            else
            {
                ReturnValue = method.Invoke( target, Parameters );
            }
        }

        /// <summary>
        /// Overrides the default behavior when the trigger action is invoked.
        /// </summary>
        /// <param name="parameter">The parameter supplied by the trigger that invoked the action.</param>
        protected sealed override void Invoke( object parameter ) => Invoke( ( parameter as EventArgs ) ?? EventArgs.Empty );
    }
}