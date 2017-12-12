namespace More.Windows.Data
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using static System.String;
    using static System.Text.RegularExpressions.Regex;
    using static System.Text.RegularExpressions.RegexOptions;

    /// <summary>
    /// Represents a numeric denomination inverse conversion rule.
    /// </summary>
    public class DenominationConvertBackRule : IRule<string, bool>
    {
        decimal denomination = 1m;

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
        /// Gets or sets the regular expression pattern associated with the rule.
        /// </summary>
        /// <value>The regular expression pattern associated with the rule.</value>
        public string RegexPattern { get; set; }

        /// <summary>
        /// Evaluates the rule.
        /// </summary>
        /// <param name="item">The <see cref="string">string</see> to evalute.</param>
        /// <returns>True if the rule is satisified; otherwise, false.</returns>
        public virtual bool Evaluate( string item ) =>
            !IsNullOrEmpty( RegexPattern ) && IsMatch( item, RegexPattern, Singleline );
    }
}