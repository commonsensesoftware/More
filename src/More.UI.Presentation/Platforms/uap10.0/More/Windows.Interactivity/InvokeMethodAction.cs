namespace More.Windows.Interactivity
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Markup;
    using System;

    /// <content>
    /// Provides additional implementation for Windows Runtime applications.
    /// </content>
    [CLSCompliant( false )]
    [ContentProperty( Name = nameof( Parameters ) )]
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
            if ( string.IsNullOrEmpty( MethodName ) )
            {
                return ReturnValue = null;
            }

            EnsureDataBinding( sender );

            var target = ReadLocalValue( TargetObjectProperty ) == DependencyProperty.UnsetValue ? sender : TargetObject;

            if ( target == null )
            {
                return ReturnValue = null;
            }

            var method = GetOrResolveMethod( target.GetType() );

            if ( method == null )
            {
                return ReturnValue = null;
            }

            return ReturnValue = method.Invoke( target, Parameters );
        }
    }
}