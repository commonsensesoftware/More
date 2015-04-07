namespace More.Windows.Controls
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Represents a bindable data grid column.
    /// </summary>
    public interface IDataGridBoundColumn
    {
        /// <summary>
        /// Gets or sets the binding associated with the data grid column.
        /// </summary>
        /// <value>A <see cref="BindingBase"/> object.</value>
        BindingBase Binding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the binding associated with the data grid column for the clipboard.
        /// </summary>
        /// <value>A <see cref="BindingBase"/> object.</value>
        BindingBase ClipboardContentBinding
        {
            get;
            set;
        }
    }
}
