namespace More.Windows.Interactivity
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interactivity;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
#endif

    /// <summary>
    /// Represents an action that invokes the method of a bound object.
    /// </summary>
    /// <example>This example demonstrates how to define an action to invoke an object method.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="http://schemas.microsoft.com/expression/2010/interactivity"
    ///  xmlns:More="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.More">
    ///  
    /// <Grid x:Name="LayoutRoot">
    ///  <Button Content="Save" Height="25" Width="75">
    ///   <Interactivity:Interaction.Triggers>
    ///    <Interactivity:EventTrigger EventName="Click">
    ///     <More:InvokeMethodAction Target="{Binding MyViewModel}" MethodName="Save" />
    ///    </Interactivity:EventTrigger>
    ///   </Interactivity:Interaction.Triggers>
    ///  </Button>
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
    /// <example>This example demonstrates how to define an action to invoke an object method with parameters.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyUserControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Interactivity="http://schemas.microsoft.com/expression/2010/interactivity"
    ///  xmlns:More="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.More">
    ///  
    /// <Grid x:Name="LayoutRoot">
    ///  <Grid.RowDefinitions>
    ///   <RowDefinition />
    ///   <RowDefinition />
    ///  </Grid.RowDefinitions>
    ///  <StackPanel Orientation="Horizontal">
    ///   <TextBlock Text="Search text: " />
    ///   <TextBox x:Name="SearchText" MinWidth="50" />
    ///  </StackPanel>
    ///  <Button Content="Search" Height="25" Width="75" VerticalAlignment="Top" Grid.Row="1">
    ///   <Interactivity:Interaction.Triggers>
    ///    <Interactivity:EventTrigger EventName="Click">
    ///     <More:InvokeMethodAction Target="{Binding MyViewModel}" MethodName="Search">
    ///      <More:MethodParameter ParameterType="System.String" ParameterValue="{Binding Text, ElementName=SearchText}" />
    ///     </More:InvokeMethodAction>
    ///    </Interactivity:EventTrigger>
    ///   </Interactivity:Interaction.Triggers>
    ///  </Button>
    /// </Grid>
    /// 
    /// </UserControl>
    /// ]]></code></example>
    public partial class InvokeMethodAction
    {
        private sealed class MethodSpecification : SpecificationBase<Tuple<Type, InvokeMethodAction>>
        {
            private readonly Type targetType;
            private readonly string methodName;
            private readonly IReadOnlyList<ParameterInfo> parameters;

            internal MethodSpecification( Type targetType, MethodInfo targetMethod )
            {
                Contract.Requires( targetType != null );
                Contract.Requires( targetMethod != null );

                this.targetType = targetType;
                this.methodName = targetMethod.Name;
                this.parameters = targetMethod.GetParameters();
            }

            public override bool IsSatisfiedBy( Tuple<Type, InvokeMethodAction> item )
            {
                if ( item == null )
                    return false;

                var otherType = item.Item1;

                if ( this.targetType != otherType )
                    return false;

                var action = item.Item2;

                if ( this.methodName != action.MethodName )
                    return false;

                // match on parameter count only
                // note: this could be enhanced to verify parameter types too
                return this.parameters.Count == action.Parameters.Count;
            }
        }

        /// <summary>
        /// Gets the target dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register( "TargetObject", typeof( object ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the property method name dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register( "MethodName", typeof( string ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the return value dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ReturnValueProperty =
            DependencyProperty.Register( "ReturnValue", typeof( object ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        private readonly Lazy<Collection<MethodParameter>> parameters = new Lazy<Collection<MethodParameter>>( () => new Collection<MethodParameter>() );
        private bool setBinding = true;
        private MethodInfo targetMethod;
        private ISpecification<Tuple<Type, InvokeMethodAction>> specification;

        /// <summary>
        /// Gets or sets the object representing the target object to invoke the method on.
        /// </summary>
        /// <value>The target <see cref="Object">object</see>.</value>
        public object TargetObject
        {
            get
            {
                return this.GetValue( TargetObjectProperty );
            }
            set
            {
                this.SetValue( TargetObjectProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the name of the method to invoke.
        /// </summary>
        /// <value>The name of the method to invoke.</value>
        public string MethodName
        {
            get
            {
                return (string) this.GetValue( MethodNameProperty );
            }
            set
            {
                this.SetValue( MethodNameProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the return value of the invoked method.
        /// </summary>
        /// <value>The method return value.</value>
        public object ReturnValue
        {
            get
            {
                return this.GetValue( ReturnValueProperty );
            }
            set
            {
                this.SetValue( ReturnValueProperty, value );
            }
        }

        /// <summary>
        /// Gets a collection of parameters for the method to invoke.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        public virtual Collection<MethodParameter> Parameters
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<MethodParameter>>() != null );
                return this.parameters.Value;
            }
        }

        private void EnsureDataBinding( object sender )
        {
            if ( !this.setBinding )
                return;

            var element = sender as FrameworkElement;

            this.SetDataContext( TargetObjectProperty, element );

            foreach ( var param in this.Parameters )
                param.SetDataContext( MethodParameter.ParameterValueProperty, element );

            this.setBinding = false;
        }

        private MethodInfo GetOrResolveMethod( Type targetType )
        {
            Contract.Requires( targetType != null );

            // non-null after first pass
            if ( this.specification != null )
            {
                var item = new Tuple<Type, InvokeMethodAction>( targetType, this );

                // if specification is met, use resolved method
                if ( this.specification.IsSatisfiedBy( item ) )
                    return this.targetMethod;

                // a new method will be resolved, so trigger binding update
                this.setBinding = true;
            }

            this.targetMethod = ReflectHelper.GetMatchingMethod( targetType, this.MethodName, this.Parameters );
            this.specification = new MethodSpecification( targetType, this.targetMethod );
            return this.targetMethod;
        }
    }
}
