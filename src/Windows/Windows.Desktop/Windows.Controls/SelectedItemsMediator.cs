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
    internal sealed class SelectedItemsMediator : IDisposable
    {
        private volatile bool disposed;
        private volatile bool suppressCollectionChanged;
        private volatile bool suppressSelectionChanged;
        private DataGrid dataGrid;
        private IList items;
        private INotifyCollectionChanged itemEvents;
        internal readonly long Key;

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

        private bool IsSuppressingEvents
        {
            get
            {
                return suppressSelectionChanged || suppressCollectionChanged;
            }
        }

        private void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( !disposing )
                return;

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

        private void OnSelectedItemsChanged( object sender, SelectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // exit if events are suppressed
            if ( IsSuppressingEvents )
                return;

            // suppress collection changed events
            suppressCollectionChanged = true;

            // remove old items
            if ( e.RemovedItems != null )
            {
                foreach ( object item in e.RemovedItems )
                    items.Remove( item );
            }

            // add new items
            if ( e.AddedItems != null )
            {
                foreach ( object item in e.AddedItems )
                    if ( !items.Contains( item ) )
                        items.Add( item );
            }

            // enable events
            suppressCollectionChanged = false;
        }

        private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // exit if events are suppressed
            if ( IsSuppressingEvents )
                return;

            suppressSelectionChanged = true;

            // HACK: the DataGrid will throw an exception if selected items are cleared while in single selection mode.
            // The user can actually unselect a row in the UI by pressing CTRL+Click on a selected row, even while in
            // single selection mode.  To get around this problem, temporarily switch to multiselect mode, clear the
            // items, and then revert back to single selection mode.

            var singleSelect = dataGrid.SelectionMode == DataGridSelectionMode.Single;
            dataGrid.SelectionMode = DataGridSelectionMode.Extended;

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                // clear selected items
                dataGrid.SelectedItems.Clear();
            }
            else
            {
                // remove old items
                if ( e.OldItems != null )
                {
                    foreach ( object item in e.OldItems )
                        dataGrid.SelectedItems.Remove( item );
                }

                // add new items
                if ( e.NewItems != null && e.NewItems.Count > 0 )
                {
                    if ( singleSelect )
                    {
                        // add or replace current item
                        if ( dataGrid.SelectedItems.Count == 0 )
                            dataGrid.SelectedItems.Add( e.NewItems[0] );
                        else
                            dataGrid.SelectedItems[0] = e.NewItems[0];
                    }
                    else
                    {
                        // add all new items not already selected
                        foreach ( object item in e.NewItems )
                        {
                            if ( !dataGrid.SelectedItems.Contains( item ) )
                                dataGrid.SelectedItems.Add( item );
                        }
                    }
                }
            }

            // revert selection mode
            if ( singleSelect )
                dataGrid.SelectionMode = DataGridSelectionMode.Single;

            // enable events
            suppressSelectionChanged = false;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
