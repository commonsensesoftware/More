namespace More.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a mediator that can coordinate columns between a
    /// <see cref="DataGrid">data grid</see> and a <see cref="IList">list</see>.
    /// </summary>
    sealed class ColumnsMediator : IDisposable
    {
        volatile bool disposed;
        volatile bool suppressCollectionChanged;
        volatile bool suppressColumnsChanged;
        DataGrid dataGrid;
        ICollection<DataGridColumn> columns;
        INotifyCollectionChanged columnEvents;
        internal readonly long Key;

        internal ColumnsMediator( DataGrid dataGrid, IEnumerable<DataGridColumn> sequence, INotifyCollectionChanged columnEvents )
        {
            Contract.Requires( dataGrid != null );
            Contract.Requires( sequence != null );
            Contract.Requires( columnEvents != null );

            Key = CreateKey( dataGrid, sequence );
            columns = sequence as ICollection<DataGridColumn>;
            this.columnEvents = columnEvents;
            this.columnEvents.CollectionChanged += OnCollectionChanged;
            this.dataGrid = dataGrid;

            // if the provided sequence isn't a collection, then mediation is one-way.
            // the sequence can notify the data grid, but not the other way around.
            if ( columns != null )
            {
                this.dataGrid.Columns.CollectionChanged += OnColumnsChanged;
            }
        }

        bool IsSuppressingEvents => suppressColumnsChanged || suppressCollectionChanged;

        void Dispose( bool disposing )
        {
            if ( disposed )
            {
                return;
            }

            disposed = true;

            if ( !disposing )
            {
                return;
            }

            if ( dataGrid != null )
            {
                dataGrid.Columns.CollectionChanged -= OnColumnsChanged;
                dataGrid = null;
            }

            if ( columnEvents != null )
            {
                columnEvents.CollectionChanged -= OnCollectionChanged;
                columnEvents = null;
            }

            columns = null;
        }

        internal static long CreateKey( DataGrid dataGrid, object items )
        {
            Contract.Requires( dataGrid != null );
            var hash = ( dataGrid.GetHashCode() << 32 ) | ( items == null ? 0 : items.GetHashCode() );
            return hash;
        }

        static void SynchronizeCollection( ICollection<DataGridColumn> columns, NotifyCollectionChangedEventArgs e, bool fastClear )
        {
            Contract.Requires( columns != null );
            Contract.Requires( e != null );

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                if ( fastClear )
                {
                    columns.Clear();
                }
                else
                {
                    // HACK: yet another blunder in the DataGrid (at least for Silverlight).  I'm making an assuming this will hold true in WPF
                    // as well.  Calling Clear() on the DataGrid.Columns property does not correctly calculate it's internal indexes.  An
                    // ArgumentOutOfRangeException is thrown from an internal List<T>, but it's difficult to say what the exact problem is.
                    // Through trial and error, removing columns one at a time proved to produce reliable results; therefore, this branch of
                    // the code uses Remove() or RemoveAt() instead of Clear().

                    if ( columns is IList<DataGridColumn> list )
                    {
                        for ( var i = list.Count - 1; i > -1; i-- )
                        {
                            list.RemoveAt( i );
                        }
                    }
                    else
                    {
                        list = new List<DataGridColumn>( columns );

                        for ( var i = list.Count - 1; i > -1; i-- )
                        {
                            columns.Remove( list[i] );
                        }
                    }
                }

                return;
            }

            if ( e.OldItems != null )
            {
                foreach ( DataGridColumn column in e.OldItems )
                {
                    columns.Remove( column );
                }
            }

            if ( e.NewItems != null )
            {
                foreach ( DataGridColumn column in e.NewItems )
                {
                    if ( !columns.Contains( column ) )
                    {
                        columns.Add( column );
                    }
                }
            }
        }

        void OnColumnsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if ( IsSuppressingEvents )
            {
                return;
            }

            suppressCollectionChanged = true;
            SynchronizeCollection( columns, e, true );
            suppressCollectionChanged = false;
        }

        void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if ( IsSuppressingEvents )
            {
                return;
            }

            suppressColumnsChanged = true;
            SynchronizeCollection( dataGrid.Columns, e, false );
            suppressColumnsChanged = false;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}