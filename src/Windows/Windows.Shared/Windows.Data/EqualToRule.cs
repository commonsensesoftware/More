namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a rule that evaluates the equality operator.
    /// </summary>
    public class EqualToRule : NumericOperatorRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualToRule"/> class.
        /// </summary>
        public EqualToRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualToRule"/> class.
        /// </summary>
        /// <param name="value">The value associated with the rule.</param>
        public EqualToRule( decimal? value )
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
            return Nullable.Equals( val, item ) || base.Evaluate( item );
        }
    }
}
