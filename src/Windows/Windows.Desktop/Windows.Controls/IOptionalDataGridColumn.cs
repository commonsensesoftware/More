namespace More.Windows.Controls
{
    using System;

    /// <summary>
    /// Defines the behavior of an optional data grid column.
    /// </summary>
    public interface IOptionalDataGridColumn
    {
        /// <summary>
        /// Gets or sets a value indicating whether the column is optional.
        /// </summary>
        /// <value>True if the column is optional; otherwise, false.</value>
        bool IsOptional
        {
            get;
            set;
        }
    }
}
