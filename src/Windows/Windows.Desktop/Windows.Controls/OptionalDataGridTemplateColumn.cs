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
        private string contentProperty;
        private string editingContentProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalDataGridTemplateColumn"/> class.
        /// </summary>
        public OptionalDataGridTemplateColumn()
        {
            IsOptional = true;
        }

        /// <summary>
        /// Gets or sets the name of the dependency property in the content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="T:DependencyProperty"/> to apply the <see cref="P:Binding"/> property in the content returned
        /// in the <see cref="T:DataTemplate"/> provided by the <see cref="P:CellTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="T:DependencyProperty"/> is found on the content <see cref="T:FrameworkElement"/>
        /// generated for the cell, the <see cref="T:DependencyProperty"/> and <see cref="P:Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="T:DataTemplate"/> for a column can be reused.</remarks>
        public string ContentDependencyProperty
        {
            get
            {
                return contentProperty;
            }
            set
            {
                if ( StringComparer.Ordinal.Equals( contentProperty, value ) )
                    return;

                contentProperty = value;
                NotifyPropertyChanged( "ContentDependencyProperty" );
            }
        }

        /// <summary>
        /// Gets or sets the name of the dependency property in the editing content data template to apply the column binding to.
        /// </summary>
        /// <value>The name of the <see cref="T:DependencyProperty"/> to apply the <see cref="P:Binding"/> property in the content returned
        /// in the <see cref="T:DataTemplate"/> provided by the <see cref="P:EditingCellTemplate"/> property.  This property can be null or an empty string.
        /// The default value is null.</value>
        /// <remarks>When a value is specified and a matching <see cref="T:DependencyProperty"/> is found on the content <see cref="T:FrameworkElement"/>
        /// generated for the cell, the <see cref="T:DependencyProperty"/> and <see cref="P:Binding"/> properties are paired.  This provides the ability to
        /// dynamically wire data binding so that a <see cref="T:DataTemplate"/> for a column can be reused.</remarks>
        public string EditingContentDependencyProperty
        {
            get
            {
                return contentProperty;
            }
            set
            {
                if ( StringComparer.Ordinal.Equals( editingContentProperty, value ) )
                    return;

                editingContentProperty = value;
                NotifyPropertyChanged( "EditingContentDependencyProperty" );
            }
        }

        private static IEnumerable<Type> GetTypeIterator( Type baseType )
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

        private static void ApplyDataBinding( FrameworkElement element, string dependencyProperty, BindingBase binding )
        {
            Contract.Requires( element != null );
            Contract.Requires( !string.IsNullOrEmpty( dependencyProperty ) );
            Contract.Requires( binding != null );

            // find matching dependency property (search local type and all base types)
            var fields = from type in GetTypeIterator( element.GetType() )
                         from dp in type.GetFields()
                         where dp.IsStatic &&
                               dp.IsInitOnly &&
                               dp.FieldType == typeof( DependencyProperty ) &&
                               StringComparer.Ordinal.Equals( dp.Name, dependencyProperty )
                         select dp;
            var field = fields.FirstOrDefault();

            // the dependency property wasn't found
            if ( field == null )
            {
                var message = ExceptionMessage.MissingDependencyProperty.FormatDefault( element.GetType(), dependencyProperty );
                System.Diagnostics.Debug.WriteLine( message );
                System.Console.WriteLine( message );
                return;
            }

            // dynamically data bind
            element.SetBinding( (DependencyProperty) field.GetValue( null ), binding );
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:CellEditingTemplate"/> that is bound to the column's <see cref="P:Binding"/> property value.
        /// </summary>
        /// <param name="cell">The <see cref="T:DataGridCell">cell</see> that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new editing element that is bound to the column's <see cref="P:Binding"/> property value.</returns>
        protected override FrameworkElement GenerateEditingElement( DataGridCell cell, object dataItem )
        {
            var element = base.GenerateEditingElement( cell, dataItem );

            if ( !string.IsNullOrEmpty( EditingContentDependencyProperty ) && Binding != null )
                ApplyDataBinding( element, EditingContentDependencyProperty, Binding );

            return element;
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:CellTemplate"/> that is bound to the column's <see cref="P:Binding"/> property value.
        /// </summary>
        /// <param name="cell">The <see cref="T:DataGridCell">cell</see> that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <returns>A new, read-only element that is bound to the column's <see cref="P:Binding"/> property value.</returns>
        protected override FrameworkElement GenerateElement( DataGridCell cell, object dataItem )
        {
            var element = base.GenerateElement( cell, dataItem );

            if ( !string.IsNullOrEmpty( ContentDependencyProperty ) && Binding != null )
                ApplyDataBinding( element, ContentDependencyProperty, Binding );

            return element;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is optional.
        /// </summary>
        /// <value>True if the column is optional; otherwise, false.  The default value is true.</value>
        public bool IsOptional
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the binding associated with the data grid column.
        /// </summary>
        /// <value>A <see cref="BindingBase"/> object.</value>
        public BindingBase Binding
        {
            get;
            set;
        }
    }
}
