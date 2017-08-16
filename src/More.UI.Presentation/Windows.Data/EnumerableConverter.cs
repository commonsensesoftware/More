namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;

    /// <summary>
    /// Represents a format converter for an enumerable sequence of items.
    /// </summary>
    public class EnumerableConverter : FormatConverter
    {
        /// <summary>
        /// Gets or sets the custom format used when the sequence is empty.
        /// </summary>
        /// <value>The empty sequence format.</value>
        public virtual string EmptyFormat { get; set; }

        /// <summary>
        /// Gets or sets the custom format used when the sequence has a single item.
        /// </summary>
        /// <value>The single item format.</value>
        public virtual string SingleItemFormat { get; set; }

        /// <summary>
        /// Gets or sets the name or path of the property that is displayed for each data item.
        /// </summary>
        /// <value>The name or path of the property that is displayed for each the data item.</value>
        /// <remarks>This property is only used when the sequence has a single item.</remarks>
        public virtual string DisplayMemberPath { get; set; }

        string FormatItem( object item, CultureInfo culture )
        {
            if ( item == null )
            {
                return null;
            }

            var itemValue = item;

            // get value of item, if defined
            if ( !string.IsNullOrEmpty( DisplayMemberPath ) )
            {
                itemValue = ReflectHelper.InvokePath( item, DisplayMemberPath );
            }

            // use format for one item, if defined
            if ( string.IsNullOrEmpty( SingleItemFormat ) )
            {
                return itemValue?.ToString();
            }

            return string.Format( culture, SingleItemFormat, itemValue );
        }

#if UAP10_0
        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not type <see cref="String"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not type <see cref="IEnumerable"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive" )]
        public override object Convert( object value, Type targetType, object parameter, string language )
#else
        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not type <see cref="String"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not type <see cref="IEnumerable"/>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive" )]
        public override object Convert( object value, Type targetType, object parameter, CultureInfo culture )
#endif
        {
            var supported = typeof( object ).Equals( targetType ) || typeof( string ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            if ( value == null )
            {
                return DefaultNullValue;
            }

            var sourceType = value.GetType();

            // special handling: System.String implements IEnumerable
            if ( typeof( string ).Equals( targetType ) && typeof( string ).Equals( sourceType ) )
            {
                return value;
            }

            if ( !typeof( IEnumerable ).GetTypeInfo().IsAssignableFrom( sourceType.GetTypeInfo() ) )
            {
                throw new ArgumentException( ExceptionMessage.InvalidTypeException.FormatDefault( typeof( IEnumerable ) ), nameof( value ) );
            }
#if UAP10_0
            var culture = Util.GetCultureFromLanguage( language );
#endif
            var sequence = (IEnumerable) value;
            var count = sequence.Count();

            switch ( count )
            {
                case 0:
                    {
                        return EmptyFormat;
                    }
                case 1:
                    {
                        return FormatItem( sequence.ElementAt( 0 ), culture );
                    }
                default:
                    {
                        if ( string.IsNullOrEmpty( Format ) )
                        {
                            return count.ToString( culture );
                        }

                        return string.Format( culture, Format, count );
                    }
            }
        }
    }
}