namespace More.Windows.Controls
{
    using More.Windows.Media;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Provides extension methods for the <see cref="DataGridColumn"/> class.
    /// </summary>
    public static class DataGridColumnExtensions
    {
        private sealed class DataGridBoundColumnAdapter : IDataGridBoundColumn
        {
            private readonly DataGridBoundColumn columm;

            internal DataGridBoundColumnAdapter( DataGridBoundColumn boundColumn )
            {
                Contract.Requires( boundColumn != null );
                this.columm = boundColumn;
            }

            public BindingBase Binding
            {
                get
                {
                    return this.columm.Binding;
                }
                set
                {
                    this.columm.Binding = value;
                }
            }

            public BindingBase ClipboardContentBinding
            {
                get
                {
                    return this.columm.ClipboardContentBinding;
                }
                set
                {
                    this.columm.ClipboardContentBinding = value;
                }
            }
        }

        private static async Task<DataGridColumn> AddFromResourceAsync(
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

            column.IsReadOnly = string.IsNullOrEmpty( editTemplateName );
            column.IsOptional = true;
            column.Header = header;
            columns.Add( column );

            // edit if there's nothing else to do
            if ( string.IsNullOrEmpty( templateName ) && string.IsNullOrEmpty( editTemplateName ) )
                return column;

            if ( string.IsNullOrEmpty( resourceName ) )
            {
                var xaml = new XamlContent<DataTemplate>();

                // resolve template from specified assembly
                if ( !string.IsNullOrEmpty( templateName ) )
                    column.CellTemplate = await xaml.FromResourceAsync( assembly.CreatePackUri( templateName ) );

                // resolve template from specified assembly
                if ( !column.IsReadOnly && !string.IsNullOrEmpty( editTemplateName ) )
                    column.CellEditingTemplate = await xaml.FromResourceAsync( assembly.CreatePackUri( editTemplateName ) );
            }
            else
            {
                // resolve resource dictionary from specified assembly
                var xaml = new XamlContent<ResourceDictionary>();
                var resourceDictionary = await xaml.FromResourceAsync( assembly.CreatePackUri( resourceName ) );

                // resolve template from resource dictionary
                if ( !string.IsNullOrEmpty( templateName ) )
                    column.CellTemplate = (DataTemplate) resourceDictionary[templateName];

                // resolve template from resource dictionary
                if ( !column.IsReadOnly && !string.IsNullOrEmpty( editTemplateName ) )
                    column.CellEditingTemplate = (DataTemplate) resourceDictionary[editTemplateName];
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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var column = new OptionalDataGridTextColumn();
            column.IsOptional = true;

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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var binding = new Binding( propertyName );
            binding.Mode = BindingMode.OneWay;

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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( templateName ), "templateName" );
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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( templateName ), "templateName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( editTemplateName ), "editTemplateName" );
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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( resourceName ), "resourceName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( templateName ), "templateName" );
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
            Contract.Requires<ArgumentNullException>( columns != null, "columns" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( resourceName ), "resourceName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( templateName ), "templateName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( editTemplateName ), "editTemplateName" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            // NOTE: the resource must be resolved from the calling assembly
            var assembly = Assembly.GetCallingAssembly();
            return AddFromResourceAsync( assembly, columns, resourceName, templateName, editTemplateName, header );
        }

        private static IDataGridBoundColumn GetBoundColumn( this DataGridColumn column )
        {
            Contract.Requires( column != null );

            var bindableColumn = column as IDataGridBoundColumn;

            if ( bindableColumn == null )
            {
                var boundColumn = column as DataGridBoundColumn;

                if ( boundColumn == null )
                    return null;

                bindableColumn = new DataGridBoundColumnAdapter( boundColumn );
            }

            return bindableColumn;
        }

        private static void SetBindingMode( this IDataGridBoundColumn column, BindingMode mode )
        {
            Contract.Requires( column != null );

            var binding = column.Binding as Binding;

            if ( binding == null )
                return;

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

        private static void SetBindingConverter( this IDataGridBoundColumn column, IValueConverter converter, object parameter )
        {
            Contract.Requires( column != null );

            var binding = column.Binding as Binding;

            if ( binding == null )
                return;

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

        private static void CloneBoundColumn( DataGridBoundColumn clone, DataGridBoundColumn column )
        {
            Contract.Requires( clone != null );
            Contract.Requires( column != null );

            var binding = column.Binding as Binding;

            if ( binding != null )
                clone.Binding = BindingExtensions.Clone( binding );

            clone.EditingElementStyle = column.EditingElementStyle;
            clone.ElementStyle = column.ElementStyle;
        }

        private static void CloneTextColumn( DataGridTextColumn clone, DataGridTextColumn column )
        {
            Contract.Requires( clone != null );
            Contract.Requires( column != null );

            clone.FontFamily = column.FontFamily;
            clone.FontSize = column.FontSize;
            clone.FontStyle = column.FontStyle;
            clone.FontWeight = column.FontWeight;
            clone.Foreground = column.Foreground;
        }

        private static void CloneTemplateColumn( DataGridTemplateColumn clone, DataGridTemplateColumn column )
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );

            var c1 = column as DataGridBoundColumn;

            if ( c1 != null )
                return c1.Binding as Binding;

            var c2 = column as IDataGridBoundColumn;
            return c2 == null ? null : c2.Binding as Binding;
        }

        /// <summary>
        /// Gets the clipboard content binding associated with a column.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="Binding"/> object or null if the column doesn't support data binding.</returns>
        public static Binding GetClipboardContentBinding( this DataGridColumn column )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );

            var c1 = column as DataGridBoundColumn;

            if ( c1 != null )
                return c1.ClipboardContentBinding as Binding;

