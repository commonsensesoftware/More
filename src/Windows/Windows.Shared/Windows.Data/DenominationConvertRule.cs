namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a numeric denomination conversion rule.
    /// </summary>
    public class DenominationConvertRule : NumericRule
    {
        private decimal denomination = 1m;

        /// <summary>
        /// Gets or sets the denomination associated with the rule.
        /// </summary>
        /// <value>The denomination associated with the rule.  The default value is one.</value>
        public decimal Denomination
        {
            get
            {
                Contract.Ensures( Contract.Result<decimal>() > 0m );
                return denomination;
            }
            set
            {
                Arg.GreaterThan( value, 0m, nameof( value ) );
                denomination = value;
            }
        }

        /// <summary>
        /// Gets or sets the format associated with the rule.
        /// </summary>
        /// <value>The string format for the value associated with the rule.</value>
        public string Format
        {
            get;
            set;
        }
    }
}