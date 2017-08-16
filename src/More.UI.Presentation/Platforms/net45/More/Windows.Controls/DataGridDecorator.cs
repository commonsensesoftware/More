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

        static readonly ConcurrentDictionary<long, ColumnsMediator> columnMediators = new ConcurrentDictionary<long, ColumnsMediator>();
        static readonly ConcurrentDictionary<long, SelectedItemsMediator> selectedItemsMediators = new ConcurrentDictionary<long, SelectedItemsMediator>();

        /// <summary>
        /// Gets the columns attached to the specified <see cref="DataGrid"/>.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGrid"/> to get the columns for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This only works for a DataGrid object." )]
        public static IEnumerable<DataGridColumn> GetColumns( DataGrid dataGrid )
        {
            Arg.NotNull( dataGrid, nameof( dataGrid ) );
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
            Arg.NotNull( dataGrid, nameof( dataGrid ) );
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
            Arg.NotNull( dataGrid, nameof( dataGrid ) );
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
            Arg.NotNull( dataGrid, nameof( dataGrid ) );
            dataGrid.SetValue( SelectedItemsProperty, selectedItems );
        }

        static void OnDataGridLoaded( object sender, RoutedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            var dataGrid = (DataGrid) sender;

            dataGrid.Loaded -= OnDataGridLoaded;

            var columns = GetColumns( dataGrid );

            if ( columns == null )
            {
                return;
            }

            foreach ( var column in columns )
            {
                if ( !dataGrid.Columns.Contains( column ) )
                {
                    dataGrid.Columns.Add( column );
                }
            }
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The mediator should not be disposd until the association is broken." )]
        static void OnColumnsPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );

            var dataGrid = (DataGrid) sender;
            var key = ColumnsMediator.CreateKey( dataGrid, e.OldValue );

            dataGrid.Loaded -= OnDataGridLoaded;

            if ( columnMediators.TryRemove( key, out var mediator ) )
            {
                mediator?.Dispose();
            }

            if ( e.OldValue is IEnumerable<DataGridColumn> columns )
            {
                foreach ( var column in columns )
                {
                    if ( dataGrid.Columns.Contains( column ) )
                    {
                        dataGrid.Columns.Remove( column );
                    }
                }
            }

            columns = e.NewValue as IEnumerable<DataGridColumn>;

            if ( columns != null )
            {
                foreach ( var column in columns )
                {
                    if ( !dataGrid.Columns.Contains( column ) )
                    {
                        dataGrid.Columns.Add( column );
                    }
                }
            }

            dataGrid.Loaded += OnDataGridLoaded;

            if ( columns is INotifyCollectionChanged columnEvents )
            {
                mediator = new ColumnsMediator( dataGrid, columns, columnEvents );
                columnMediators.TryAdd( mediator.Key, mediator );
            }
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The mediator should not be disposd until the association is broken." )]
        static void OnSelectedItemsPropertyChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );

            var dataGrid = (DataGrid) sender;
            var key = SelectedItemsMediator.CreateKey( dataGrid, e.OldValue );

            if ( selectedItemsMediators.TryRemove( key, out var mediator ) )
            {
                mediator?.Dispose();
            }

            if ( !( e.NewValue is IList items ) )
            {
                return;
            }

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
                    foreach ( var item in items )
                    {
                        dataGrid.SelectedItems.Add( item );
                    }
                }

                if ( singleSelect )
                {
                    dataGrid.SelectionMode = DataGridSelectionMode.Single;
                }
            }

            if ( items is INotifyCollectionChanged itemEvents )
            {
                mediator = new SelectedItemsMediator( dataGrid, items, itemEvents );
                selectedItemsMediators.TryAdd( mediator.Key, mediator );
            }
        }
    }
}