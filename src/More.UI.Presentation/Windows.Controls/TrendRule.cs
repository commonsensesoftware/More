namespace More.Windows.Controls
{
    using More.Windows.Data;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a trend selection rule for the <see cref="TrendIndicator"/>.
    /// </summary>
    public partial class TrendRule : NumericRule
    {
        /// <summary>
        /// Gets or sets the visual state associated with the rule.
        /// </summary>
        /// <value>The trend visual state this rule identifies.</value>
        public string VisualState { get; set; }

        /// <summary>
        /// Evaluates the rule.
        /// </summary>
        /// <param name="item">The <see cref="Nullable{T}"/> to evalute.</param>
        /// <returns>True if the rule is satisified; otherwise, false.</returns>
        public override bool Evaluate( decimal? item ) => Rules.Any() && Rules.All( rule => rule.Evaluate( item ) );
    }
}