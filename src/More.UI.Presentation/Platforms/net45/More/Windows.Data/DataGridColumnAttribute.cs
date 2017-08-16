namespace System.Windows.Data
{
    using System;
    using System.Windows;

    /// <summary>
    /// Represents the metadata for a data grid column definition.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public sealed class DataGridColumnAttribute : Attribute
    {
        /// <summary>
        /// Gets the value used to indicate a column name is unspecified.
        /// </summary>
        /// <value>The value of an unspecified column name.</value>
        public const string Unspecified = "\0";

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnAttribute"/> class.
        /// </summary>
        public DataGridColumnAttribute()
        {
            CanReorder = true;
            CanResize = true;
            CanSort = true;
            DisplayIndex = -1;
            MaxWidth = double.PositiveInfinity;
            Width = double.NaN;
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets or sets the column header.
        /// </summary>
        /// <value>The column header.</value>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is required.
        /// </summary>
        /// <value>True if the column is required; otherwise, false.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can change the column
        /// display position by dragging the column header.
        /// </summary>
        /// <value>True if the user can drag the column header to a new position; otherwise, false.</value>
        public bool CanReorder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can adjust the column width
        /// using the mouse.
        /// </summary>
        /// <value>True if the user can resize the column; false if the user cannot resize the column.</value>
        public bool CanResize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can sort the column by clicking
        /// the column header.
        /// </summary>
        /// <value>True if the user can sort the column; otherwise, false.</value>
        public bool CanSort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cells in the column can be edited.
        /// </summary>
        /// <value>True if cells in the column cannot be edited; otherwise, false. The default is false.</value>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the maximum column width in pixels.
        /// </summary>
        /// <value>The maximum column width in pixels. The default is <see cref="double.PositiveInfinity"/>.</value>
        public double MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the minimum column width in pixels.
        /// </summary>
        /// <value>The minimum column width in pixels, or 0 if the value is not set. The default is 0.</value>
        public double MinWidth { get; set; }

        /// <summary>
        /// Gets or sets the column width.
        /// </summary>
        /// <value>The width of the column.  The default value is <see cref="double.NaN"/> which indicates automatical width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column width uses header-based automatic sizing.
        /// </summary>
        /// <value>True if the column uses header-based automatic sizing; otherwise, false.</value>
        public bool SizeToHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column width uses cell-based automatic sizing.
        /// </summary>
        /// <value>True if the column uses cell-based automatic sizing; otherwise, false.</value>
        public bool SizeToCells { get; set; }

        /// <summary>
        /// Gets or sets the column display index.
        /// </summary>
        /// <value>The column display index.  The default value is -1, which is interpretted as the last column
        /// and is useful for optional columns that are not typically displayed.</value>
        public int DisplayIndex { get; set; }

        /// <summary>
        /// Gets or sets the column visibility.
        /// </summary>
        /// <value>One of the <see cref="Visibility"/> values.  The default value is
        /// <see cref="Visibility.Visible"/>.</value>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Gets or sets the member binding path for the column.
        /// </summary>
        /// <value>The member binding path for the column.  The default value is used is the name of the property
        /// the attribute is defined for.</value>
        /// <remarks>This property is useful for data binding the path of property that is a complex object.</remarks>
        public string BindingPath { get; set; }

        /// <summary>
        /// Gets or sets the member binding path to sort the column.
        /// </summary>
        /// <value>The member binding path to sort the column.  The default value is null,
        /// which indicates the default sorting behavior.</value>
        public string SortMemberPath { get; set; }

        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        /// <value>The value to use when the binding is unable to return a value. The default is null.</value>
        public object FallbackValue { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
        /// </summary>
        /// <value>A string that specifies how to format the binding if it displays the bound value as a string. The default is null.</value>
        public string StringFormat { get; set; }

        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source is null.
        /// </summary>
        /// <value>The value that is used in the target when the value of the source is null.</value>
        public object TargetNullValue { get; set; }
    }
}