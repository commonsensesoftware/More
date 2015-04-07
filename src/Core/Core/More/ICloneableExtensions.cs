namespace More
{
    using Microsoft.CSharp.RuntimeBinder;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    internal static class ICloneableExtensions
    {
        private sealed class CloneableBridge : ICloneable
        {
            private readonly object instance;
            private readonly TypeInfo instanceType;
            private readonly Type cloneableType;

            internal CloneableBridge( object instance, TypeInfo instanceType, Type cloneableType )
            {
                Contract.Requires( instance != null );
                Contract.Requires( instanceType != null );
                Contract.Requires( cloneableType != null );

                this.instance = instance;
                this.instanceType = instanceType;
                this.cloneableType = cloneableType;
            }

            public object Clone()
            {
                dynamic cloneable = this.instance;

                try
                {
                    // invoke Clone using dynamic runtime binding and dispatch
                    return cloneable.Clone();
                }
                catch ( RuntimeBinderException )
                {
                    // this will fail if the interface is implemented explicitly
                    var value = new Lazy<Tuple<Type, TypeInfo>>( () => new Tuple<Type, TypeInfo>( this.cloneableType, this.cloneableType.GetTypeInfo() ) );
                    Interlocked.CompareExchange( ref ICloneableTypeInfo, value, null );

                    // ICloneable only has one method - Clone; just invoke it
                    var method = this.instanceType.GetRuntimeInterfaceMap( ICloneableTypeInfo.Value.Item1 ).TargetMethods.Single();
                    return method.Invoke( this.instance, null );
                }
            }
        }

        private const string ICloneableTypeName = "System.ICloneable";
        private static Lazy<Tuple<Type, TypeInfo>> ICloneableTypeInfo;

        internal static ICloneable AsICloneable( this object obj )
        {
            // null; nothing to clone
            if ( obj == null )
                return null;

            var type = obj.GetType().GetTypeInfo();

            // the object is not cloneable
            if ( ICloneableTypeInfo != null && !ICloneableTypeInfo.Value.Item2.IsAssignableFrom( type ) )
                return null;

            // match on implemented interface name
            var interfaces = type.ImplementedInterfaces;
            var cloneableType = interfaces.FirstOrDefault( t => t.FullName == ICloneableTypeName );

            // ICloneable not implemented
            if ( cloneableType == null )
                return null;

            return new CloneableBridge( obj, type, cloneableType );
        }
    }
}
