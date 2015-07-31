namespace More.Windows.Interactivity
{
    using System;
    using System.ComponentModel;
#if NETFX_CORE
    using System.Diagnostics.CodeAnalysis;
    using global::Windows.UI.Xaml;
#else
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
#endif

    /// <summary>
    /// Represents a method parameter.
    /// </summary>
#if NETFX_CORE
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
#if !NETFX_CORE
        [TypeConverter( typeof( TypeNameConverter ) )]
#endif
        public Type ParameterType
        {
            get
            {
                return (Type) GetValue( ParameterTypeProperty );
            }
            set
            {
                SetValue( ParameterTypeProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        public object ParameterValue
        {
            get
            {
                return GetValue( ParameterValueProperty );
            }
            set
            {
                SetValue( ParameterValueProperty, value );
            }
        }
    }
}
