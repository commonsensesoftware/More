namespace More.VisualStudio.Views
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable]
    sealed class SimpleType : Type, ISerializable
    {
        internal SimpleType( SerializationInfo info, StreamingContext context )
        {
            if ( info == null )
            {
                throw new ArgumentNullException( "info" );
            }

            AssemblyQualifiedName = info.GetString( "AssemblyQualifiedName" );
            FullName = info.GetString( "FullName" );
            Namespace = info.GetString( "Namespace" );
            Name = info.GetString( "Name" );
        }

        internal SimpleType( Type type )
        {
            Contract.Requires( type != null );

            AssemblyQualifiedName = type.AssemblyQualifiedName;
            FullName = type.FullName;
            Namespace = type.Namespace;
            Name = type.Name;
        }

        public void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            if ( info == null )
            {
                throw new ArgumentNullException( "info" );
            }

            info.AddValue( "AssemblyQualifiedName", AssemblyQualifiedName );
            info.AddValue( "FullName", FullName );
            info.AddValue( "Namespace", Namespace );
            info.AddValue( "Name", Name );
        }

        public override Assembly Assembly => throw new NotImplementedException();

        public override string AssemblyQualifiedName { get; }

        public override Type BaseType => throw new NotImplementedException();

        public override string FullName { get; }

        public override Guid GUID => throw new NotImplementedException();

        protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();

        protected override ConstructorInfo GetConstructorImpl( BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers ) => throw new NotImplementedException();

        public override ConstructorInfo[] GetConstructors( BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override Type GetElementType() => throw new NotImplementedException();

        public override EventInfo GetEvent( string name, BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override EventInfo[] GetEvents( BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override FieldInfo GetField( string name, BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override FieldInfo[] GetFields( BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override Type GetInterface( string name, bool ignoreCase ) => throw new NotImplementedException();

        public override Type[] GetInterfaces() => throw new NotImplementedException();

        public override MemberInfo[] GetMembers( BindingFlags bindingAttr ) => throw new NotImplementedException();

        protected override MethodInfo GetMethodImpl( string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers ) => throw new NotImplementedException();

        public override MethodInfo[] GetMethods( BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override Type GetNestedType( string name, BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override Type[] GetNestedTypes( BindingFlags bindingAttr ) => throw new NotImplementedException();

        public override PropertyInfo[] GetProperties( BindingFlags bindingAttr ) => throw new NotImplementedException();

        protected override PropertyInfo GetPropertyImpl( string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers ) => throw new NotImplementedException();

        protected override bool HasElementTypeImpl() => throw new NotImplementedException();

        public override object InvokeMember( string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters ) => throw new NotImplementedException();

        protected override bool IsArrayImpl() => throw new NotImplementedException();

        protected override bool IsByRefImpl() => throw new NotImplementedException();

        protected override bool IsCOMObjectImpl() => throw new NotImplementedException();

        protected override bool IsPointerImpl() => throw new NotImplementedException();

        protected override bool IsPrimitiveImpl() => throw new NotImplementedException();

        public override Module Module => throw new NotImplementedException();

        public override string Namespace { get; }

        public override Type UnderlyingSystemType => this;

        public override object[] GetCustomAttributes( Type attributeType, bool inherit ) => new object[0];

        public override object[] GetCustomAttributes( bool inherit ) => new object[0];

        public override bool IsDefined( Type attributeType, bool inherit ) => false;

        public override string Name { get; }

        public override bool Equals( Type o )
        {
            if ( o == null )
            {
                return false;
            }

            return AssemblyQualifiedName == o.AssemblyQualifiedName;
        }

        public override int GetHashCode() => RuntimeHelpers.GetHashCode( this );
    }
}