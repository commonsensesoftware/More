namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Represents a <see cref="IValueConverter">value converter</see> that can convert from a
    /// <see cref="ISpecification{T}">specification</see> to a <see cref="Func{T,TResult}">function</see>.
    /// </summary>
    public sealed class SpecificationToFunctionConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value to a function.
        /// </summary>
        /// <param name="value">The <see cref="ISpecification{T}">specification</see> to convert.</param>
        /// <param name="targetType">The target <see cref="Type">type</see>; only <see cref="Func{T,TResult}"/> is supported.</param>
        /// <param name="parameter">A user-defined conversion parameter. This parameter is not used.</param>
        /// <param name="culture">The <see cref="CultureInfo">culture</see> used to perform the conversion.</param>
        /// <returns>A converted <see cref="Func{T,TResult}"/> object.</returns>
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            var specification = (ISpecification<Type>) value;
            Func<Type, bool> function = specification.IsSatisfiedBy;
            return function;
        }

        object IValueConverter.ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotSupportedException();
        }
    }
}
