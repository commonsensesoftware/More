namespace More.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Represents an optional data grid template column.
    /// </summary>
    public class OptionalDataGridTemplateColumn : DataGridTemplateColumn, IOptionalDataGridColumn, IDataGridBoundColumn
    {
        string contentProperty;
        string editingContentProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalDataGridTemplateColumn"/> class.
        /// </summary>
        public OptionalDataGridTemplateColumn() => IsOptional = true;

        /// <summary>
        /// Gets or sets the name of the dependency property in the content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="DependencyProperty"/> to apply the <see cref="DataGridBoundColumn.Binding"/> property in the content returned
        /// in the <see cref="DataTemplate"/> provided by the <see cref="DataGridTemplateColumn.CellTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="DependencyProperty"/> is found on the content <see cref="FrameworkElement"/>
        /// generated for the cell, the <see cref="DependencyProperty"/> and <see cref="DataGridBoundColumn.Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="DataTemplate"/> for a column can be reused.</remarks>
        public string ContentDependencyProperty
        {
            get => contentProperty;
            set
            {
                if ( StringComparer.Ordinal.Equals( contentProperty, value ) )
                {
                    return;
                }

                contentProperty = value;
                NotifyPropertyChanged( nameof( ContentDependencyProperty ) );
            }
        }

        /// <summary>
        /// Gets or sets the name of the dependency property in the editing content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="DependencyProperty"/> to apply the <see cref="DataGridBoundColumn.Binding"/> property in the content returned
        /// in the <see cref="DataTemplate"/> provided by the <see cref="DataGridTemplateColumn.CellEditingTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="DependencyProperty"/> is found on the content <see cref="FrameworkElement"/>
        /// generated for the cell, the <see cref="DependencyProperty"/> and <see cref="DataGridBoundColumn.Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="DataTemplate"/> for a column can be reused.</remarks>
        public string EditingContentDependencyProperty
        {
            get => contentProperty;
            set
            {
                if ( StringComparer.Ordinal.Equals( editingContentProperty, value ) )
                {
                    return;
                }

                editingContentProperty = value;
                NotifyPropertyChanged( nameof( EditingContentDependencyProperty ) );
            }
        }

        static IEnumerable<Type> GetTypeIterator( Type baseType )
        {
            Contract.Requires( baseType != null );
            Contract.Ensures( Contract.Result<IEnumerable<Type>>() != null );
            Contract.Ensures( Contract.ForAll( Contract.Result<IEnumerable<Type>>(), t => t != null ) );

            var type = baseType;

            while ( type != null && type != typeof( object ) )
            {
                yield return type;
                type = type.BaseType;
            }
        }

        static void ApplyDataBinding( FrameworkElement element, string dependencyProperty, BindingBase binding )
        {
            Contract.Requires( element != null );
            Contract.Requires( !string.IsNullOrEmpty( dependencyProperty ) );
            Contract.Requires( binding != null );

            var fields = from type in GetTypeIterator( element.GetType() )
                         from dp in type.GetFields()
                         where dp.IsStatic &&
                               dp.IsInitOnly &&
                               dp.FieldType == typeof( DependencyProperty ) &&
                               StringComparer.Ordinal.Equals( dp.Name, dependencyProperty )
                         select dp;
            var field = fields.FirstOrDefault();

            if ( field == null )
            {
                var message = ExceptionMessage.MissingDependencyProperty.FormatDefault( element.GetType(), dependencyProperty );
                System.Diagnostics.Debug.WriteLine( message );
                System.Console.WriteLine( message );
                return;
            }

            element.SetBinding( (DependencyProperty) field.GetValue( null ), binding );
        }

        /// <summary>
        /// Gets an element defined by the <see cref="DataGridTemplateColumn.CellEditingTemplate"/> that is bound to the column's <see cref="DataGridBoundColumn.Binding"/> property value.
        /// </summary>
        /// <param name="cell">The <see cref="DataGridCell">cell</see> that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new editing element that is bound to the column's <see cref="Binding"/> property value.</returns>
        protected override FrameworkElement GenerateEditingElement( DataGridCell cell, object dataItem )
        {
            var element = base.GenerateEditingElement( cell, dataItem );

            if ( !string.IsNullOrEmpty( EditingContentDependencyProperty ) && Binding != null )
            {
                ApplyDataBinding( element, EditingContentDependencyProperty, Binding );
            }

            return element;
        }

        /// <summary>
        /// Gets an element defined by the <see cref="DataGridTemplateColumn.CellTemplate"/> that is bound to the column's <see cref="DataGridBoundColumn.Binding"/> property value.
        /// </summary>
        /// <param name="cell">The <see cref="DataGridCell">cell</see> that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new, read-only element that is bound to the column's <see cref="DataGridBoundColumn.Binding"/> property value.</returns>
        protected override FrameworkElement GenerateElement( DataGridCell cell, object dataItem )
        {
            var element = base.GenerateElement( cell, dataItem );

            if ( !string.IsNullOrEmpty( ContentDependencyProperty ) && Binding != null )
            {
                ApplyDataBinding( element, ContentDependencyProperty, Binding );
            }

            return element;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is optional.
        /// </summary>
        /// <value>True if the column is optional; otherwise, false.  The default value is true.</value>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Gets or sets the binding associated with the data grid column.
        /// </summary>
        /// <value>A <see cref="BindingBase"/> object.</value>
        public BindingBase Binding { get; set; }
    }
}