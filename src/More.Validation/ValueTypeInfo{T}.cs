namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Reflection;

    sealed class ValueTypeInfo<T>
    {
#pragma warning disable SA1401 // Fields should be private
        internal readonly TypeInfo TypeInfo = typeof( T ).GetTypeInfo();
        internal readonly bool IsValueType = default( T ) != null;
#pragma warning restore SA1401 // Fields should be private
    }
}