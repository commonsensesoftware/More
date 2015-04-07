namespace More.Windows.Data
{
    using System;

    /// <summary>
    /// Represents a rule that evaluates the less than operator.
    /// </summary>
    public class LessThanRule : NumericOperatorRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanRule"/> class.
        /// </summary>
        public LessThanRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanRule"/> class.
        /// </summary>
        /// <param name="value">The value associated with the rule.</param>
        public LessThanRule( decimal? value )
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
            var val = this.Value;
            return ( item.HasValue && val.HasValue && item.Value < val.Value ) || base.Evaluate( item );
        }
    }
}
