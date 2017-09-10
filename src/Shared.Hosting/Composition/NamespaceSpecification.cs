namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using static System.StringComparison;

    sealed class NamespaceSpecification : SpecificationBase<Type>
    {
        static readonly TypeInfo AttributeType = typeof( Attribute ).GetTypeInfo();
        readonly string[] fragments = new string[3];

        internal NamespaceSpecification( string @namespace )
        {
            Contract.Requires( !string.IsNullOrEmpty( @namespace ) );

            var fragment = "." + @namespace;

            fragments[0] = @namespace;
            fragments[1] = fragment;
            fragments[2] = fragment + ".";
        }

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null || item.GetTypeInfo().IsAssignableFrom( AttributeType ) )
            {
                return false;
            }

            var @namespace = item.Namespace;

            if ( string.IsNullOrEmpty( @namespace ) )
            {
                return false;
            }

            if ( @namespace.Equals( fragments[0], Ordinal ) )
            {
                return true;
            }

            if ( @namespace.EndsWith( fragments[1], Ordinal ) )
            {
                return true;
            }

            return @namespace.Contains( fragments[2], Ordinal );
        }
    }
}