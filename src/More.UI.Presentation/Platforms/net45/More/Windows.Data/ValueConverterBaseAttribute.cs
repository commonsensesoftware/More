namespace More.Windows.Data
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <content>
    /// Provides additional implementation for Windows Desktop applications.
    /// </content>
    public abstract partial class ValueConverterBaseAttribute : IValueConverter
    {
        sealed class NullValueConverter : IValueConverter
        {
            public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) => value;

            public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) => value;
        }

        /// <summary>
        /// Converts the specified state to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) =>
            Converter.Convert( value, targetType, parameter, culture );

        /// <summary>
        /// Converts a previously converted state back to the specified type. 
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) =>
            Converter.ConvertBack( value, targetType, parameter, culture );
    }
}