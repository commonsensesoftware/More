namespace More.Windows.Controls
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Represents an optional data grid check box column.
    /// </summary>
    public class OptionalDataGridCheckBoxColumn : DataGridCheckBoxColumn, IOptionalDataGridColumn, IDataGridBoundColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalDataGridCheckBoxColumn"/> class.
        /// </summary>
        public OptionalDataGridCheckBoxColumn()
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
