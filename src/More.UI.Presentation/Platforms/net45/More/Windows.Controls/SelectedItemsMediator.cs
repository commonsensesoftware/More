namespace More.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a mediator that can coordinate selected items between a
    /// <see cref="DataGrid">data grid</see> and a <see cref="IList">list</see>.
    /// </summary>
    sealed class SelectedItemsMediator : IDisposable
    {
        volatile bool disposed;
        volatile bool suppressCollectionChanged;
        volatile bool suppressSelectionChanged;
        DataGrid dataGrid;
        IList items;
        INotifyCollectionChanged itemEvents;
#pragma warning disable SA1401 // Fields should be private
        internal readonly long Key;
#pragma warning restore SA1401 // Fields should be private

        internal SelectedItemsMediator( DataGrid dataGrid, IList items, INotifyCollectionChanged itemEvents )
        {
            Contract.Requires( dataGrid != null );
            Contract.Requires( items != null );
            Contract.Requires( itemEvents != null );

            Key = CreateKey( dataGrid, items );
            this.dataGrid = dataGrid;
            this.dataGrid.SelectionChanged += OnSelectedItemsChanged;
            this.items = items;
            this.itemEvents = itemEvents;
            this.itemEvents.CollectionChanged += OnCollectionChanged;
        }

        bool IsSuppressingEvents => suppressSelectionChanged || suppressCollectionChanged;

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
                dataGrid.SelectionChanged -= OnSelectedItemsChanged;
                dataGrid = null;
            }

            if ( itemEvents != null )
            {
                itemEvents.CollectionChanged -= OnCollectionChanged;
                itemEvents = null;
            }

            items = null;
        }

        internal static long CreateKey( DataGrid dataGrid, object items )
        {
            Contract.Requires( dataGrid != null );
            var hash = ( dataGrid.GetHashCode() << 32 ) | ( items == null ? 0 : items.GetHashCode() );
            return hash;
        }

        void OnSelectedItemsChanged( object sender, SelectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( IsSuppressingEvents )
            {
                return;
            }

            suppressCollectionChanged = true;

            if ( e.RemovedItems != null )
            {
                foreach ( var item in e.RemovedItems )
                {
                    items.Remove( item );
                }
            }

            if ( e.AddedItems != null )
            {
                foreach ( var item in e.AddedItems )
                {
                    if ( !items.Contains( item ) )
                    {
                        items.Add( item );
                    }
                }
            }

            suppressCollectionChanged = false;
        }

        void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            if ( IsSuppressingEvents )
            {
                return;
            }

            suppressSelectionChanged = true;

            // HACK: the DataGrid will throw an exception if selected items are cleared while in single selection mode.
            // The user can actually unselect a row in the UI by pressing CTRL+Click on a selected row, even while in
            // single selection mode.  To get around this problem, temporarily switch to multiselect mode, clear the
            // items, and then revert back to single selection mode.
            var singleSelect = dataGrid.SelectionMode == DataGridSelectionMode.Single;

            dataGrid.SelectionMode = DataGridSelectionMode.Extended;

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                dataGrid.SelectedItems.Clear();
            }
            else
            {
                if ( e.OldItems != null )
                {
                    foreach ( var item in e.OldItems )
                    {
                        dataGrid.SelectedItems.Remove( item );
                    }
                }

                if ( e.NewItems != null && e.NewItems.Count > 0 )
                {
                    if ( singleSelect )
                    {
                        if ( dataGrid.SelectedItems.Count == 0 )
                        {
                            dataGrid.SelectedItems.Add( e.NewItems[0] );
                        }
                        else
                        {
                            dataGrid.SelectedItems[0] = e.NewItems[0];
                        }
                    }
                    else
                    {
                        foreach ( var item in e.NewItems )
                        {
                            if ( !dataGrid.SelectedItems.Contains( item ) )
                            {
                                dataGrid.SelectedItems.Add( item );
                            }
                        }
                    }
                }
            }

            if ( singleSelect )
            {
                dataGrid.SelectionMode = DataGridSelectionMode.Single;
            }

            suppressSelectionChanged = false;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}