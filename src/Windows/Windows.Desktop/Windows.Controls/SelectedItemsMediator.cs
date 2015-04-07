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

            this.Key = CreateKey( dataGrid, items );
            this.dataGrid = dataGrid;
            this.dataGrid.SelectionChanged += this.OnSelectedItemsChanged;
            this.items = items;
            this.itemEvents = itemEvents;
            this.itemEvents.CollectionChanged += this.OnCollectionChanged;
        }

        private bool IsSuppressingEvents
        {
            get
            {
                return this.suppressSelectionChanged || this.suppressCollectionChanged;
            }
        }

        private void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            if ( this.dataGrid != null )
            {
                this.dataGrid.SelectionChanged -= this.OnSelectedItemsChanged;
                this.dataGrid = null;
            }

            if ( this.itemEvents != null )
            {
                this.itemEvents.CollectionChanged -= this.OnCollectionChanged;
                this.itemEvents = null;
            }

            this.items = null;
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
            if ( this.IsSuppressingEvents )
                return;

            // suppress collection changed events
            this.suppressCollectionChanged = true;

            // remove old items
            if ( e.RemovedItems != null )
            {
                foreach ( object item in e.RemovedItems )
                    this.items.Remove( item );
            }

            // add new items
            if ( e.AddedItems != null )
            {
                foreach ( object item in e.AddedItems )
                    if ( !this.items.Contains( item ) )
                        this.items.Add( item );
            }

            // enable events
            this.suppressCollectionChanged = false;
        }

        private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // exit if events are suppressed
            if ( this.IsSuppressingEvents )
                return;

            this.suppressSelectionChanged = true;

            // HACK: the DataGrid will throw an exception if selected items are cleared while in single selection mode.
            // The user can actually unselect a row in the UI by pressing CTRL+Click on a selected row, even while in
            // single selection mode.  To get around this problem, temporarily switch to multiselect mode, clear the
            // items, and then revert back to single selection mode.

            var singleSelect = this.dataGrid.SelectionMode == DataGridSelectionMode.Single;
            this.dataGrid.SelectionMode = DataGridSelectionMode.Extended;

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                // clear selected items
                this.dataGrid.SelectedItems.Clear();
            }
            else
            {
                // remove old items
                if ( e.OldItems != null )
                {
                    foreach ( object item in e.OldItems )
                        this.dataGrid.SelectedItems.Remove( item );
                }

                // add new items
                if ( e.NewItems != null && e.NewItems.Count > 0 )
                {
                    if ( singleSelect )
                    {
                        // add or replace current item
                        if ( this.dataGrid.SelectedItems.Count == 0 )
                            this.dataGrid.SelectedItems.Add( e.NewItems[0] );
                        else
                            this.dataGrid.SelectedItems[0] = e.NewItems[0];
                    }
                    else
                    {
                        // add all new items not already selected
                        foreach ( object item in e.NewItems )
                        {
                            if ( !this.dataGrid.SelectedItems.Contains( item ) )
                                this.dataGrid.SelectedItems.Add( item );
                        }
                    }
                }
            }

            // revert selection mode
            if ( singleSelect )
                this.dataGrid.SelectionMode = DataGridSelectionMode.Single;

            // enable events
            this.suppressSelectionChanged = false;
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
