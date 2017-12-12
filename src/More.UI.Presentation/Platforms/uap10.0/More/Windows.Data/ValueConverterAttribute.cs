namespace More.Windows.Data
{
    using global::Windows.UI.Xaml.Data;
    using System;

    /// <content>
    /// Provides additional implementation for Windows Runtime applications.
    /// </content>
    public sealed partial class ValueConverterAttribute : IValueConverter
    {
        /// <summary>
        /// Converts the specified state to the target type.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="object"/>.</returns>
        public object Convert( object value, Type targetType, object parameter, string language ) =>
            Converter.Convert( value, targetType, parameter, language );

        /// <summary>
        /// Converts a previously converted state back to the specified type.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="object"/>.</returns>
        public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
            Converter.ConvertBack( value, targetType, parameter, language );
    }
}