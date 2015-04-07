namespace More
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;

    /// <content>
    /// Provides implementation for Windows Store apps.
    /// </content>
    internal static partial class ReflectHelper
    {
        private static object ConvertToTargetType( Type destinationType, object value )
        {
            Contract.Requires( destinationType != null );

            if ( value == null )
                return null;

            var sourceType = value.GetType();

            // ensure a conversion is required
            if ( destinationType.Equals( sourceType ) || destinationType.GetTypeInfo().IsAssignableFrom( sourceType.GetTypeInfo() ) )
                return value;

            // fallback to IConvertible implementation
            return Convert.ChangeType( value, destinationType, CultureInfo.CurrentCulture );
        }

        internal static object ConvertToTargetType( MemberInfo member, Type destinationType, object value )
        {
            Contract.Requires( member != null );
            Contract.Requires( destinationType != null );
            return ConvertToTargetType( destinationType, value );
        }

        internal static object ConvertToTargetType( ParameterInfo parameter, Type destinationType, object value )
        {
            Contract.Requires( parameter != null );
            Contract.Requires( destinationType != null );
            return ConvertToTargetType( destinationType, value );
        }
    }
}