            var c2 = column as IDataGridBoundColumn;
            return c2 == null ? null : c2.ClipboardContentBinding as Binding;
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
            // NOTE: since DataGridColumn is abstract, we manually check for a parameterless constructor rather than using the new() type constraint
            // which supports scenarios such as cloning all the columns in a collection (which may be of different types)
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Requires<InvalidOperationException>( !column.GetType().IsAbstract );
            Contract.Requires<InvalidOperationException>( Contract.Exists( column.GetType().GetConstructors(), c => c.GetParameters().Length == 0 ) );
            Contract.Ensures( Contract.Result<TColumn>() != null );

            var clone = (TColumn) Activator.CreateInstance( column.GetType() );
            var binding = column.ClipboardContentBinding as Binding;

            if ( binding != null )
                clone.ClipboardContentBinding = BindingExtensions.Clone( binding );

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

            // special handling for out-of-the-box columns
            if ( typeof( DataGridBoundColumn ).IsAssignableFrom( typeof( TColumn ) ) )
                CloneBoundColumn( clone as DataGridBoundColumn, column as DataGridBoundColumn );
            else
            {
                // additional special case if the template column supports a binding interface
                // all other DataGridBoundColumn cases are already covered
                var bindable = column as IDataGridBoundColumn;

                if ( bindable != null )
                {
                    binding = bindable.Binding as Binding;

                    if ( binding != null )
                        ( (IDataGridBoundColumn) clone ).Binding = BindingExtensions.Clone( binding );
                }
            }

            if ( typeof( DataGridTextColumn ).IsAssignableFrom( typeof( TColumn ) ) )
                CloneTextColumn( clone as DataGridTextColumn, column as DataGridTextColumn );

            if ( typeof( DataGridCheckBoxColumn ).IsAssignableFrom( typeof( TColumn ) ) )
                ( clone as DataGridCheckBoxColumn ).IsThreeState = ( column as DataGridCheckBoxColumn ).IsThreeState;

            if ( typeof( DataGridTemplateColumn ).IsAssignableFrom( typeof( TColumn ) ) )
                CloneTemplateColumn( clone as DataGridTemplateColumn, column as DataGridTemplateColumn );

            var optional = column as IOptionalDataGridColumn;

            if ( optional != null )
                ( (IOptionalDataGridColumn) clone ).IsOptional = optional.IsOptional;

            return clone;
        }

        /// <summary>
        /// Makes a column read-only.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> derives from <see cref="DataGridBoundColumn"/>, the binding mode
        /// will be changed to <see cref="T:BindingMode.OneWay"/> if a binding is present.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn ReadOnly( this DataGridColumn column )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.IsReadOnly = true;

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
                boundColumn.SetBindingMode( BindingMode.OneWay );

            return column;
        }

        /// <summary>
        /// Makes a column read-write.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> derives from <see cref="DataGridBoundColumn"/>, the binding mode
        /// will be changed to <see cref="T:BindingMode.TwoWay"/> if a binding is present.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static DataGridColumn ReadWrite( this DataGridColumn column )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.IsReadOnly = false;

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
                boundColumn.SetBindingMode( BindingMode.TwoWay );

            return column;
        }

        /// <summary>
        /// Makes a column required.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> implements <see cref="IOptionalDataGridColumn"/>, the <see cref="P:IOptionalDataGridColumn.IsOptional"/>
        /// property will be set to false; otherwise, this method has no effect.</remarks>
        public static DataGridColumn Required( this DataGridColumn column )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var optionalColumn = column as IOptionalDataGridColumn;

            if ( optionalColumn != null )
                optionalColumn.IsOptional = false;

            return column;
        }

        /// <summary>
        /// Makes a column optional.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> implements <see cref="IOptionalDataGridColumn"/>, the <see cref="P:IOptionalDataGridColumn.IsOptional"/>
        /// property will be set to true; otherwise, this method has no effect.</remarks>
        public static DataGridColumn Optional( this DataGridColumn column )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var optionalColumn = column as IOptionalDataGridColumn;

            if ( optionalColumn != null )
                optionalColumn.IsOptional = true;

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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            if ( width == null )
                return column.SetAutoWidth();

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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding is Binding )
                boundColumn.SetBindingConverter( converter, parameter );

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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            column.CanUserSort = true;

            if ( !string.IsNullOrEmpty( memberPath ) )
            {
                column.SortMemberPath = memberPath;
                return column;
            }

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn == null )
                return column;

            var binding = boundColumn.Binding as Binding;

            if ( binding != null )
                column.SortMemberPath = binding.Path.Path;

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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
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
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
                boundColumn.Binding.StringFormat = format;

            return column;
        }

        /// <summary>
        /// Sets the fallback value associated with column's binding.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="fallbackValue">The fallback <see cref="Object"/> assigned to the column.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetFallbackValue( this DataGridColumn column, object fallbackValue )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
                boundColumn.Binding.FallbackValue = fallbackValue;

            return column;
        }

        /// <summary>
        /// Sets the value associated with column's binding when the source value is null.
        /// </summary>
        /// <param name="column">The extended <see cref="DataGridColumn"/> object.</param>
        /// <param name="nullValue">The <see cref="Object"/> assigned to the column when the source value is null.</param>
        /// <returns>A <see cref="DataGridColumn"/> object.</returns>
        /// <remarks>If <paramref name="column"/> does not derive from <see cref="DataGridBoundColumn"/>, this method has no effect.</remarks>
        public static DataGridColumn SetTargetNullValue( this DataGridColumn column, object nullValue )
        {
            Contract.Requires<ArgumentNullException>( column != null, "column" );
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );

            var boundColumn = column.GetBoundColumn();

            if ( boundColumn != null && boundColumn.Binding != null )
                boundColumn.Binding.TargetNullValue = nullValue;

            return column;
        }
    }
}
