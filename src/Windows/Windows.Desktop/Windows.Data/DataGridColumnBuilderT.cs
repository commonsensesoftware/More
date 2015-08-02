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
    using System.Windows.Media;

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
        private readonly string adapterBindingPath;
        private readonly XamlContent<DataTemplate> templateFactory = new XamlContent<DataTemplate>();
        private readonly XamlContent<Style> styleFactory = new XamlContent<Style>();
        private readonly XamlContent<ResourceDictionary> resourceDictionaryFactory = new XamlContent<ResourceDictionary>();
        private readonly ConcurrentDictionary<Uri, DataTemplate> templates = new ConcurrentDictionary<Uri, DataTemplate>( new Dictionary<Uri, DataTemplate>( UriComparer.OrdinalIgnoreCase ) );
        private readonly ConcurrentDictionary<Uri, Style> styles = new ConcurrentDictionary<Uri, Style>( new Dictionary<Uri, Style>( UriComparer.OrdinalIgnoreCase ) );
        private readonly ConcurrentDictionary<string, DataTemplate> parsedTemplates = new ConcurrentDictionary<string, DataTemplate>( new Dictionary<string, DataTemplate>( StringComparer.Ordinal ) );
        private readonly ConcurrentDictionary<string, Style> parsedStyles = new ConcurrentDictionary<string, Style>( new Dictionary<string, Style>( StringComparer.Ordinal ) );
        private readonly ConcurrentDictionary<Uri, ResourceDictionary> resourceDictionaries = new ConcurrentDictionary<Uri, ResourceDictionary>( new Dictionary<Uri, ResourceDictionary>( UriComparer.OrdinalIgnoreCase ) );

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnBuilder{T}"/> class.
        /// </summary>
        public DataGridColumnBuilder()
        {
            adapterBindingPath = string.Empty;
        }

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
            this.adapterBindingPath = adapterBindingPath.TrimEnd( '.' );
        }

        private static TAttribute GetAttribute<TAttribute>( IEnumerable<Attribute> attributes ) where TAttribute : Attribute
        {
            Contract.Requires( attributes != null );
            return (TAttribute) attributes.SingleOrDefault( a => a.GetType().Equals( typeof( TAttribute ) ) );
        }

        private static TAttribute GetAssignableAttribute<TAttribute>( IEnumerable<Attribute> attributes ) where TAttribute : Attribute
        {
            Contract.Requires( attributes != null );
            return (TAttribute) attributes.SingleOrDefault( a => typeof( TAttribute ).IsAssignableFrom( a.GetType() ) );
        }

        private static IValueConverter GetValueConverter( IEnumerable<Attribute> attributes )
        {
            Contract.Requires( attributes != null );
            return (IValueConverter) attributes.FirstOrDefault( a => typeof( IValueConverter ).IsAssignableFrom( a.GetType() ) );
        }

        private static TColumn BuildUpColumn<TColumn>( TColumn column, DataGridColumnAttribute attribute, string propertyName, string adapterBindingPath ) where TColumn : DataGridColumn
        {
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
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

            // set column width
            if ( attribute.SizeToHeader )
                column.Width = DataGridLength.SizeToHeader;
            else if ( attribute.SizeToCells )
                column.Width = DataGridLength.SizeToCells;
            else if ( double.IsNaN( attribute.Width ) )
                column.Width = DataGridLength.Auto;
            else
                column.Width = new DataGridLength( attribute.Width );

            if ( string.IsNullOrEmpty( adapterBindingPath ) )
            {
                // only change default behavior when specified
                if ( !string.IsNullOrEmpty( attribute.SortMemberPath ) )
                    column.SortMemberPath = attribute.SortMemberPath;
            }
            else
            {
                // must change the sort member path
                if ( string.IsNullOrEmpty( attribute.SortMemberPath ) )
                    column.SortMemberPath = string.Format( null, "{0}.{1}", adapterBindingPath, propertyName );
                else
                    column.SortMemberPath = string.Format( null, "{0}.{1}", adapterBindingPath, attribute.SortMemberPath );
            }

            // if the column header should be blank, the special "Unspecified" value must be used
            if ( !object.ReferenceEquals( attribute.Header, DataGridColumnAttribute.Unspecified ) )
                column.Header = string.IsNullOrEmpty( attribute.Header ) ? Util.HeaderTextFromPropertyName( propertyName ) : attribute.Header;

            return column;
        }

        private static string GetBindingPath( string propertyName, string bindingPath, string adapterBindingPath )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var path = string.IsNullOrEmpty( bindingPath ) ? propertyName : bindingPath;

            if ( !string.IsNullOrEmpty( adapterBindingPath ) )
                path = string.Format( null, "{0}.{1}", adapterBindingPath, path );

            return path;
        }

        private async Task<DataTemplate> GetDataTemplateAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<DataTemplate>() != null );

            DataTemplate template = null;

            // look up existing data template
            if ( templates.TryGetValue( resourceUri, out template ) )
                return template;

            // create and cache data template for future reference
            template = await templateFactory.FromResourceAsync( resourceUri );
            templates.TryAdd( resourceUri, template );

            return template;
        }

        private async Task<Style> GetStyleAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<Style>() != null );

            Style style = null;

            // look up existing style
            if ( styles.TryGetValue( resourceUri, out style ) )
                return style;

            // create and cache styles for future reference
            style = await styleFactory.FromResourceAsync( resourceUri );
            styles.TryAdd( resourceUri, style );

            return style;
        }

        private async Task<ResourceDictionary> GetResourceDictionaryAsync( Uri resourceUri )
        {
            Contract.Requires( resourceUri != null );
            Contract.Ensures( Contract.Result<ResourceDictionary>() != null );

            ResourceDictionary resources = null;

            // look up existing resource dictionary
            if ( resourceDictionaries.TryGetValue( resourceUri, out resources ) )
                return resources;

            // create and cache resource dictionary for future reference
            resources = await resourceDictionaryFactory.FromResourceAsync( resourceUri );
            resourceDictionaries.TryAdd( resourceUri, resources );

            return resources;
        }

        private async Task<DataTemplate> GetDataTemplateFromResourceAsync( Uri resourceUri, string resourceKey )
        {
            Contract.Requires( resourceUri != null );
            Contract.Requires( !string.IsNullOrEmpty( resourceKey ) );
            Contract.Ensures( Contract.Result<DataTemplate>() != null );

            // create key based resource and resource (to avoid duplicate names in difference resource dictionaries)
            var key = string.Format( CultureInfo.InvariantCulture, "{0}:{1}", resourceUri.ToString(), resourceKey );
            DataTemplate template = null;

            if ( parsedTemplates.TryGetValue( key, out template ) )
                return template;

            var resources = await GetResourceDictionaryAsync( resourceUri );

            template = (DataTemplate) resources[resourceKey];
            parsedTemplates.TryAdd( key, template );

            return template;
        }

        private async Task<Style> GetStyleFromResourceAsync( Uri resourceUri, string resourceKey )
        {
            Contract.Requires( resourceUri != null );
            Contract.Requires( !string.IsNullOrEmpty( resourceKey ) );
            Contract.Ensures( Contract.Result<Style>() != null );

            // create key based resource and resource (to avoid duplicate names in difference resource dictionaries)
            var key = string.Format( CultureInfo.InvariantCulture, "{0}:{1}", resourceUri.ToString(), resourceKey );
            Style style = null;

            if ( parsedStyles.TryGetValue( key, out style ) )
                return style;

            var resources = await GetResourceDictionaryAsync( resourceUri );

            style = (Style) resources[resourceKey];
            parsedStyles.TryAdd( key, style );

            return style;
        }

        private async Task ApplyTemplatesAsync( Type type, DataGridTemplateColumn column, DataGridTemplateColumnAttribute attribute )
        {
            Contract.Requires( type != null );
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );

            // exit if there's nothing to do
            if ( string.IsNullOrEmpty( attribute.CellTemplateName ) && string.IsNullOrEmpty( attribute.CellEditingTemplateName ) )
                return;

            if ( string.IsNullOrEmpty( attribute.ResourceDictionary ) )
            {
                // resolve template from specified assembly
                if ( !string.IsNullOrEmpty( attribute.CellTemplateName ) )
                    column.CellTemplate = await GetDataTemplateAsync( type.CreatePackUri( attribute.CellTemplateName ) );

                // resolve template from specified assembly
                if ( !column.IsReadOnly && !string.IsNullOrEmpty( attribute.CellEditingTemplateName ) )
                    column.CellEditingTemplate = await GetDataTemplateAsync( type.CreatePackUri( attribute.CellEditingTemplateName ) );
            }
            else
            {
                // resolve resource dictionary from specified assembly
                var resourceUri = type.CreatePackUri( attribute.ResourceDictionary );

                // resolve template from resource dictionary
                if ( !string.IsNullOrEmpty( attribute.CellTemplateName ) )
                    column.CellTemplate = await GetDataTemplateFromResourceAsync( resourceUri, attribute.CellTemplateName );

                // resolve template from resource dictionary
                if ( !column.IsReadOnly && !string.IsNullOrEmpty( attribute.CellEditingTemplateName ) )
                    column.CellEditingTemplate = await GetDataTemplateFromResourceAsync( resourceUri, attribute.CellEditingTemplateName );
            }
        }

        private async Task ApplyStylesAsync( Type type, DataGridColumn column, DataGridElementStyleAttribute attribute )
        {
            Contract.Requires( type != null );
            Contract.Requires( column != null );
            Contract.Requires( attribute != null );

            var boundColumn = column as DataGridBoundColumn;

            if ( boundColumn == null )
                return;

            // exit if there's nothing to do
            if ( string.IsNullOrEmpty( attribute.ElementStyleName ) && string.IsNullOrEmpty( attribute.EditingElementStyleName ) )
                return;

            if ( string.IsNullOrEmpty( attribute.ResourceDictionary ) )
            {
                // resolve template from specified assembly
                if ( !string.IsNullOrEmpty( attribute.ElementStyleName ) )
                    boundColumn.ElementStyle = await GetStyleAsync( type.CreatePackUri( attribute.ElementStyleName ) );

                // resolve template from specified assembly
                if ( !boundColumn.IsReadOnly && !string.IsNullOrEmpty( attribute.EditingElementStyleName ) )
                    boundColumn.EditingElementStyle = await GetStyleAsync( type.CreatePackUri( attribute.EditingElementStyleName ) );
            }
            else
            {
                // resolve resource dictionary from specified assembly
                var resourceUri = type.CreatePackUri( attribute.ResourceDictionary );

                // resolve template from resource dictionary
                if ( !string.IsNullOrEmpty( attribute.ElementStyleName ) )
                    boundColumn.ElementStyle = await GetStyleFromResourceAsync( resourceUri, attribute.ElementStyleName );

                // resolve template from resource dictionary
                if ( !column.IsReadOnly && !string.IsNullOrEmpty( attribute.EditingElementStyleName ) )
                    boundColumn.EditingElementStyle = await GetStyleFromResourceAsync( resourceUri, attribute.EditingElementStyleName );
            }
        }

        private static IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>> SortByDisplayIndex( IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>> properties, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( properties != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>>>() != null );

            int i = 0;

            // sort columns by display index
            if ( ( buildOrders & ColumnBuildOrders.Ascending ) == ColumnBuildOrders.Ascending )
            {
                return from property in properties
                       let displayIndex = GetAttribute<DataGridColumnAttribute>( property.Item2 ).DisplayIndex
                       let index = displayIndex < 0 ? i++ : displayIndex
                       orderby index
                       select property;
            }
            else if ( ( buildOrders & ColumnBuildOrders.Descending ) == ColumnBuildOrders.Descending )
            {
                return from property in properties
                       let displayIndex = GetAttribute<DataGridColumnAttribute>( property.Item2 ).DisplayIndex
                       let index = displayIndex < 0 ? i++ : displayIndex
                       orderby index descending
                       select property;
            }

            // unordered
            return properties;
        }

        private static IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>> SortByName( IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>> properties, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( properties != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>>>() != null );

            // sort columns by name
            if ( ( buildOrders & ColumnBuildOrders.Ascending ) == ColumnBuildOrders.Ascending )
            {
                return from property in properties
                       let header = GetAttribute<DataGridColumnAttribute>( property.Item2 ).Header
                       let name = string.IsNullOrEmpty( header ) ? Util.HeaderTextFromPropertyName( property.Item1.Name ) : header
                       orderby name
                       select property;
            }
            else if ( ( buildOrders & ColumnBuildOrders.Descending ) == ColumnBuildOrders.Descending )
            {
                return from property in properties
                       let header = GetAttribute<DataGridColumnAttribute>( property.Item2 ).Header
                       let name = string.IsNullOrEmpty( header ) ? Util.HeaderTextFromPropertyName( property.Item1.Name ) : header
                       orderby name descending
                       select property;
            }

            // unordered
            return properties;
        }

        private static IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>> GetPropertiesInOrder( Type type, ColumnBuildOrders buildOrders )
        {
            Contract.Requires( type != null );
            Contract.Ensures( Contract.Result<IEnumerable<Tuple<PropertyInfo, IEnumerable<Attribute>>>>() != null );

            var list = new List<Tuple<PropertyInfo, IEnumerable<Attribute>>>();

            // enumerate all properties
            foreach ( var property in type.GetRuntimeProperties() )
            {
                // must have the a data grid column definition
                if ( Attribute.IsDefined( property, typeof( DataGridColumnAttribute ), true ) )
                    list.Add( new Tuple<PropertyInfo, IEnumerable<Attribute>>( property, property.GetCustomAttributes( true ).Cast<Attribute>().ToArray() ) );
            }

            if ( ( buildOrders & ColumnBuildOrders.DisplayIndex ) == ColumnBuildOrders.DisplayIndex )
                return SortByDisplayIndex( list, buildOrders );
            else if ( ( buildOrders & ColumnBuildOrders.Name ) == ColumnBuildOrders.Name )
                return SortByName( list, buildOrders );

            // unordered
            return list;
        }

        private static Binding CreateBinding( string propertyName, string adapterPath, IValueConverter converter, DataGridColumnAttribute attribute )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Requires( adapterPath != null );
            Contract.Requires( attribute != null );
            Contract.Ensures( Contract.Result<Binding>() != null );

            var bindingPath = GetBindingPath( propertyName, attribute.BindingPath, adapterPath );

            return new Binding( bindingPath ) {
                Converter = converter,
                FallbackValue = attribute.FallbackValue,
                Mode = attribute.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                StringFormat = attribute.StringFormat,
                TargetNullValue = attribute.TargetNullValue
            };
        }

        private static void SetBinding( DataGridColumn column, Binding binding )
        {
            Contract.Requires( column != null );

            var boundColumn = column as IDataGridBoundColumn;

            if ( boundColumn != null )
                boundColumn.Binding = binding;
        }

        private static void SetOptional( DataGridColumn column, bool optional )
        {
            Contract.Requires( column != null );

            var optionalColumn = column as IOptionalDataGridColumn;

            if ( optionalColumn != null )
                optionalColumn.IsOptional = optional;
        }

        private static void SetTemplateDependencyProperties( DataGridTemplateColumn column, DataGridTemplateColumnAttribute attribute )
        {
            Contract.Requires( column != null );

            var templateColumn = column as OptionalDataGridTemplateColumn;

            if ( templateColumn == null )
                return;

            templateColumn.ContentDependencyProperty = attribute.ContentDependencyProperty;
            templateColumn.EditingContentDependencyProperty = attribute.EditingContentDependencyProperty;
        }

        private async Task<IEnumerable<DataGridColumn>> GetColumnsAsync( Type type, string adapterPath, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Contract.Requires( type != null );
            Contract.Requires( adapterPath != null );
            Contract.Ensures( Contract.Result<Task<IEnumerable<DataGridColumn>>>() != null );

            var columns = new List<DataGridColumn>();

            // enumerate all properties in display index order
            foreach ( var property in GetPropertiesInOrder( type, buildOrders ) )
            {
                // check for cancellation on each property
                cancellationToken.ThrowIfCancellationRequested();

                var propertyName = property.Item1.Name;
                var attribute = GetAttribute<DataGridColumnAttribute>( property.Item2 );
                var template = GetAttribute<DataGridTemplateColumnAttribute>( property.Item2 );
                var converter = GetValueConverter( property.Item2 );

                // default to last display index when unset
                if ( attribute.DisplayIndex == -1 )
                    attribute.DisplayIndex = columns.Count;

                if ( template == null )
                {
                    // no templates are defined so use a text column
                    var column = BuildUpColumn( CreateTextColumn(), attribute, propertyName, adapterPath );
                    var binding = CreateBinding( propertyName, adapterPath, converter, attribute );

                    SetOptional( column, !attribute.IsRequired );
                    SetBinding( column, binding );
                    column.ClipboardContentBinding = binding.Clone();

                    // apply element styles if defined
                    var style = GetAttribute<DataGridElementStyleAttribute>( property.Item2 );

                    if ( style != null )
                        await ApplyStylesAsync( type, column, style );

                    columns.Add( column );
                }
                else
                {
                    // create a templated column
                    var column = BuildUpColumn( CreateTemplateColumn(), attribute, propertyName, adapterPath );
                    var binding = CreateBinding( propertyName, adapterPath, converter, attribute );

                    SetOptional( column, !attribute.IsRequired );
                    SetBinding( column, binding );
                    SetTemplateDependencyProperties( column, template );
                    column.ClipboardContentBinding = binding.Clone();

                    // apply data templates to column
                    await ApplyTemplatesAsync( type, column, template );

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
        public Type TargetType
        {
            get
            {
                return typeof( T );
            }
        }

        /// <summary>
        /// Gets the adapter binding path for the specified model type.
        /// </summary>
        /// <value>The adapter binding path for the model type. The default value is an empty string.</value>
        /// <remarks>This property is only used when the model type is adapted by another type and the binding path
        /// of the adapter must be specified for data binding.</remarks>
        public string AdapterBindingPath
        {
            get
            {
                return adapterBindingPath;
            }
        }

        /// <summary>
        /// Builds up a collection of columns by clearing the specified collection of columns and
        /// appending the columns resolved by the builder asynchronously.
        /// </summary>
        /// <param name="columns">The <see cref="ICollection{T}"/> to append the columns to.</param>
        /// <param name="buildOrders">One or more of the <see cref="ColumnBuildOrders"/> values.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken">token</see> that can be used to cancel the operation.</param>
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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual async Task AppendToAsync( ICollection<DataGridColumn> columns, ColumnBuildOrders buildOrders, CancellationToken cancellationToken )
        {
            Arg.NotNull( columns, nameof( columns ) );
            var columnsToAppend = await GetColumnsAsync( TargetType, AdapterBindingPath, buildOrders, cancellationToken );
            columns.AddRange( columnsToAppend );
        }
    }
}
