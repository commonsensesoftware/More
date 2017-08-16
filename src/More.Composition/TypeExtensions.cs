namespace More.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    static class TypeExtensions
    {
        static readonly TypeInfo ValueTypeInfo = typeof( ValueType ).GetTypeInfo();
        static readonly TypeInfo EnumTypeInfo = typeof( Enum ).GetTypeInfo();
        static readonly Type NullableType = typeof( Nullable<> );

        internal static bool IsAssignableFrom( this Type sourceType, Type targetType ) => sourceType.GetTypeInfo().IsAssignableFrom( targetType.GetTypeInfo() );

        internal static bool IsValueType( this Type type ) => ValueTypeInfo.IsAssignableFrom( type.GetTypeInfo() );

        internal static bool IsNullable( this Type type ) => type.GetTypeInfo().IsNullable();

        internal static bool IsNullable( this Type type, out Type typeArg ) => type.GetTypeInfo().IsNullable( out typeArg );

        internal static bool IsNullable( this TypeInfo typeInfo ) => typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().Equals( NullableType );

        internal static bool IsNullable( this TypeInfo typeInfo, out Type typeArg )
        {
            typeArg = null;

            if ( !typeInfo.IsGenericType || !typeInfo.GetGenericTypeDefinition().Equals( NullableType ) )
            {
                return false;
            }

            typeArg = typeInfo.GenericTypeArguments[0];
            return true;
        }

        internal static bool IsEnum( this Type type )
        {
            Contract.Requires( type != null );

            var typeInfo = type.GetTypeInfo();
            return EnumTypeInfo.IsAssignableFrom( typeInfo ) || ( typeInfo.IsNullable( out var typeArg ) && EnumTypeInfo.IsAssignableFrom( typeArg.GetTypeInfo() ) );
        }
    }
}