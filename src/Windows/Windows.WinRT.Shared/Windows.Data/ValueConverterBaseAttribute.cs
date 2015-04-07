namespace More.Windows.Data
{
    using System;
    using global::Windows.UI.Xaml.Data;

    /// <content>
    /// Provides the implementation for Windows Store apps.
    /// </content>
    public abstract partial class ValueConverterBaseAttribute : IValueConverter
    {
        private sealed class NullValueConverter : IValueConverter
        {
            public object Convert( object value, Type targetType, object parameter, string language )
            {
                return value;
            }

            public object ConvertBack( object value, Type targetType, object parameter, string language )
            {
                return value;
            }
        }

        /// <summary>
        /// Converts the specified state to the target type.
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        public object Convert( object value, Type targetType, object parameter, string language )
        {
            return this.Converter.Convert( value, targetType, parameter, language );
        }

        /// <summary>
        /// Converts a previously converted state back to the specified type. 
        /// </summary>
        /// <param name="value">The <see cref="Object"/> to be converted.</param>
        /// <param name="targetType">The destination conversion <see cref="Type"/>.</param>
        /// <param name="parameter">An <see cref="Object"/> containing custom conversion parameters.</param>
        /// <param name="language">The language used for formatting.</param>
        /// <returns>The converted <see cref="Object"/>.</returns>
        public object ConvertBack( object value, Type targetType, object parameter, string language )
        {
            return this.Converter.ConvertBack( value, targetType, parameter, language );
        }
    }
}
