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
#if UAP10_0
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
        /// <summary>
        /// Gets the target dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register( nameof( TargetObject ), typeof( object ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the property method name dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the return value dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ReturnValueProperty =
            DependencyProperty.Register( nameof( ReturnValue ), typeof( object ), typeof( InvokeMethodAction ), new PropertyMetadata( (object) null ) );

        readonly Lazy<Collection<MethodParameter>> parameters = new Lazy<Collection<MethodParameter>>( () => new Collection<MethodParameter>() );
        bool setBinding = true;
        MethodInfo targetMethod;
        ISpecification<Tuple<Type, InvokeMethodAction>> specification;

        /// <summary>
        /// Gets or sets the object representing the target object to invoke the method on.
        /// </summary>
        /// <value>The target <see cref="object">object</see>.</value>
        public object TargetObject
        {
            get => GetValue( TargetObjectProperty );
            set => SetValue( TargetObjectProperty, value );
        }

        /// <summary>
        /// Gets or sets the name of the method to invoke.
        /// </summary>
        /// <value>The name of the method to invoke.</value>
        public string MethodName
        {
            get => (string) GetValue( MethodNameProperty );
            set => SetValue( MethodNameProperty, value );
        }

        /// <summary>
        /// Gets or sets the return value of the invoked method.
        /// </summary>
        /// <value>The method return value.</value>
        public object ReturnValue
        {
            get => GetValue( ReturnValueProperty );
            set => SetValue( ReturnValueProperty, value );
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
                return parameters.Value;
            }
        }

        void EnsureDataBinding( object sender )
        {
            if ( !setBinding )
            {
                return;
            }

            var element = sender as FrameworkElement;

            this.SetDataContext( TargetObjectProperty, element );

            foreach ( var param in Parameters )
            {
                param.SetDataContext( MethodParameter.ParameterValueProperty, element );
            }

            setBinding = false;
        }

        MethodInfo GetOrResolveMethod( Type targetType )
        {
            Contract.Requires( targetType != null );

            if ( specification != null )
            {
                var item = Tuple.Create( targetType, this );

                if ( specification.IsSatisfiedBy( item ) )
                {
                    return targetMethod;
                }

                setBinding = true;
            }

            targetMethod = ReflectHelper.GetMatchingMethod( targetType, MethodName, Parameters );
            specification = new MethodSpecification( targetType, targetMethod );
            return targetMethod;
        }

        sealed class MethodSpecification : SpecificationBase<Tuple<Type, InvokeMethodAction>>
        {
            readonly Type targetType;
            readonly string methodName;
            readonly IReadOnlyList<ParameterInfo> parameters;

            internal MethodSpecification( Type targetType, MethodInfo targetMethod )
            {
                Contract.Requires( targetType != null );
                Contract.Requires( targetMethod != null );

                this.targetType = targetType;
                methodName = targetMethod.Name;
                parameters = targetMethod.GetParameters();
            }

            public override bool IsSatisfiedBy( Tuple<Type, InvokeMethodAction> item )
            {
                if ( item == null )
                {
                    return false;
                }

                var otherType = item.Item1;

                if ( targetType != otherType )
                {
                    return false;
                }

                var action = item.Item2;

                if ( methodName != action.MethodName )
                {
                    return false;
                }

                // match on parameter count only
                // note: this could be enhanced to verify parameter types too
                return parameters.Count == action.Parameters.Count;
            }
        }
    }
}