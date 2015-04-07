namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    [ContractClassFor( typeof( IDataGridColumnBuilder ) )]
    internal abstract class IDataGridColumnBuilderContract : IDataGridColumnBuilder
    {
        Type IDataGridColumnBuilder.TargetType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null );
                return default( Type );
            }
        }

        string IDataGridColumnBuilder.AdapterBindingPath
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return default( string );
            }
        }

        Task IDataGridColumnBuilder.BuildupAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<InvalidOperationException>( !columns.IsReadOnly, "columns" );
            Contract.Requires<ArgumentOutOfRangeException>( buildOrders != ColumnBuildOrders.None, "buildOrders" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }

        Task IDataGridColumnBuilder.AppendToAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<InvalidOperationException>( !columns.IsReadOnly, "columns" );
            Contract.Requires<ArgumentOutOfRangeException>( buildOrders != ColumnBuildOrders.None, "buildOrders" );
            Contract.Ensures( Contract.Result<Task>() != null );
            return null;
        }
    }
}
