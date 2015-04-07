namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Input;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
    using TypeConstraint = global::Microsoft.Xaml.Interactivity.TypeConstraintAttribute;
#endif

    /// <summary>
    /// Represents a trigger that is invoked when a key is pressed.
    /// </summary>
    /// <example>This example demonstrates how to define an action to invoke a command when a key is pressed on an element.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="http://schemas.microsoft.com/expression/2010/interactivity"
    ///  xmlns:More="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.More">
    ///  
    /// <Grid x:Name="LayoutRoot">
    ///  <StackPanel Orientation="Horizontal">
    ///   <TextBlock Text="Search text: " />
    ///   <TextBox x:Name="SearchText" MinWidth="50">
    ///    <!-- trigger the 'Search' operation when the 'Enter' key is pressed in the textbox -->
    ///    <Interactivity:Interaction.Behaviors>
    ///     <More:KeyPressTrigger Key="Enter">
    ///      <More:InvokeMethodAction Target="{Binding MyViewModel}" MethodName="Search">
    ///       <More:MethodParameter ParameterType="System.String" ParameterValue="{Binding Text, ElementName=SearchText}" />
    ///      </More:InvokeMethodAction>
    ///     </More:KeyPressTrigger>
    ///    </Interactivity:Interaction.Behaviors>
    ///   </TextBox>
    ///  </StackPanel>
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    [TypeConstraint( typeof( UIElement ) )]
    public class KeyPressTrigger : TriggerBase<UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPressTrigger"/> class.
        /// </summary>
        public KeyPressTrigger()
        {   
            this.Key = Key.Enter;
        }

        /// <summary>
        /// Gets or sets the key that invokes the trigger.
        /// </summary>
        /// <value>The <see cref="Key">key</see> associated with the trigger.  The default value is <see cref="T:Key.Enter">ENTER</see>.</value>
        public Key Key
        {
            get;
            set;
        }

        private void OnKeyUp( object sender, KeyEventArgs e )
        {
            Contract.Requires( e != null );

            if ( e.Key != this.Key )
                return;

            this.OnKeyPress( e );

            if ( !e.Handled )
            {
#if NETFX_CORE
                this.Execute( sender, e );
#else
                this.InvokeActions( e );
#endif
            }
        }

        /// <summary>
        /// Occurs when the behavior is triggered by a key press.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> event data.</param>
        protected virtual void OnKeyPress( KeyEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyUp += OnKeyUp;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.KeyUp -= this.OnKeyUp;
            base.OnDetaching();
        }
    }
}
