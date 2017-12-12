namespace More.Windows.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Provides a value converter that supports converting a <see cref="bool"/> value to a <see cref="DataGridSelectionMode"/> value.
    /// </summary>
    public class DataGridSelectionModeConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified state to the target type.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="object"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="targetType"/> is not type <see cref="string"/>.</exception>
        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( !( value is bool extended ) )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( ( value ?? new object() ).GetType() ) );
            }

            var supported = typeof( object ).Equals( targetType ) || typeof( DataGridSelectionMode ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            return extended ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single;
        }

        /// <summary>
        /// Converts a previously converted state back to the specified type.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="object"/>.</returns>
        public virtual object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( !( value is DataGridSelectionMode mode ) )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedValueType.FormatDefault( ( value ?? new object() ).GetType() ) );
            }

            var supported = typeof( object ).Equals( targetType ) || typeof( bool ).Equals( targetType );

            if ( !supported )
            {
                throw new ArgumentException( ExceptionMessage.UnsupportedConversionType.FormatDefault( targetType ), nameof( targetType ) );
            }

            return mode == DataGridSelectionMode.Extended;
        }
    }
}