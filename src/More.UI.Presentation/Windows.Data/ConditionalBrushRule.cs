namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts; 
#if UAP10_0
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
        Brush brush = new SolidColorBrush( Colors.Gray );

        /// <summary>
        /// Gets or sets the brush associated with the rule.
        /// </summary>
        /// <value>A <see cref="Brush">brush</see> object. The default value is a <see cref="P:Colors.Gray">gray</see>
        /// <see cref="SolidColorBrush">brush</see>.</value>
#if UAP10_0
        [CLSCompliant( false )]
#endif
        public Brush Brush
        {
            get
            {
                Contract.Ensures( Contract.Result<Brush>() != null ); 
                return brush;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                brush = value;
            }
        }
    }
}