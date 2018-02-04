namespace More.Windows.Data
{
    using More.Windows.Controls;
    using More.Windows.Media;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using static ColumnBuildOrders;
    using static System.String;

    /// <summary>
    /// Represents a class responsible for building up metadata-driven columns for a specified type
    /// <seealso cref="DataGridColumnAttribute"/><seealso cref="DataGridTemplateColumnAttribute"/>
    /// <seealso cref="ValueConverterAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to build columns for.</typeparam>
    /// <example>This example demonstrates how to decorate object properties with the <see cref="DataGridColumnAttribute"/> and
    /// build-up a collection of <see cref="DataGridColumn"/> objects.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Threading.Tasks;
    /// using System.Windows;
    /// using System.Windows.Data;
    /// using System.Windows.Controls;
    ///
    /// public class MyClass
    /// {
    ///     [DataGridColumn( DisplayIndex = 0 )]
    ///     public string Name
    ///     {
    ///         get;
    ///         set;
    ///     }
    ///
    ///     [DataGridColumn( DisplayIndex = 2, StringFormat = "C" )]
    ///     public decimal Amount
    ///     {
    ///         get;
    ///         set;
    ///     }
    ///
    ///     [DataGridColumn( DisplayIndex = 1, StringFormat = "MM-dd-yyy", Header = "Created" )]
    ///     public DateTime CreatedDate
    ///     {
    ///         get;
    ///         set;
    ///     }
    /// }
    ///
    /// // build-up list of metadata-driven columns for a model
    /// var builder = new DataGridColumnBuilder<MyClass>();
    /// var columns = new List<DataGridColumn>();
    /// await builder.BuildUpAsync( columns );
    ///
    /// // build-up list of metadata-driven columns for an adapted model.
    /// // assume the bound items follow the adapter pattern; for example, SelectableItem<MyClass>.
    /// // the SelectableItem<MyClass>.Value property will be the AdapterBindingPath to the model.
    /// var adaptedBuilder = new DataGridColumnBuilder<MyClass>( "Value" );
    /// await adaptedBuilder.BuildUpAsync( columns );
    ///
    /// ]]></code></example>
    public partial class DataGridColumnBuilder<T> : IDataGridColumnBuilder
    {
        readonly XamlContent<DataTemplate> templateFactory = new XamlContent<DataTemplate>();
        readonly XamlContent<Style> styleFactory = new XamlContent<Style>();
        readonly XamlContent<ResourceDictionary> resourceDictionaryFactory = new XamlContent<ResourceDictionary>();
        readonly ConcurrentDictionary<Uri, DataTemplate> templates = new ConcurrentDictionary<Uri, DataTemplate>( new Dictionary<Uri, DataTemplate>( UriComparer.OrdinalIgnoreCase ) );
        readonly ConcurrentDictionary<Uri, Style> styles = new ConcurrentDictionary<Uri, Style>( new Dictionary<Uri, Style>( UriComparer.OrdinalIgnoreCase ) );
        readonly ConcurrentDictionary<string, DataTemplate> parsedTemplates = new ConcurrentDictionary<string, DataTemplate>( new Dictionary<string, DataTemplate>( StringComparer.Ordinal ) );
        readonly ConcurrentDictionary<string, Style> parsedStyles = new ConcurrentDictionary<string, Style>( new Dictionary<string, Style>( StringComparer.Ordinal ) );
        readonly ConcurrentDictionary<Uri, ResourceDictionary> resourceDictionaries = new ConcurrentDictionary<Uri, ResourceDictionary>( new Dictionary<Uri, ResourceDictionary>( UriComparer.OrdinalIgnoreCase ) );

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnBuilder{T}"/> class.
        /// </summary>
        public DataGridColumnBuilder() => AdapterBindingPath = Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnBuilder{T}"/> class.
        /// </summary>
        /// <param name="adapterBindingPath">The adapter binding path for the target type.  When specified, this parameter is used in
        /// conjunction with the target type to construct the qualified binding path from the adapted type to the target type. The specified
        /// path should be the binding path prefix to any target type property.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public DataGridColumnBuilder( string adapterBindingPath )
        {
            Arg.NotNullOrEmpty( adapterBindingPath, nameof( adapterBindingPath ) );
            AdapterBindingPath = adapterBindingPath.TrimEnd( '.' );
        }

        static TAttribute GetAttribute<TAttribute>( IEnumerable<Attribute> attributes ) where TAttribute : Attribute =>
            (TAttribute) attributes.SingleOrDefault( a => a.GetType().Equals( typeof( TAttribute ) ) );

        static TAttribute GetAssignableAttribute<TAttribute>( IEnumerable<Attribute> attributes ) where TAttribute : Attribute =>
            attributes.OfType<TAttribute>().SingleOrDefault();

        static IValueConverter GetValueConverter( IEnumerable<Attribute> attributes ) => attributes.OfType<IValueConverter>().FirstOrDefault();

        static TColumn BuildUpColumn<TColumn>( TColumn column, DataGridColumnAttribute attribute, string propertyName, string adapterBindingPath ) where TColumn : DataGridColumn
        {
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );
            Contract.Requires( !IsNullOrEmpty( propertyName ) );
            Contract.Requires( adapterBindingPath != null );
            Contract.Ensures( Contract.Result<TColumn>() != null );

            column.CanUserReorder = attribute.CanReorder;
            column.CanUserResize = attribute.CanResize;
            column.CanUserSort = attribute.CanSort;
            column.DisplayIndex = attribute.DisplayIndex;
            column.IsReadOnly = attribute.IsReadOnly;
            column.MaxWidth = attribute.MaxWidth;
            column.MinWidth = attribute.MinWidth;
            column.Visibility = attribute.Visibility;

            if ( attribute.SizeToHeader )
            {
                column.Width = DataGridLength.SizeToHeader;
            }
            else if ( attribute.SizeToCells )
            {
                column.Width = DataGridLength.SizeToCells;
            }
            else if ( double.IsNaN( attribute.Width ) )
            {
                column.Width = DataGridLength.Auto;
            }
            else
            {
                column.Width = new DataGridLength( attribute.Width );
            }

            if ( IsNullOrEmpty( adapterBindingPath ) )
            {
                if ( !IsNullOrEmpty( attribute.SortMemberPath ) )
                {
                    column.SortMemberPath = attribute.SortMemberPath;
                }
            }
            else
            {
                if ( IsNullOrEmpty( attribute.SortMemberPath ) )
                {
                    column.SortMemberPath = Format( null, "{0}.{1}", adapterBindingPath, propertyName );
                }
                else
                {
                    column.SortMemberPath = Format( null, "{0}.{1}", adapterBindingPath, attribute.SortMemberPath );
                }
            }

            if ( !ReferenceEquals( attribute.Header, DataGridColumnAttribute.Unspecified ) )
            {
                column.Header = IsNullOrEmpty( attribute.Header ) ? Util.HeaderTextFromPropertyName( propertyName ) : attribute.Header;
            }

            return column;
        }

        static string GetBindingPath( string propertyName, string bindingPath, string adapterBindingPath )
        {
            Contract.Requires( !IsNullOrEmpty( propertyName ) );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var path = IsNullOrEmpty( bindingPath ) ? propertyName : bindingPath;

            if ( !IsNullOrEmpty( adapterBindingPath ) )
            {
                path = Format( null, "{0}.{1}", adapterBindingPath, path );
            }

            return path;
        }

        async Task<DataTemplate> GetDataTemplateAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<DataTemplate>() != null );

            if ( templates.TryGetValue( resourceUri, out var template ) )
            {
                return template;
            }

            template = await templateFactory.FromResourceAsync( resourceUri ).ConfigureAwait( true );
            templates.TryAdd( resourceUri, template );

            return template;
        }

        async Task<Style> GetStyleAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<Style>() != null );

            if ( styles.TryGetValue( resourceUri, out var style ) )
            {
                return style;
            }

            style = await styleFactory.FromResourceAsync( resourceUri ).ConfigureAwait( true );
            styles.TryAdd( resourceUri, style );

            return style;
        }

        async Task<ResourceDictionary> GetResourceDictionaryAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<ResourceDictionary>() != null );

            if ( resourceDictionaries.TryGetValue( resourceUri, out var resources ) )
            {
                return resources;
            }

            resources = await resourceDictionaryFactory.FromResourceAsync( resourceUri ).ConfigureAwait( true );
            resourceDictionaries.TryAdd( resourceUri, resources );

            return resources;
        }

        static string NewUniqueResourceKey( Uri resourceUri, string resourceKey )
        {
            Contract.Requires( resourceUri != null );
            Contract.Requires( !IsNullOrEmpty( resourceKey ) );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            // create key based resource and resource (to avoid duplicate names in difference resource dictionaries)
            return Format( CultureInfo.InvariantCulture, "{0}:{1}", resourceUri.ToString(), resourceKey );
        }

        async Task<DataTemplate> GetDataTemplateFromResourceAsync( Uri resourceUri, string resourceKey )
        {
            Contract.Requires( resourceUri != null );
            Contract.Requires( !IsNullOrEmpty( resourceKey ) );
            Contract.Ensures( Contract.Result<DataTemplate>() != null );

            var key = NewUniqueResourceKey( resourceUri, resourceKey );

            if ( parsedTemplates.TryGetValue( key, out var template ) )
            {
                return template;
            }

            var resources = await GetResourceDictionaryAsync( resourceUri ).ConfigureAwait( true );

            template = (DataTemplate) resources[resourceKey];
            parsedTemplates.TryAdd( key, template );

            return template;
        }

        async Task<Style> GetStyleFromResourceAsync( Uri resourceUri, string resourceKey )
        {
            Contract.Requires( resourceUri != null );
            Contract.Requires( !IsNullOrEmpty( resourceKey ) );
            Contract.Ensures( Contract.Result<Style>() != null );

            var key = NewUniqueResourceKey( resourceUri, resourceKey );

            if ( parsedStyles.TryGetValue( key, out var style ) )
            {
                return style;
            }

            var resources = await GetResourceDictionaryAsync( resourceUri ).ConfigureAwait( true );

            style = (Style) resources[resourceKey];
            parsedStyles.TryAdd( key, style );

            return style;
        }

        async Task ApplyTemplatesAsync( Type type, DataGridTemplateColumn column, DataGridTemplateColumnAttribute attribute )
        {
            Contract.Requires( type != null );
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );

            if ( IsNullOrEmpty( attribute.CellTemplateName ) && IsNullOrEmpty( attribute.CellEditingTemplateName ) )
            {
                return;
            }

            if ( IsNullOrEmpty( attribute.ResourceDictionary ) )
            {
                if ( !IsNullOrEmpty( attribute.CellTemplateName ) )
                {
                    column.CellTemplate = await GetDataTemplateAsync( type.CreatePackUri( attribute.CellTemplateName ) ).ConfigureAwait( true );
                }

                if ( !column.IsReadOnly && !IsNullOrEmpty( attribute.CellEditingTemplateName ) )
                {
                    column.CellEditingTemplate = await GetDataTemplateAsync( type.CreatePackUri( attribute.CellEditingTemplateName ) ).ConfigureAwait( true );
                }
            }
            else
            {
                var resourceUri = type.CreatePackUri( attribute.ResourceDictionary );

                if ( !IsNullOrEmpty( attribute.CellTemplateName ) )
                {
                    column.CellTemplate = await GetDataTemplateFromResourceAsync( resourceUri, attribute.CellTemplateName ).ConfigureAwait( true );
                }

                if ( !column.IsReadOnly && !IsNullOrEmpty( attribute.CellEditingTemplateName ) )
                {
                    column.CellEditingTemplate = await GetDataTemplateFromResourceAsync( resourceUri, attribute.CellEditingTemplateName ).ConfigureAwait( true );
                }
            }
        }

        async Task ApplyStylesAsync( Type type, DataGridColumn column, DataGridElementStyleAttribute attribute )
        {
            Contract.Requires( type != null );
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );

            if ( !( column is DataGridBoundColumn boundColumn ) )
            {
                return;
            }

            if ( IsNullOrEmpty( attribute.ElementStyleName ) && IsNullOrEmpty( attribute.EditingElementStyleName ) )
            {
                return;
            }

            if ( IsNullOrEmpty( attribute.ResourceDictionary ) )
            {
                if ( !IsNullOrEmpty( attribute.ElementStyleName ) )
                {
                    boundColumn.ElementStyle = await GetStyleAsync( type.CreatePackUri( attribute.ElementStyleName ) ).ConfigureAwait( true );
                }

                if ( !boundColumn.IsReadOnly && !IsNullOrEmpty( attribute.EditingElementStyleName ) )
                {
                    boundColumn.EditingElementStyle = await GetStyleAsync( type.CreatePackUri( attribute.EditingElementStyleName ) ).ConfigureAwait( true );
                }
            }
            else
            {
                var resourceUri = type.CreatePackUri( attribute.ResourceDictionary );

                if ( !IsNullOrEmpty( attribute.ElementStyleName ) )
                {
                    boundColumn.ElementStyle = await GetStyleFromResourceAsync( resourceUri, attribute.ElementStyleName ).ConfigureAwait( true );
                }

                if ( !column.IsReadOnly && !IsNullOrEmpty( attribute.EditingElementStyleName ) )
                {
                    boundColumn.EditingElementStyle = await GetStyleFromResourceAsync( resourceUri, attribute.EditingElementStyleName ).ConfigureAwait( true );
                }
            }
        }

        static IEnumerable<Tuple<PropertyInfo, Attribute[]>> SortByDisplayIndex( IEnumerable<Tuple<PropertyInfo, Attribute[]>> properties, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( properties != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, Attribute[]>>>() != null );

            var i = 0;

            if ( ( buildOrders & Ascending ) == Ascending )
            {
                return from property in properties
                       let displayIndex = GetAttribute<DataGridColumnAttribute>( property.Item2 ).DisplayIndex
                       let index = displayIndex < 0 ? i++ : displayIndex
                       orderby index
                       select property;
            }
            else if ( ( buildOrders & Descending ) == Descending )
            {
                return from property in properties
                       let displayIndex = GetAttribute<DataGridColumnAttribute>( property.Item2 ).DisplayIndex
                       let index = displayIndex < 0 ? i++ : displayIndex
                       orderby index descending
                       select property;
            }

            return properties;
        }

        static IEnumerable<Tuple<PropertyInfo, Attribute[]>> SortByName( IEnumerable<Tuple<PropertyInfo, Attribute[]>> properties, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( properties != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, Attribute[]>>>() != null );

            if ( ( buildOrders & Ascending ) == Ascending )
            {
                return from property in properties
                       let header = GetAttribute<DataGridColumnAttribute>( property.Item2 ).Header
                       let name = IsNullOrEmpty( header ) ? Util.HeaderTextFromPropertyName( property.Item1.Name ) : header
                       orderby name
                       select property;
            }
            else if ( ( buildOrders & Descending ) == Descending )
            {
                return from property in properties
                       let header = GetAttribute<DataGridColumnAttribute>( property.Item2 ).Header
                       let name = IsNullOrEmpty( header ) ? Util.HeaderTextFromPropertyName( property.Item1.Name ) : header
                       orderby name descending
                       select property;
            }

            return properties;
        }

        static IEnumerable<Tuple<PropertyInfo, Attribute[]>> GetPropertiesInOrder( Type type, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( type != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, Attribute[]>>>() != null );

            var list = new List<Tuple<PropertyInfo, Attribute[]>>();

            foreach ( var property in type.GetRuntimeProperties() )
            {
                if ( Attribute.IsDefined( property, typeof( DataGridColumnAttribute ), true ) )
                {
                    list.Add( Tuple.Create( property, property.GetCustomAttributes( true ).Cast<Attribute>().ToArray() ) );
                }
            }

            if ( ( buildOrders & DisplayIndex ) == DisplayIndex )
            {
                return SortByDisplayIndex( list, buildOrders );
            }
            else if ( ( buildOrders & Name ) == Name )
            {
                return SortByName( list, buildOrders );
            }

            return list;
        }

        static Binding CreateBinding( string propertyName, string adapterPath, IValueConverter converter, DataGridColumnAttribute attribute )
        {
            Contract.Requires( !IsNullOrEmpty( propertyName ) );
            Contract.Requires( adapterPath != null );
            Contract.Requires( attribute != null );
            Contract.Ensures( Contract.Result<Binding>() != null );

            var bindingPath = GetBindingPath( propertyName, attribute.BindingPath, adapterPath );

            return new Binding( bindingPath )
            {
                Converter = converter,
                FallbackValue = attribute.FallbackValue,
                Mode = attribute.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                StringFormat = attribute.StringFormat,
                TargetNullValue = attribute.TargetNullValue,
            };
        }

        static void SetBinding( DataGridColumn column, Binding binding )
        {
            Contract.Requires( column != null );

            if ( column is IDataGridBoundColumn boundColumn )
            {
                boundColumn.Binding = binding;
            }
        }

        static void SetOptional( DataGridColumn column, bool optional )
        {
            Contract.Requires( column != null );

            if ( column is IOptionalDataGridColumn optionalColumn )
            {
                optionalColumn.IsOptional = optional;
            }
        }

        static void SetTemplateDependencyProperties( DataGridTemplateColumn column, DataGridTemplateColumnAttribute attribute )
        {
            Contract.Requires( column != null );

            var templateColumn = column as OptionalDataGridTemplateColumn;

            if ( templateColumn == null || attribute == null )
            {
                return;
            }

            templateColumn.ContentDependencyProperty = attribute.ContentDependencyProperty;
            templateColumn.EditingContentDependencyProperty = attribute.EditingContentDependencyProperty;
        }

        async Task<IEnumerable<DataGridColumn>> GetColumnsAsync( Type type, string adapterPath, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Contract.Requires( type != null );
            Contract.Requires( adapterPath != null );
            Contract.Ensures( Contract.Result<Task<IEnumerable<DataGridColumn>>>() != null );

            var columns = new List<DataGridColumn>();

            foreach ( var property in GetPropertiesInOrder( type, buildOrders ) )
            {
                cancellationToken.ThrowIfCancellationRequested();

                var propertyName = property.Item1.Name;
                var attribute = GetAttribute<DataGridColumnAttribute>( property.Item2 );
                var template = GetAttribute<DataGridTemplateColumnAttribute>( property.Item2 );
                var converter = GetValueConverter( property.Item2 );

                if ( attribute.DisplayIndex == -1 )
                {
                    attribute.DisplayIndex = columns.Count;
                }

                if ( template == null )
                {
                    var column = BuildUpColumn( CreateTextColumn(), attribute, propertyName, adapterPath );
                    var binding = CreateBinding( propertyName, adapterPath, converter, attribute );

                    SetOptional( column, !attribute.IsRequired );
                    SetBinding( column, binding );
                    column.ClipboardContentBinding = binding.Clone();

                    var style = GetAttribute<DataGridElementStyleAttribute>( property.Item2 );

                    if ( style != null )
                    {
                        await ApplyStylesAsync( type, column, style ).ConfigureAwait( true );
                    }

                    columns.Add( column );
                }
                else
                {
                    var column = BuildUpColumn( CreateTemplateColumn(), attribute, propertyName, adapterPath );
                    var binding = CreateBinding( propertyName, adapterPath, converter, attribute );

                    SetOptional( column, !attribute.IsRequired );
                    SetBinding( column, binding );
                    SetTemplateDependencyProperties( column, template );
                    column.ClipboardContentBinding = binding.Clone();

                    await ApplyTemplatesAsync( type, column, template ).ConfigureAwait( true );

                    columns.Add( column );
                }
            }

            return columns;
        }

        /// <summary>
        /// Creates and returns a new data grid text column.
        /// </summary>
        /// <returns>A <see cref="DataGridColumn"/> for text.</returns>
        protected virtual DataGridColumn CreateTextColumn()
        {
            Contract.Ensures( Contract.Result<DataGridColumn>() != null );
            return new OptionalDataGridTextColumn();
        }

        /// <summary>
        /// Creates and returns a new data grid template column.
        /// </summary>
        /// <returns>A templated <see cref="DataGridTemplateColumn">data grid column</see>.</returns>
        protected virtual DataGridTemplateColumn CreateTemplateColumn()
        {
            Contract.Ensures( Contract.Result<DataGridTemplateColumn>() != null );
            return new OptionalDataGridTemplateColumn();
        }

        /// <summary>
        /// Gets the target type the builder is for.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type TargetType { get; } = typeof( T );

        /// <summary>
        /// Gets the adapter binding path for the specified model type.
        /// </summary>
        /// <value>The adapter binding path for the model type. The default value is an empty string.</value>
        /// <remarks>This property is only used when the model type is adapted by another type and the binding path
        /// of the adapter must be specified for data binding.</remarks>
        public string AdapterBindingPath { get; }

        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual Task BuildupAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Arg.NotNull( columns, nameof( columns ) );
            columns.Clear();
            return AppendToAsync( columns, buildOrders, cancellationToken );
        }

        /// <summary>
        /// Appends the columns resolved by the builder into the specified collection asynchronously.
        /// </summary>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual async Task AppendToAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Arg.NotNull( columns, nameof( columns ) );
            var columnsToAppend = await GetColumnsAsync( TargetType, AdapterBindingPath, buildOrders, cancellationToken ).ConfigureAwait( true );
            columns.AddRange( columnsToAppend );
        }
    }
}