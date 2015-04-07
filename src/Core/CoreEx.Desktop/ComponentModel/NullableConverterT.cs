namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Provides automatic conversion between a nullable type and its underlying primitive type.
    /// </summary>
    /// <typeparam name="T">The primitive <see cref="Type">type</see> the converter is for.</typeparam>
    public class NullableConverter<T> : TypeConverter where T : struct
    {
        /// <summary>
        /// Gets the nullable type.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type NullableType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null ); 
                return typeof( T? );
            }
        }

        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type UnderlyingType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null ); 
                return typeof( T );
            }
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        /// <returns>True if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <returns>An <see cref="Object"/> that represents the converted value.</returns>
        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( value is T || value is T? )
                return (T?) value;

            var str = value as string;

            if ( str != null || value == null )
                return !string.IsNullOrEmpty( str ) ? new T?( (T) Convert.ChangeType( value, typeof( T ), culture ) ) : null;

            return new T?( (T) Convert.ChangeType( value, typeof( T ), culture ) );
        }
    }
}
