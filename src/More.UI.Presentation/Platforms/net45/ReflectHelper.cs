namespace More
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    internal static partial class ReflectHelper
    {
        private static TypeConverter GetTypeConverter( TypeConverterAttribute attribute )
        {
            // use type converter if defined
            if ( attribute == null || string.IsNullOrEmpty( attribute.ConverterTypeName ) )
                return null;

            var converterType = Type.GetType( attribute.ConverterTypeName, true );
            var converter = (TypeConverter) Activator.CreateInstance( converterType, true );

            return converter;
        }

        private static Func<CultureInfo, object, object> GetTypeConverter( TypeConverterAttribute attribute, Type sourceType, Type destinationType )
        {
            Contract.Requires( attribute != null );
            Contract.Requires( sourceType != null );
            Contract.Requires( destinationType != null );

            // honor type converter on defined member if specified
            var destType = destinationType;
            var converter = GetTypeConverter( attribute );

            if ( converter != null && converter.CanConvertTo( null, destinationType ) )
                return ( c, v ) => converter.ConvertTo( null, c, v, destType );

            // try to get type converter from source type (needs to support conversion to destination)
            converter = TypeDescriptor.GetConverter( sourceType );

            if ( converter != null && converter.CanConvertTo( null, destType ) )
                return ( c, v ) => converter.ConvertTo( null, c, v, destType );

            // try to get type converter from destination type (needs to support conversion from source)
            converter = TypeDescriptor.GetConverter( destinationType );

            if ( converter != null && converter.CanConvertFrom( null, sourceType ) )
                return ( c, v ) => converter.ConvertFrom( null, c, v );

            return null;
        }

        private static object ConvertToTargetType( TypeConverterAttribute attribute, Type destinationType, object value )
        {
            Contract.Requires( attribute != null );
            Contract.Requires( destinationType != null );

            if ( value == null )
                return null;

            var sourceType = value.GetType();

            // ensure a conversion is required
            if ( destinationType.Equals( sourceType ) || destinationType.IsAssignableFrom( sourceType ) )
                return value;

            var convert = GetTypeConverter( attribute, sourceType, destinationType );
            var culture = CultureInfo.CurrentCulture;

            // if the type converter supports the conversion, let it do it's magic
            if ( convert != null )
                return convert( culture, value );

            var typeCode = Type.GetTypeCode( destinationType );

            try
            {
                // fallback to IConvertible implementation
                return Convert.ChangeType( value, typeCode, culture );
            }
            catch ( InvalidCastException innerEx )
            {
                throw new InvalidCastException( ExceptionMessage.InvalidCast.FormatDefault( sourceType, destinationType ), innerEx );
            }
        }

        internal static object ConvertToTargetType( MemberInfo member, Type destinationType, object value )
        {
            Contract.Requires( member != null );
            Contract.Requires( destinationType != null );

            var attribute = member.GetCustomAttribute<TypeConverterAttribute>( true );
            return ConvertToTargetType( attribute, destinationType, value );
        }

        internal static object ConvertToTargetType( ParameterInfo parameter, Type destinationType, object value )
        {
            Contract.Requires( parameter != null );
            Contract.Requires( destinationType != null );

            var attribute = parameter.GetCustomAttribute<TypeConverterAttribute>( true );
            return ConvertToTargetType( attribute, destinationType, value );
        }
    }
}
