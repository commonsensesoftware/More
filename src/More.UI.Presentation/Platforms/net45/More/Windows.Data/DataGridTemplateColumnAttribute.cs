namespace More.Windows.Data
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents the metadata used to locate a resource-based data template.
    /// </summary>
    /// <remarks>The resource specified must be in the current <see cref="Application"/>.</remarks>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public sealed class DataGridTemplateColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the resource dictionary that contains the data templates.
        /// </summary>
        /// <value>The name of the resource dictionary.  This property can be null.</value>
        public string ResourceDictionary { get; set; }

        /// <summary>
        /// Gets or sets the name of the standard cell data template.
        /// </summary>
        /// <value>The name of the standard cell data template.</value>
        /// <remarks>If the <see cref="ResourceDictionary"/> property is null or an empty string, then this property
        /// is assumed to be the name of a resource that only contains a <see cref="DataTemplate"/>; otherwise, this
        /// property is the key for the <see cref="DataTemplate"/> in the corresponding <see cref="ResourceDictionary"/>.</remarks>
        public string CellTemplateName { get; set; }

        /// <summary>
        /// Gets or sets the name of the edit cell data template.
        /// </summary>
        /// <value>The name of the edit cell data template.  This property can be null or an empty string if the column is read-only.</value>
        /// <remarks>If the <see cref="ResourceDictionary"/> property is null or an empty string, then this property
        /// is assumed to be the name of a resource that only contains a <see cref="DataTemplate"/>; otherwise, this
        /// property is the key for the <see cref="DataTemplate"/> in the corresponding <see cref="ResourceDictionary"/>.</remarks>
        public string CellEditingTemplateName { get; set; }

        /// <summary>
        /// Gets or sets the name of the dependency property in the content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="DependencyProperty"/> to apply the <see cref="DataGridBoundColumn.Binding"/> property in the content returned
        /// in the <see cref="DataTemplate"/> provided by the <see cref="DataGridTemplateColumn.CellTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="DependencyProperty"/> is found on the content <see cref="FrameworkElement"/>
        /// generated for the cell, the <see cref="DependencyProperty"/> and <see cref="DataGridBoundColumn.Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="DataTemplate"/> for a column can be reused.</remarks>
        public string ContentDependencyProperty { get; set; }

        /// <summary>
        /// Gets or sets the name of the dependency property in the editing content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="DependencyProperty"/> to apply the <see cref="DataGridBoundColumn.Binding"/> property in the content returned
        /// in the <see cref="DataTemplate"/> provided by the <see cref="DataGridTemplateColumn.CellEditingTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="DependencyProperty"/> is found on the content <see cref="FrameworkElement"/>
        /// generated for the cell, the <see cref="DependencyProperty"/> and <see cref="DataGridBoundColumn.Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="DataTemplate"/> for a column can be reused.</remarks>
        public string EditingContentDependencyProperty { get; set; }
    }
}