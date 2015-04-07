namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts; 
#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// Represents a rule to conditionally select a brush based on a numeric value.
    /// </summary>
    public class ConditionalBrushRule : NumericRule
    {
        private Brush brush = new SolidColorBrush( Colors.Gray );

        /// <summary>
        /// Gets or sets the brush associated with the rule.
        /// </summary>
        /// <value>A <see cref="Brush">brush</see> object. The default value is a <see cref="P:Colors.Gray">gray</see>
        /// <see cref="SolidColorBrush">brush</see>.</value>
#if NETFX_CORE
        [CLSCompliant( false )]
#endif
        public Brush Brush
        {
            get
            {
                Contract.Ensures( Contract.Result<Brush>() != null ); 
                return this.brush;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.brush = value;
            }
        }
    }
}
