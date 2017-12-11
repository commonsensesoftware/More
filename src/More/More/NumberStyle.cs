namespace More
{
    using System;

    /// <summary>
    /// Represents the possible number styles.
    /// </summary>
    public enum NumberStyle
    {
        /// <summary>
        /// Indicates the default number style, which is the raw number.
        /// </summary>
        Default,

        /// <summary>
        /// Indicates the number is an integer.
        /// </summary>
        Integer,

        /// <summary>
        /// Indicates the number is a decimal.
        /// </summary>
        Decimal,

        /// <summary>
        /// Indicates the number is currency.
        /// </summary>
        Currency,

        /// <summary>
        /// Indicates the number is a percent.
        /// </summary>
        Percent,
    }
}