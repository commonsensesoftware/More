namespace More.Windows.Interactivity
{
    using System;
    using System.ComponentModel;
#if UAP10_0
    using global::Windows.UI.Xaml;
    using System.Diagnostics.CodeAnalysis;
#else
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
#endif

    /// <summary>
    /// Represents a method parameter.
    /// </summary>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    public class MethodParameter : DependencyObject
    {
        /// <summary>
        /// Gets the parameter type dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ParameterTypeProperty =
            DependencyProperty.Register( nameof( ParameterType ), typeof( Type ), typeof( MethodParameter ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the parameter value dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ParameterValueProperty =
            DependencyProperty.Register( nameof( ParameterValue ), typeof( object ), typeof( MethodParameter ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets or sets the parameter type.
        /// </summary>
        /// <value>The parameter <see cref="ParameterType">type</see>.</value>
#if NET45
        [TypeConverter( typeof( TypeNameConverter ) )]
#endif
        public Type ParameterType
        {
            get => (Type) GetValue( ParameterTypeProperty );
            set => SetValue( ParameterTypeProperty, value );
        }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        public object ParameterValue
        {
            get => GetValue( ParameterValueProperty );
            set => SetValue( ParameterValueProperty, value );
        }
    }
}