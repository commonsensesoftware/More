namespace More.Composition
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using static System.StringComparison;

    internal sealed class NamespaceSpecification : SpecificationBase<Type>
    {
        private static readonly TypeInfo AttributeType = typeof( Attribute ).GetTypeInfo();
        private readonly string[] fragments = new string[3];

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
            // do not match null types or attributes
            if ( item == null || item.GetTypeInfo().IsAssignableFrom( AttributeType ) )
                return false;

            var @namespace = item.Namespace;

            // skip null or empty namespaces; typically happens with generic types or type definitions
            if ( string.IsNullOrEmpty( @namespace ) )
                return false;

            // try exact match
            if ( @namespace.Equals( fragments[0], Ordinal ) )
                return true;

            // match suffix
            if ( @namespace.EndsWith( fragments[1], Ordinal ) )
                return true;

            // match any containing namespace
            return @namespace.Contains( fragments[2], Ordinal );
        }
    }
}
