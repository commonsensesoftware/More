namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
#if UAP10_0
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Represents the base implementation for an attribute that encapsulates a value converter.
    /// </summary>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    public abstract partial class ValueConverterBaseAttribute : Attribute
    {
        readonly IValueConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterBaseAttribute"/> class.
        /// </summary>
        protected ValueConverterBaseAttribute() : this( new NullValueConverter() ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterBaseAttribute"/> class.
        /// </summary>
        /// <param name="converter">The <see cref="IValueConverter"/> encapsulated by the attribute.</param>
        protected ValueConverterBaseAttribute( IValueConverter converter )
        {
            Arg.NotNull( converter, nameof( converter ) );
            this.converter = converter;
        }

        /// <summary>
        /// Gets the value converter encapsulated by the attribute.
        /// </summary>
        /// <value>An <see cref="IValueConverter"/> object.</value>
        protected virtual IValueConverter Converter
        {
            get
            {
                Contract.Ensures( Contract.Result<IValueConverter>() != null );
                return converter;
            }
        }
    }
}