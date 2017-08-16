namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the behavior of a data grid column builder.
    /// </summary>
    [ContractClass( typeof( IDataGridColumnBuilderContract ) )]
    public interface IDataGridColumnBuilder
    {
        /// <summary>
        /// Gets the target type the builder is for.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets the adapter binding path for the specified target type.
        /// </summary>
        /// <value>The adapter binding path for the target type. The default value is an empty string.</value>
        /// <remarks>This property is only used when the target type is adapted by another type and the binding path
        /// of the adapter must be specified for data binding.</remarks>
        string AdapterBindingPath { get; }

        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task BuildupAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken );

        /// <summary>
        /// Appends the columns resolved by the builder into the specified collection asynchronously.
        /// </summary>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task AppendToAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken );
    }
}