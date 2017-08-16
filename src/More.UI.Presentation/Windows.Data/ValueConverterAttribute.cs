namespace More.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;
#if UAP10_0
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Represents a value converter defined as an <see cref="Attribute">attribute</see>.
    /// </summary>
    [SuppressMessage( "Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Required to support a CLS-compliant attribute." )]
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public sealed partial class ValueConverterAttribute : Attribute
    {
        readonly List<object> ctorArgs = new List<object>();
        Type type;
        IValueConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterAttribute"/> class.
        /// </summary>
        public ValueConverterAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterAttribute"/> class.
        /// </summary>
        /// <param name="converterType">The <see cref="Type"/> of <see cref="IValueConverter"/> to create.</param>
        public ValueConverterAttribute( Type converterType )
        {
            Arg.NotNull( converterType, nameof( converterType ) );
            type = converterType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterAttribute"/> class.
        /// </summary>
        /// <param name="converterType">The <see cref="Type"/> of <see cref="IValueConverter"/> to create.</param>
        /// <param name="constructorArguments">An array of type <see cref="Object">object</see> containing the constructor arguments for the value converter.</param>
        [CLSCompliant( false )]
        public ValueConverterAttribute( Type converterType, params object[] constructorArguments )
            : this( converterType ) => ctorArgs.AddRange( constructorArguments );

        /// <summary>
        /// Gets or sets the type of converter.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type ConverterType
        {
            get => type;
            set
            {
                Arg.NotNull( value, nameof( value ) );
                type = value;
            }
        }

        /// <summary>
        /// Gets or sets the constructor arguments for the type of converter.
        /// </summary>
        /// <value>An array of type <see cref="Object">object</see> that matches the order and type of parameters
        /// supplied to the target constructor.</value>
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required to support a CLS-compliant attribute. The backing field is immutable." )]
        public object[] ConstructorArguments
        {
            get
            {
                Contract.Ensures( ctorArgs != null );
                return ctorArgs.ToArray();
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                ctorArgs.ReplaceAll( value );
            }
        }

        /// <summary>
        /// Gets or sets the property initializers for the type of converter.
        /// </summary>
        /// <value>A string of comma-separated key/value pairs that map properties names to literal values.  Values are parsed according to
        /// the type of property they map to.  Nested property initializers and non-primitive values are not supported.</value>
        /// <example>This example demonstrates how to define property initializers for the <see cref="IValueConverter"/> defined by the attribute.
        /// <code lang="C#"><![CDATA[
        /// using System.Windows.Data;
        /// using System;
        /// 
        /// public class MyClass
        /// {
        ///     [ValueConverter( Type = typeof( MyCurrencyConverter ), PropertyInitializers = "ThousandsFormat = '0 k', MillionsFormat = '0 m', BillionsFormat = '0 b'" )]
        ///     public decimal Amount
        ///     {
        ///         get;
        ///         set;
        ///     }
        /// }
        /// ]]></code></example>
        public string PropertyInitializers { get; set; }

        IValueConverter Converter
        {
            get
            {
                Contract.Ensures( converter != null );

                if ( converter == null )
                {
                    var converterType = ConverterType;

                    if ( converterType == null )
                    {
                        throw new InvalidOperationException( ExceptionMessage.AttributePropertyUnset.FormatDefault( nameof( ConverterType ) ) );
                    }
                    else if ( !IsValidConverterType( converterType ) )
                    {
                        throw new InvalidOperationException( ExceptionMessage.InvalidValueConverterType.FormatDefault( converterType, typeof( IValueConverter ) ) );
                    }

                    var instance = Activator.CreateInstance( converterType, ConstructorArguments );
                    ApplyPropertyInitializers( instance, PropertyInitializers );
                    converter = (IValueConverter) instance;
                }

                return converter;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified type is a valid value converter.
        /// </summary>
        /// <param name="converterType">The <see cref="Type"/> to be evaluated.</param>
        /// <returns>True if the specified type is a valid value converter; otherwise, false.</returns>
        [Pure]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static bool IsValidConverterType( Type converterType )
        {
            Arg.NotNull( converterType, nameof( converterType ) );
#if UAP10_0
            var type = converterType.GetTypeInfo();
            return type.IsPublic && !type.IsAbstract && typeof( IValueConverter ).GetTypeInfo().IsAssignableFrom( type );
#else
            return converterType.IsPublic && !converterType.IsAbstract && typeof( IValueConverter ).IsAssignableFrom( converterType );
#endif
        }

        static void ApplyPropertyInitializers( object target, string propertyInitializers )
        {
            Contract.Requires( target != null );

            if ( string.IsNullOrEmpty( propertyInitializers ) )
            {
                return;
            }

            const string ParsePattern = "\\b([a-zA-Z]\\w*)\\x20*=\\x20*(['\"])([^'\"]*)\\2\\x20*(?:,|$)|\\b([a-zA-Z]\\w*)\\x20*=\\x20*([^\\s,]+)\\x20*(?:,|$)";

            var type = target.GetType();
            var match = Regex.Match( propertyInitializers, ParsePattern, RegexOptions.Singleline );

            while ( match.Success )
            {
                // take quoted value first
                var propertyName = match.Groups[1].Value;
                var valueIndex = 3;

                if ( string.IsNullOrEmpty( propertyName ) )
                {
                    // assume this isn't a quoted key/value pair; use second group
                    propertyName = match.Groups[4].Value;
                    valueIndex = 5;
                }

#if UAP10_0
                var property = type.GetRuntimeProperty( propertyName );
#else
                var property = type.GetProperty( propertyName );
#endif
                var formatProvider = CultureInfo.CurrentCulture;

                if ( property == null )
                {
#if UAP10_0
                    throw new MissingMemberException( string.Format( formatProvider, ExceptionMessage.MissingMemberException, type.FullName, propertyName ) );
#else
                    throw new MissingMemberException( type.FullName, propertyName );
#endif
                }

                // parse to property value and assign value
                var value = System.Convert.ChangeType( match.Groups[valueIndex].Value, property.PropertyType, formatProvider );
                property.SetValue( target, value, null );

                match = match.NextMatch();
            }
        }
    }
}