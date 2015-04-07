namespace More.Windows.Data
{
    using System;

    /// <summary>
    /// Represents the possible column build orders.
    /// </summary>
    [Flags]
    public enum ColumnBuildOrders
    {
        /// <summary>
        /// Indicates that a column build order was not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that columns will be built-up according to display index.
        /// </summary>
        DisplayIndex = 1,

        /// <summary>
        /// Indicates that columns will be built-up according to name.
        /// </summary>
        Name = 2,

        /// <summary>
        /// Indicates that columns will be built-up in ascending order.
        /// </summary>
        Ascending = 4,

        /// <summary>
        /// Indicates that columns will be built-up in descending order.
        /// </summary>
        Descending = 8,

        /// <summary>
        /// Indicates the default column builder order.
        /// </summary>
        /// <value>The default value is <see cref="DisplayIndex"/> and <see cref="Ascending"/>.</value>
        Default = DisplayIndex | Ascending
    }
}
