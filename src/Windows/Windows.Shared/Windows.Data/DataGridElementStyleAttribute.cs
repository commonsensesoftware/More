namespace System.Windows.Data
{
    using System;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Represents the metadata used to locate a resource-based data grid element style.
    /// </summary>
    /// <remarks>The resource specified must be in the current <see cref="Application"/>.</remarks>
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public sealed class DataGridElementStyleAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the resource dictionary that contains the styles.
        /// </summary>
        /// <value>The name of the resource dictionary.  This property can be null.</value>
        public string ResourceDictionary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the standard element style.
        /// </summary>
        /// <value>The name of the standard element style.</value>
        /// <remarks>If the <see cref="P:ResourceDictionary"/> property is null or an empty string, then this property
        /// is assumed to be the name of a resource that only contains a <see cref="T:Style"/>; otherwise, this
        /// property is the key for the <see cref="T:Style"/> in the corresponding <see cref="T:ResourceDictionary"/>.</remarks>
        public string ElementStyleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the edit element style.
        /// </summary>
        /// <value>The name of the edit element style.  This property can be null or an empty string if the column is read-only.</value>
        /// <remarks>If the <see cref="P:ResourceDictionary"/> property is null or an empty string, then this property
        /// is assumed to be the name of a resource that only contains a <see cref="T:Style"/>; otherwise, this
        /// property is the key for the <see cref="T:Style"/> in the corresponding <see cref="T:ResourceDictionary"/>.</remarks>
        public string EditingElementStyleName
        {
            get;
            set;
        }
    }
}
