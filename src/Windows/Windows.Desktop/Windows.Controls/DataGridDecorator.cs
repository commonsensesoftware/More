namespace More.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a data grid columns decorator.
    /// </summary>
    /// <example>The following example demonstrates who to data bind columns and selected items to a <see cref="DataGrid">data grid</see>.
    /// <code lang="Xaml"><![CDATA[
    /// <UserControl
    ///  x:Class="MyApp.MyControl"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:Data="clr-namespace:System.Windows.Controls;assembly=PresentationFramework"
    ///  xmlns:More="clr-namespace:System.Windows.Controls;assembly=System.Windows.More">
    /// 
    /// <Grid>
    ///   <Data:DataGrid ItemsSource="{Binding ItemsSource}"
    ///                  AutoGenerateColumns="False"
    ///                  More:DataGridDecorator.Columns="{Binding Columns}"
    ///                  More:DataGridDecorator.SelectedItems="{Binding SelectedItems}" />
    /// </Grid>
    ///  
    /// </UserControl>
    /// ]]></code></example>
    public static class DataGridDecorator
    {
        /// <summary>
        /// Gets the columns dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached( "Columns", typeof( IEnumerable<DataGridColumn> ), typeof( DataGridDecorator ), new PropertyMetadata( OnColumnsPropertyChanged ) );

        /// <summary>
        /// Gets the selected items dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached( "SelectedItems", typeof( IList ), typeof( DataGridDecorator ), new PropertyMetadata( OnSelectedItemsPropertyChanged ) );

        private static readonly ConcurrentDictionary<long, ColumnsMediator> columnMediators = new ConcurrentDictionary<long, ColumnsMediator>();
        private static readonly ConcurrentDictionary<long, SelectedItemsMediator> selectedItemsMediators = new ConcurrentDictionary<long, SelectedItemsMediator>();

        /// <summary>
        /// Gets the columns attached to the specified <see cref="DataGrid"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> to get the columns for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This only works for a DataGrid object." )]
        public static IEnumerable<DataGridColumn> GetColumns( DataGrid dataGrid )
        {
            Contract.Requires<ArgumentNullException>( dataGrid != null, "dataGrid" );
            return (IEnumerable<DataGridColumn>) dataGrid.GetValue( ColumnsProperty );
        }

        /// <summary>
        /// Sets the columns attached to the specified <see cref="DataGrid"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> to set the columns for.</param>
        /// <param name="columns">An <see cref="IEnumerable{T}"/> object.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This only works for a DataGrid object." )]
        public static void SetColumns( DataGrid dataGrid, IEnumerable<DataGridColumn> columns )
        {
            Contract.Requires<ArgumentNullException>( dataGrid != null, "dataGrid" );
            dataGrid.SetValue( ColumnsProperty, columns );
        }

        /// <summary>
        /// Gets the selected items attached to the specified <see cref="DataGrid"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> to get the columns for.</param>
        /// <returns>An <see cref="IList"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This only works for a DataGrid object." )]
        public static IList GetSelectedItems( DataGrid dataGrid )
        {
            Contract.Requires<ArgumentNullException>( dataGrid != null, "dataGrid" );
            return (IList) dataGrid.GetValue( SelectedItemsProperty );
        }

        /// <summary>
        /// Sets the selected items attached to the specified <see cref="DataGrid"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> to set the columns for.</param>
        /// <param name="selectedItems">An <see cref="IList"/> object.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This only works for a DataGrid object." )]
        public static void SetSelectedItems( DataGrid dataGrid, IList selectedItems )
        {
            Contract.Requires<ArgumentNullException>( dataGrid != null, "dataGrid" );
            dataGrid.SetValue( SelectedItemsProperty, selectedItems );
        }

        private static void OnDataGridLoaded( object sender, RoutedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            var dataGrid = sender as DataGrid;

            // remove handler (needed only once)
            dataGrid.Loaded -= OnDataGridLoaded;

            var columns = GetColumns( dataGrid );

            // no attached columns
            if ( columns == null )
                return;

            // add initial columns
            foreach ( var column in columns )
                if ( !dataGrid.Columns.Contains( column ) )
                    dataGrid.Columns.Add( column );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The mediator should not be disposd until the association is broken." )]
        private static void OnColumnsPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );

            var dataGrid = (DataGrid) sender;
            var key = ColumnsMediator.CreateKey( dataGrid, e.OldValue );
            ColumnsMediator mediator = null;

            // remove event handler
            dataGrid.Loaded -= OnDataGridLoaded;

            // dispose of existing mediator
            if ( columnMediators.TryRemove( key, out mediator ) )
            {
                if ( mediator != null )
                    mediator.Dispose();
            }

            var columns = e.OldValue as IEnumerable<DataGridColumn>;

            // remove old columns
            if ( columns != null )
            {
                foreach ( var column in columns )
                    if ( dataGrid.Columns.Contains( column ) )
                        dataGrid.Columns.Remove( column );
            }

            columns = e.NewValue as IEnumerable<DataGridColumn>;

            // add new columns
            if ( columns != null )
            {
                foreach ( var column in columns )
                    if ( !dataGrid.Columns.Contains( column ) )
                        dataGrid.Columns.Add( column );
            }

            // rewire event handler
            dataGrid.Loaded += OnDataGridLoaded;

            var columnEvents = columns as INotifyCollectionChanged;

            // change notification isn't supported so there's nothing to mediate
            if ( columnEvents == null )
                return;

            mediator = new ColumnsMediator( dataGrid, columns, columnEvents );
            columnMediators.TryAdd( mediator.Key, mediator );
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The mediator should not be disposd until the association is broken." )]
        private static void OnSelectedItemsPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null, "sender" );

            var dataGrid = (DataGrid) sender;
            var key = SelectedItemsMediator.CreateKey( dataGrid, e.OldValue );
            SelectedItemsMediator mediator = null;

            // dispose of old mediator
            if ( selectedItemsMediators.TryRemove( key, out mediator ) )
            {
                if ( mediator != null )
                    mediator.Dispose();
            }

            var items = e.NewValue as IList;

            // no further processing required
            if ( items == null )
                return;

            // add existing items
            if ( items.Count > 0 )
            {
                var singleSelect = dataGrid.SelectionMode == DataGridSelectionMode.Single;

                // clear current items (exception is thrown when clearing in single selection mode)
                dataGrid.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid.SelectedItems.Clear();

                if ( singleSelect )
                {
                    dataGrid.SelectedItems.Add( items[0] );
                }
                else
                {
                    foreach ( object item in items )
                        dataGrid.SelectedItems.Add( item );
                }

                // revert setting as necessary
                if ( singleSelect )
                    dataGrid.SelectionMode = DataGridSelectionMode.Single;
            }

            var itemEvents = items as INotifyCollectionChanged;

            // change notification isn't supported so there's nothing to mediate
            if ( itemEvents == null )
                return;

            // mediate events
            mediator = new SelectedItemsMediator( dataGrid, items, itemEvents );
            selectedItemsMediators.TryAdd( mediator.Key, mediator );
        }
    }
}
