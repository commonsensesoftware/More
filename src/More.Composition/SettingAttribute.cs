namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the metadata for an application setting.
    /// </summary>
    [CLSCompliant( false )]
    [MetadataAttribute]
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Parameter )]
    public sealed class SettingAttribute : ImportAttribute
    {
        readonly static Lazy<IDictionary<Type, ITypeConverter>> mappedConverters = new Lazy<IDictionary<Type, ITypeConverter>>( CreateDefaultConverters );
        readonly static object syncRoot = new object();
        object defaultValue = NullValue;

        /// <summary>
        /// Gets a value that can be used to represent null for the <see cref="P:DefaultValue">default value</see>.
        /// </summary>
        /// <value>An <see cref="Object">object</see> that represents <c>null</c>.</value>
        /// <remarks>This field is used to ensure the <see cref="P:DefaultValue"/> property is not actually null,
        /// which can cause an exception in the Managed Extensibility Framework under certain conditions.</remarks>
        public static readonly object NullValue = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute" /> class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        public SettingAttribute( string key ) : base( key ) => Arg.NotNullOrEmpty( key, nameof( key ) );

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <value>The setting key.</value>
        public string Key
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return ContractName;
            }
        }

        /// <summary>
        /// Gets or sets the default value of the setting.
        /// </summary>
        /// <value>The setting default value.</value>
        public object DefaultValue
        {
            get
            {
                Contract.Ensures( defaultValue != null );
                return defaultValue;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                defaultValue = value;
            }
        }

        static IDictionary<Type, ITypeConverter> CreateDefaultConverters()
        {
            var guidConverter = new GuidConverter();
            var timeSpanConverter = new TimeSpanConverter();

            return new Dictionary<Type, ITypeConverter>()
            {
                { typeof( object ), new DefaultConverter() },
                { typeof( Guid ), guidConverter },
                { typeof( Guid? ), guidConverter },
                { typeof( Uri ), new UriConverter() },
                { typeof( TimeSpan ), timeSpanConverter },
                { typeof( TimeSpan? ), timeSpanConverter },
                { typeof( Enum ), new EnumConverter() }
            };
        }

        /// <summary>
        /// Adds or replaces a conversion function for the specified value.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value to convert to.</typeparam>
        /// <param name="converter">The converter <see cref="Func{T1,T2,T3,TResult}">function</see>.</param>
        /// <remarks>Default converters are provided for the following types: <see cref="Guid"/>, <see cref="Uri"/>,
        /// <see cref="TimeSpan"/>, and <see cref="Enum"/>. A default converter is also provided to convert all
        /// primitive types. If a <paramref name="converter"/> is provided that maps to <see cref="Enum"/>, it will
        /// become the default converter for all enumerations. If a <paramref name="converter"/> is provided that
        /// maps to <see cref="Object"/>, it will become the fallback converter for all conversions.</remarks>
        public static void SetConverter<TValue>( Func<object, Type, IFormatProvider, TValue> converter )
        {
            Arg.NotNull( converter, nameof( converter ) );

            lock ( syncRoot )
            {
                mappedConverters.Value[typeof( TValue )] = new UserDefinedConverter<TValue>( converter );
            }
        }

        /// <summary>
        /// Converts the specified value to the requested target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target <see cref="Type">type</see> to convert the value to.</param>
        /// <param name="formatProvider">The <see cref="IFormatProvider">format provider</see> used to convert the value.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>Default converters are provided for the following types: <see cref="Guid"/>, <see cref="Uri"/>,
        /// <see cref="TimeSpan"/>, and <see cref="Enum"/>. A default converter is provided to convert the built-in
        /// primitive types. The applicable <see cref="Nullable{T}">nullable</see> variants of each of these types
        /// is also supported. To register additional conversion methods, use the <see cref="SetConverter{T}"/> method.</remarks>
        public static object Convert( object value, Type targetType, IFormatProvider formatProvider )
        {
            Arg.NotNull( targetType, nameof( targetType ) );

            var converters = default( IDictionary<Type, ITypeConverter> );
            var converter = default( ITypeConverter );

            lock ( syncRoot )
            {
                converters = mappedConverters.Value;

                if ( converters.TryGetValue( targetType, out converter ) )
                {
                    return converter.Convert( value, targetType, formatProvider );
                }
            }

            var key = targetType.IsEnum() ? typeof( Enum ) : typeof( object );

            lock ( syncRoot )
            {
                converter = converters[key];
            }

            return converter.Convert( value, targetType, formatProvider );
        }
    }
}