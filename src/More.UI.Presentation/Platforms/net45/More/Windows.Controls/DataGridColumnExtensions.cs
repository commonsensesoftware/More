namespace More.Windows.Controls
{
    using Media;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using static System.String;

    /// <summary>
    /// Provides extension methods for the <see cref="DataGridColumn"/> class.
    /// </summary>
    public static class DataGridColumnExtensions
    {
        static async Task<DataGridColumn> AddFromResourceAsync(
            Assembly assembly,
            ICollection<DataGridColumn> columns,
            string resourceName,
            string templateName,
            string editTemplateName,
            string header )
        {
            Contract.Requires( assembly != null );
            Contract.Requires( columns != null );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var column = new OptionalDataGridTemplateColumn();

            column.IsReadOnly = IsNullOrEmpty( editTemplateName );
            column.IsOptional = true;
            column.Header = header;
            columns.Add( column );

            if ( IsNullOrEmpty( templateName ) && IsNullOrEmpty( editTemplateName ) )
            {
                return column;
            }

            if ( IsNullOrEmpty( resourceName ) )
            {
                var xaml = new XamlContent<DataTemplate>();

                if ( !IsNullOrEmpty( templateName ) )
                {
                    column.CellTemplate = await xaml.FromResourceAsync( assembly.CreatePackUri( templateName ) ).ConfigureAwait( true );
                }

                if ( !column.IsReadOnly && !IsNullOrEmpty( editTemplateName ) )
                {
                    column.CellEditingTemplate = await xaml.FromResourceAsync( assembly.CreatePackUri( editTemplateName ) ).ConfigureAwait( true );
                }
            }
            else
            {
                var xaml = new XamlContent<ResourceDictionary>();
                var resourceDictionary = await xaml.FromResourceAsync( assembly.CreatePackUri( resourceName ) ).ConfigureAwait( true );

                if ( !IsNullOrEmpty( templateName ) )
                {
                    column.CellTemplate = (DataTemplate) resourceDictionary[templateName];
                }

                if ( !column.IsReadOnly && !IsNullOrEmpty( editTemplateName ) )
                {
                    column.CellEditingTemplate = (DataTemplate) resourceDictionary[editTemplateName];
                }
            }

            return column;
        }

        /// <summary>
        /// Adds a new column to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="propertyName">The name of the data bound property.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>The header for the column will be one or more words conforming the Pascal-case naming guidelines
        /// for the specified property name.</remarks>
        public static DataGridColumn Add( this ICollection<DataGridColumn> columns, string propertyName )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            return Add( columns, propertyName, Util.HeaderTextFromPropertyName( propertyName ) );
        }

        /// <summary>
        /// Adds a new column to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="propertyName">The name of the data bound property.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        public static DataGridColumn Add( this ICollection<DataGridColumn> columns, string propertyName, string header )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var column = new OptionalDataGridTextColumn() { IsOptional = true };
            return Add( columns, column, propertyName, header );
        }

        /// <summary>
        /// Adds a new column to the specified collection.
        /// </summary>
        /// <typeparam name="TColumn">The <see cref="Type"/> of column to add.</typeparam>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="column">The <see cref="DataGridBoundColumn"/> column of type <typeparamref name="TColumn"/> to add.</param>
        /// <param name="propertyName">The name of the data bound property.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        public static DataGridColumn Add<TColumn>( this ICollection<DataGridColumn> columns, TColumn column, string propertyName, string header ) where TColumn : DataGridBoundColumn
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNull( column, nameof( column ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var binding = new Binding( propertyName ) { Mode = BindingMode.OneWay };

            column.IsReadOnly = true;
            column.Header = header;
            column.Binding = binding;
            columns.Add( column );

            return column;
        }

        /// <summary>
        /// Adds a new column using a template to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="templateName">The name of the standard template resource.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the added <see cref="DataGridColumn"/> object.</returns>
        public static Task<DataGridColumn> AddUsingTemplateAsync( this ICollection<DataGridColumn> columns, string templateName, string header )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( templateName, nameof( templateName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            // NOTE: the resource must be resolved from the calling assembly
            var assembly = Assembly.GetCallingAssembly();
            return AddFromResourceAsync( assembly, columns, null, templateName, null, header );
        }

        /// <summary>
        /// Adds a new column using a template to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="templateName">The name of the standard template resource.</param>
        /// <param name="editTemplateName">The name of the editing template resource.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the added <see cref="DataGridColumn"/> object.</returns>
        public static Task<DataGridColumn> AddUsingTemplateAsync( this ICollection<DataGridColumn> columns, string templateName, string editTemplateName, string header )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( templateName, nameof( templateName ) );
            Arg.NotNullOrEmpty( editTemplateName, nameof( editTemplateName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            // NOTE: the resource must be resolved from the calling assembly
            var assembly = Assembly.GetCallingAssembly();
            return AddFromResourceAsync( assembly, columns, null, templateName, editTemplateName, header );
        }

        /// <summary>
        /// Adds a new column using a template from the specified resource to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="resourceName">The name of the resource dictionary containing the templates.</param>
        /// <param name="templateName">The name of the standard template resource.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the added <see cref="DataGridColumn"/> object.</returns>
        public static Task<DataGridColumn> AddFromResourceAsync( this ICollection<DataGridColumn> columns, string resourceName, string templateName, string header )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Arg.NotNullOrEmpty( templateName, nameof( templateName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            // NOTE: the resource must be resolved from the calling assembly
            var assembly = Assembly.GetCallingAssembly();
            return AddFromResourceAsync( assembly, columns, resourceName, templateName, null, header );
        }

        /// <summary>
        /// Adds a new column using a template from the specified resource to the specified collection.
        /// </summary>
        /// <param name="columns">The extended <see cref="ICollection{T}"/> object.</param>
        /// <param name="resourceName">The name of the resource dictionary containing the templates.</param>
        /// <param name="templateName">The name of the standard template resource.</param>
        /// <param name="editTemplateName">The name of the editing template resource.</param>
        /// <param name="header">The column header.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the added <see cref="DataGridColumn"/> object.</returns>
        public static Task<DataGridColumn> AddFromResourceAsync( this ICollection<DataGridColumn> columns, string resourceName, string templateName, string editTemplateName, string header )
        {
            Arg.NotNull( columns, nameof( columns ) );
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Arg.NotNullOrEmpty( templateName, nameof( templateName ) );
            Arg.NotNullOrEmpty( editTemplateName, nameof( editTemplateName ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            // NOTE: the resource must be resolved from the calling assembly
            var assembly = Assembly.GetCallingAssembly();
            return AddFromResourceAsync( assembly, columns, resourceName, templateName, editTemplateName, header );
        }

        static IDataGridBoundColumn GetBoundColumn( this DataGridColumn column )
        {
            Contract.Requires( column != null );

            if ( column is IDataGridBoundColumn bindableColumn )
            {
                return bindableColumn;
            }

            if ( column is DataGridBoundColumn boundColumn )
            {
                bindableColumn = new DataGridBoundColumnAdapter( boundColumn );
            }

            return null;
        }

        static void SetBindingMode( this IDataGridBoundColumn column, BindingMode mode )
        {
            Contract.Requires( column != null );

            if ( !( column.Binding is Binding binding ) )
            {
                return;
            }

            try
            {
                binding.Mode = mode;
            }
            catch ( InvalidOperationException )
            {
                // binding cannot be changed once it's connected to the data source
                var newBinding = BindingExtensions.Clone( binding );
                newBinding.Mode = mode;
                column.Binding = newBinding;
            }
        }

        static void SetBindingConverter( this IDataGridBoundColumn column, IValueConverter converter, object parameter )
        {
            Contract.Requires( column != null );

            if ( !( column.Binding is Binding binding ) )
            {
                return;
            }

            try
            {
                binding.Converter = converter;
                binding.ConverterParameter = parameter;
            }
            catch ( InvalidOperationException )
            {
                // binding cannot be changed once it's connected to the data source
                var newBinding = BindingExtensions.Clone( binding );
                newBinding.Converter = converter;
                newBinding.ConverterParameter = parameter;
                column.Binding = newBinding;
            }
        }

        static void CloneBoundColumn( DataGridBoundColumn clone, DataGridBoundColumn column )
        {
            Contract.Requires( clone != null );
            Contract.Requires( column != null );

            if ( column.Binding is Binding binding )
            {
                clone.Binding = BindingExtensions.Clone( binding );
            }

            clone.EditingElementStyle = column.EditingElementStyle;
            clone.ElementStyle = column.ElementStyle;
        }

        static void CloneTextColumn( DataGridTextColumn clone, DataGridTextColumn column )
        {
            Contract.Requires( clone != null );
            Contract.Requires( column != null );

            clone.FontFamily = column.FontFamily;
            clone.FontSize = column.FontSize;
            clone.FontStyle = column.FontStyle;
            clone.FontWeight = column.FontWeight;
            clone.Foreground = column.Foreground;
        }

        static void CloneTemplateColumn( DataGridTemplateColumn clone, DataGridTemplateColumn column )
        {
            Contract.Requires( clone != null );
            Contract.Requires( column != null );

            clone.CellEditingTemplate = column.CellEditingTemplate;
            clone.CellTemplate = column.CellTemplate;
        }

        /// <summary>
        /// Gets the binding associated with a column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="Binding"/> object or null if the column doesn't support data binding.</returns>
        public static Binding GetBinding( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );

            if ( column is DataGridBoundColumn boundColumn )
            {
                return boundColumn.Binding as Binding;
            }

            if ( column is IDataGridBoundColumn bindableColumn )
            {
                return bindableColumn.Binding as Binding;
            }

            return null;
        }

        /// <summary>
        /// Gets the clipboard content binding associated with a column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="Binding"/> object or null if the column doesn't support data binding.</returns>
        public static Binding GetClipboardContentBinding( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );

            if ( column is DataGridBoundColumn boundColumn )
            {
                return boundColumn.ClipboardContentBinding as Binding;
            }

            if ( column is IDataGridBoundColumn bindableColumn )
            {
                return bindableColumn.ClipboardContentBinding as Binding;
            }

            return null;
        }

        /// <summary>
        /// Creates a deep copy of a column.
        /// </summary>
        /// <typeparam name="TColumn">The <see cref="Type"/> of <see cref="DataGridColumn"/> to clone.</typeparam>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A column of type <typeparamref name="TColumn"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static TColumn Clone<TColumn>( this TColumn column ) where TColumn : DataGridColumn
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<TColumn>() != null );

            var columnType = column.GetType();

            // NOTE: since DataGridColumn is abstract, we manually check for a parameterless constructor rather than using the new() type constraint
            // which supports scenarios such as cloning all the columns in a collection (which may be of different types). For example, we want
            // to call Clone<T> on a collection of MyColumnBase which is abstract, but we are cloning mixed, concrete types such as MyColumn1 and
            // MyColumn2.  The new() constraint cannot be applied to MyColumnBase and a collection of mixed column types cannot be cloned without
            // using MyColumnBase as the type constraint.
            if ( columnType.IsAbstract || !columnType.GetConstructors().Any( c => c.GetParameters().Length == 0 ) )
            {
                throw new InvalidOperationException( ExceptionMessage.NoParameterlessConstructor.FormatDefault( columnType ) );
            }

            var clone = (TColumn) Activator.CreateInstance( columnType );

            if ( column.ClipboardContentBinding is Binding binding )
            {
                clone.ClipboardContentBinding = BindingExtensions.Clone( binding );
            }

            clone.CanUserReorder = column.CanUserReorder;
            clone.CanUserResize = column.CanUserResize;
            clone.CanUserSort = column.CanUserSort;
            clone.CellStyle = column.CellStyle;
            clone.DisplayIndex = column.DisplayIndex;
            clone.DragIndicatorStyle = column.DragIndicatorStyle;
            clone.Header = column.Header;
            clone.HeaderStyle = column.HeaderStyle;
            clone.IsReadOnly = column.IsReadOnly;
            clone.MaxWidth = column.MaxWidth;
            clone.MinWidth = column.MinWidth;
            clone.SortMemberPath = column.SortMemberPath;
            clone.Visibility = column.Visibility;
            clone.Width = column.Width;
            clone.HeaderStringFormat = column.HeaderStringFormat;
            clone.HeaderTemplate = column.HeaderTemplate;
            clone.HeaderTemplateSelector = column.HeaderTemplateSelector;
            clone.SortDirection = column.SortDirection;

            if ( typeof( DataGridBoundColumn ).IsAssignableFrom( typeof( TColumn ) ) )
            {
                CloneBoundColumn( clone as DataGridBoundColumn, column as DataGridBoundColumn );
            }
            else if ( column is IDataGridBoundColumn bindable )
            {
                binding = bindable.Binding as Binding;

                if ( binding != null )
                {
                    ( (IDataGridBoundColumn) clone ).Binding = BindingExtensions.Clone( binding );
                }
            }

            if ( typeof( DataGridTextColumn ).IsAssignableFrom( typeof( TColumn ) ) )
            {
                CloneTextColumn( clone as DataGridTextColumn, column as DataGridTextColumn );
            }

            if ( typeof( DataGridCheckBoxColumn ).IsAssignableFrom( typeof( TColumn ) ) )
            {
                ( clone as DataGridCheckBoxColumn ).IsThreeState = ( column as DataGridCheckBoxColumn ).IsThreeState;
            }

            if ( typeof( DataGridTemplateColumn ).IsAssignableFrom( typeof( TColumn ) ) )
            {
                CloneTemplateColumn( clone as DataGridTemplateColumn, column as DataGridTemplateColumn );
            }

            if ( column is IOptionalDataGridColumn optionalColumn )
            {
                ( (IOptionalDataGridColumn) clone ).IsOptional = optionalColumn.IsOptional;
            }

            return clone;
        }

        /// <summary>
        /// Makes a column read-only.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> derives from <see cref="DataGridBoundColumn"/>, the binding mode
        /// will be changed to <see cref="BindingMode.OneWay"/> if a binding is present.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn ReadOnly( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.IsReadOnly = true;

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
            {
                boundColumn.SetBindingMode( BindingMode.OneWay );
            }

            return column;
        }

        /// <summary>
        /// Makes a column read-write.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> derives from <see cref="DataGridBoundColumn"/>, the binding mode
        /// will be changed to <see cref="BindingMode.TwoWay"/> if a binding is present.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn ReadWrite( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.IsReadOnly = false;

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
            {
                boundColumn.SetBindingMode( BindingMode.TwoWay );
            }

            return column;
        }

        /// <summary>
        /// Makes a column required.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> implements <see cref="IOptionalDataGridColumn"/>, the <see cref="IOptionalDataGridColumn.IsOptional"/>
        /// property will be set to false; otherwise, this method has no effect.</remarks>
        public static DataGridColumn Required( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            if ( column is IOptionalDataGridColumn optionalColumn )
            {
                optionalColumn.IsOptional = false;
            }

            return column;
        }

        /// <summary>
        /// Makes a column optional.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> implements <see cref="IOptionalDataGridColumn"/>, the <see cref="IOptionalDataGridColumn.IsOptional"/>
        /// property will be set to true; otherwise, this method has no effect.</remarks>
        public static DataGridColumn Optional( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            if ( column is IOptionalDataGridColumn optionalColumn )
            {
                optionalColumn.IsOptional = true;
            }

            return column;
        }

        /// <summary>
        /// Makes a column visible.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn Visible( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.Visibility = Visibility.Visible;
            return column;
        }

        /// <summary>
        /// Makes a column hidden.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn Hidden( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.Visibility = Visibility.Collapsed;
            return column;
        }

        /// <summary>
        /// Sets the width of a column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="width">The column width.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn SetWidth( this DataGridColumn column, double? width )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            if ( width == null )
            {
                return column.SetAutoWidth();
            }

            column.Width = new DataGridLength( width.Value );

            return column;
        }

        /// <summary>
        /// Sets the width of a column to automatically adjust its width to the width of both the column header and column cells.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn SetAutoWidth( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.Width = DataGridLength.Auto;
            return column;
        }

        /// <summary>
        /// Sets the width of a column to automatically size to the width of the column cells.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn SetWidthToCells( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.Width = DataGridLength.SizeToCells;
            return column;
        }

        /// <summary>
        /// Sets the width of a column to automatically size to the width of the column header.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn SetWidthToHeader( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.Width = DataGridLength.SizeToHeader;
            return column;
        }

        /// <summary>
        /// Sets the value converter associated with the column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="converter">The <see cref="IValueConverter"/> to assign to the column.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetConverter( this DataGridColumn column, IValueConverter converter )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            return SetConverter( column, converter, null );
        }

        /// <summary>
        /// Sets the value converter associated with the column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="converter">The <see cref="IValueConverter"/> to assign to the column.</param>
        /// <param name="parameter">The value converter parameter.  This parameter can be null.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetConverter( this DataGridColumn column, IValueConverter converter, object parameter )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
            {
                boundColumn.SetBindingConverter( converter, parameter );
            }

            return column;
        }

        /// <summary>
        /// Enables a column to be sorted.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn EnableSort( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            return EnableSort( column, null );
        }

        /// <summary>
        /// Enables a column to be sorted by the specified member path.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="memberPath">The property name, or a period-delimited hierarchy of property names, that indicates the member to sort by.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/> and <paramref name="memberPath"/> is
        /// null or an empty string, this method has no effect.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn EnableSort( this DataGridColumn column, string memberPath )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.CanUserSort = true;

            if ( !IsNullOrEmpty( memberPath ) )
            {
                column.SortMemberPath = memberPath;
                return column;
            }

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn == null )
            {
                return column;
            }

            if ( boundColumn.Binding is Binding binding )
            {
                column.SortMemberPath = binding.Path.Path;
            }

            return column;
        }

        /// <summary>
        /// Disables a column from being sorted.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn DisableSort( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.CanUserSort = false;
            return column;
        }

        /// <summary>
        /// Enables a column to be resized.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn EnableResize( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.CanUserResize = true;
            return column;
        }

        /// <summary>
        /// Disables a column from being resized.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn DisableResize( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.CanUserResize = false;
            return column;
        }

        /// <summary>
        /// Enables a column to be reordered.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn EnableReorder( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.CanUserReorder = true;
            return column;
        }

        /// <summary>
        /// Disables a column from being reordered.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn DisableReorder( this DataGridColumn column )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            column.CanUserReorder = false;
            return column;
        }

        /// <summary>
        /// Sets the string format associated with column's binding.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="format">The string format assigned to the column.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetFormat( this DataGridColumn column, string format )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
            {
                boundColumn.Binding.StringFormat = format;
            }

            return column;
        }

        /// <summary>
        /// Sets the fallback value associated with column's binding.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="fallbackValue">The fallback <see cref="object"/> assigned to the column.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetFallbackValue( this DataGridColumn column, object fallbackValue )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
            {
                boundColumn.Binding.FallbackValue = fallbackValue;
            }

            return column;
        }

        /// <summary>
        /// Sets the value associated with column's binding when the source value is null.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="nullValue">The <see cref="object"/> assigned to the column when the source value is null.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetTargetNullValue( this DataGridColumn column, object nullValue )
        {
            Arg.NotNull( column, nameof( column ) );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
            {
                boundColumn.Binding.TargetNullValue = nullValue;
            }

            return column;
        }

        sealed class DataGridBoundColumnAdapter : IDataGridBoundColumn
        {
            readonly DataGridBoundColumn columm;

            internal DataGridBoundColumnAdapter( DataGridBoundColumn boundColumn ) => columm = boundColumn;

            public BindingBase Binding
            {
                get => columm.Binding;
                set => columm.Binding = value;
            }

            public BindingBase ClipboardContentBinding
            {
                get => columm.ClipboardContentBinding;
                set => columm.ClipboardContentBinding = value;
            }
        }
    }
}