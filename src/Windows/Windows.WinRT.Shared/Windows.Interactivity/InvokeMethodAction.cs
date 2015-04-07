namespace More.Windows.Interactivity
{
    using System;
    using System.Windows.Interactivity;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Markup;

    /// <content>
    /// Provides additional implementation for Windows Runtime applications.
    /// </content>
    [CLSCompliant( false )]
    [ContentProperty( Name = "Parameters" )]
    public partial class InvokeMethodAction : System.Windows.Interactivity.TriggerAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The object that triggered the action.</param>
        /// <param name="parameter">The parameter provided to the action.</param>
        /// <returns>The result of the action. The default implementation always executes asynchronously and returns null.</returns>
        public override object Execute( object sender, object parameter )
        {
            // ensure there is a valid method to execute
            if ( string.IsNullOrEmpty( this.MethodName ) )
                return ( this.ReturnValue = null );

            this.EnsureDataBinding( sender );

            // use target override or failover to associated object
            var target = this.ReadLocalValue( TargetObjectProperty ) == DependencyProperty.UnsetValue ? sender : this.TargetObject;

            if ( target == null )
                return ( this.ReturnValue = null );

            // get resolved method or resolve it now
            var method = this.GetOrResolveMethod( target.GetType() );

            // method binding failed
            if ( method == null )
                return ( this.ReturnValue = null );

            // invoke method and capture return value
            return ( this.ReturnValue = method.Invoke( target, this.Parameters ) );
        }
    }
}
