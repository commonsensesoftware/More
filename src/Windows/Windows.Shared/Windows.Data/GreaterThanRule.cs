namespace More.Windows.Data
{
    using System;

    /// <summary>
    /// Represents a rule that evaluates the greater than operator.
    /// </summary>
    public class GreaterThanRule : NumericOperatorRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanRule"/> class.
        /// </summary>
        public GreaterThanRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanRule"/> class.
        /// </summary>
        /// <param name="value">The value associated with the rule.</param>
        public GreaterThanRule( decimal? value )
            : base( value )
        {
        }

        /// <summary>
        /// Evaluates the rule.
        /// </summary>
        /// <param name="item">The <see cref="Nullable{T}"/> to evalute.</param>
        /// <returns>True if the rule is satisified; otherwise, false.</returns>
        public override bool Evaluate( decimal? item )
        {
            var val = Value;
            return ( item.HasValue && val.HasValue && item.Value > val.Value ) || base.Evaluate( item );
        }
    }
}
