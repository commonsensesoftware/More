namespace More.Windows.Data
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
#if UAP10_0
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Markup;
#else
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Represents a rule that matches a format to a specific number style.
    /// </summary>
    public class NumberStyleRule : IRule<Number?, bool>
    {
        /// <summary>
        /// Gets or sets the number style this rule applies to.
        /// </summary>
        /// <value>One of the <see cref="NumberStyle"/> values.</value>
        public NumberStyle NumberStyle { get; set; }

        /// <summary>
        /// Gets or sets the string format associated with the number style.
        /// </summary>
        /// <value>A <see cref="string">string</see> representing the number format to apply.  This property can be null.</value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the value converter associated with the number style.
        /// </summary>
        /// <value>An <see cref="IValueConverter"/> used to format the number. This property can be null.</value>
#if UAP10_0
        [CLSCompliant( false )]
#endif
        public IValueConverter ValueConverter { get; set; }

        /// <summary>
        /// Evaluates the rule.
        /// </summary>
        /// <param name="item">The <see cref="Nullable{T}"/> to evalute.</param>
        /// <returns>True if the rule is satisified; otherwise, false.</returns>
        public bool Evaluate( Number? item ) => item != null && item.Value.NumberStyle == NumberStyle;
    }
}