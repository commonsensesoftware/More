namespace More.VisualStudio.Views
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [Serializable]
    internal sealed class SimpleType : Type, ISerializable
    {
        private readonly string assemblyQualifiedName;
        private readonly string fullName;
        private readonly string namespaceName;
        private readonly string typeName;

        internal SimpleType( SerializationInfo info, StreamingContext context )
        {
            if ( info == null )
                throw new ArgumentNullException( "info" );

            this.assemblyQualifiedName = info.GetString( "AssemblyQualifiedName" );
            this.fullName = info.GetString( "FullName" );
            this.namespaceName = info.GetString( "Namespace" );
            this.typeName = info.GetString( "Name" );
        }

        internal SimpleType( Type type )
        {
            Contract.Requires( type != null );

            this.assemblyQualifiedName = type.AssemblyQualifiedName;
            this.fullName = type.FullName;
            this.namespaceName = type.Namespace;
            this.typeName = type.Name;
        }

        public void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            if ( info == null )
                throw new ArgumentNullException( "info" );

            info.AddValue( "AssemblyQualifiedName", this.AssemblyQualifiedName );
            info.AddValue( "FullName", this.FullName );
            info.AddValue( "Namespace", this.Namespace );
            info.AddValue( "Name", this.Name );
        }

        public override Assembly Assembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string AssemblyQualifiedName
        {
            get
            {
                return this.assemblyQualifiedName;
            }
        }

        public override Type BaseType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string FullName
        {
            get
            {
                return this.fullName;
            }
        }

        public override Guid GUID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl( BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers )
        {
            throw new NotImplementedException();
        }

        public override ConstructorInfo[] GetConstructors( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        public override EventInfo GetEvent( string name, BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField( string name, BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override Type GetInterface( string name, bool ignoreCase )
        {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        public override MemberInfo[] GetMembers( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl( string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers )
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override Type GetNestedType( string name, BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        public override PropertyInfo[] GetProperties( BindingFlags bindingAttr )
        {
            throw new NotImplementedException();
        }

        protected override PropertyInfo GetPropertyImpl( string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers )
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        public override object InvokeMember( string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters )
        {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        public override Module Module
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Namespace
        {
            get
            {
                return this.namespaceName;
            }
        }

        public override Type UnderlyingSystemType
        {
            get
            {
                return this;
            }
        }

        public override object[] GetCustomAttributes( Type attributeType, bool inherit )
        {
            return new object[0];
        }

        public override object[] GetCustomAttributes( bool inherit )
        {
            return new object[0];
        }

        public override bool IsDefined( Type attributeType, bool inherit )
        {
            return false;
        }

        public override string Name
        {
            get
            {
                return this.typeName;
            }
        }

        public override bool Equals( Type o )
        {
            if ( o == null )
                return false;

            return this.AssemblyQualifiedName == o.AssemblyQualifiedName;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode( this );
        }
    }
}
