namespace More.Windows.Controls
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Represents an optional data grid text column.
    /// </summary>
    public class OptionalDataGridTextColumn : DataGridTextColumn, IOptionalDataGridColumn, IDataGridBoundColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalDataGridTextColumn"/> class.
        /// </summary>
        public OptionalDataGridTextColumn()
        {
            IsOptional = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is optional.
        /// </summary>
        /// <value>True if the column is optional; otherwise, false.</value>
        public bool IsOptional
        {
            get;
            set;
        }
    }
}
