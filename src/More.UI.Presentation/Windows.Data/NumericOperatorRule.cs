namespace More.Windows.Data
{
    using System;

    /// <summary>
    /// Represents the base class for a numeric rule.
    /// </summary>
    public abstract class NumericOperatorRule : NumericRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericOperatorRule"/> class.
        /// </summary>
        protected NumericOperatorRule() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericOperatorRule"/> class.
        /// </summary>
        /// <param name="value">The value associated with the rule.</param>
        protected NumericOperatorRule( decimal? value ) => Value = value;

        /// <summary>
        /// Gets or sets the value associated with the rule.
        /// </summary>
        /// <value>The number representing the value associated with the rule.</value>
        public decimal? Value { get; set; }
    }
}