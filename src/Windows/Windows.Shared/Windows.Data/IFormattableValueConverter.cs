namespace More.Windows.Data
{
    using System;
#if NETFX_CORE
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Defines the behavior of a formatted value converter.
    /// </summary>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public interface IFormattableValueConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the value format.
        /// </summary>
        /// <value>The value format.</value>
        string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default value used to format null values.
        /// </summary>
        /// <value>The default value for null values.</value>
        string DefaultNullValue
        {
            get;
            set;
        }
    }
}
