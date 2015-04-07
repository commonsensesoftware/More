namespace More.Windows.Interactivity
{
    using More.Configuration;
    using System;
    using System.ComponentModel;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Configuration;
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
        public static readonly DependencyProperty ParameterTypeProperty =
            DependencyProperty.Register( "ParameterType", typeof( Type ), typeof( MethodParameter ), new PropertyMetadata( (object) null ) );

        /// <summary>
        /// Gets the parameter value dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ParameterValueProperty =
            DependencyProperty.Register( "ParameterValue", typeof( object ), typeof( MethodParameter ), new PropertyMetadata( (object) null ) );

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
                return (Type) this.GetValue( ParameterTypeProperty );
            }
            set
            {
                this.SetValue( ParameterTypeProperty, value );
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
                return this.GetValue( ParameterValueProperty );
            }
            set
            {
                this.SetValue( ParameterValueProperty, value );
            }
        }
    }
}
