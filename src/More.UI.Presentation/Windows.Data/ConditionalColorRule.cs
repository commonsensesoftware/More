namespace More.Windows.Data
{
    using System.ComponentModel;
    using System.Collections.ObjectModel;
#if UAP10_0
    using global::Windows.UI;
#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// Represents a rule to conditionally select a color based on a numeric value.
    /// </summary>
    public class ConditionalColorRule : NumericRule
    {
        /// <summary>
        /// Gets or sets the color associated with the rule.
        /// </summary>
        /// <value>A <see cref="Color"/> structure.</value>
        public Color Color { get; set; }
    }
}