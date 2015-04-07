namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    /// <summary>
    /// Provides extension methods for the <see cref="IDataGridColumnBuilder"/> interface.
    /// </summary>
    public static class IDataGridColumnBuilderExtensions
    {
        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task BuildupAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.BuildupAsync( columns, ColumnBuildOrders.Default, CancellationToken.None );
        }

        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task BuildupAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.BuildupAsync( columns, buildOrders, CancellationToken.None );
        }

        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task BuildupAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.BuildupAsync( columns, ColumnBuildOrders.Default, cancellationToken );
        }

        /// <summary>
        /// Appends the columns resolved by the builder into the specified collection asynchronously.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task AppendToAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.AppendToAsync( columns, ColumnBuildOrders.Default, CancellationToken.None );
        }

        /// <summary>
        /// Appends the columns resolved by the builder into the specified collection asynchronously.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task AppendToAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.AppendToAsync( columns, buildOrders, CancellationToken.None );
        }

        /// <summary>
        /// Appends the columns resolved by the builder into the specified collection.
        /// </summary>
        /// <param name="builder">The extended <see cref="IDataGridColumnBuilder">builder</see>.</param>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task AppendToAsync( this IDataGridColumnBuilder builder, ICollection<DataGridColumn> columns, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( builder != null, "builder" );
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return builder.AppendToAsync( columns, ColumnBuildOrders.Default, cancellationToken );
        }
    }
}
