namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Reflection;

    internal sealed class ValueTypeInfo<T>
    {
        internal readonly TypeInfo TypeInfo = typeof( T ).GetTypeInfo();
        internal readonly bool IsValueType = default( T ) != null;
    }
}
