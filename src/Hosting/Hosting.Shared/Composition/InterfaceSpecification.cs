namespace More.Composition
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a <see cref="ISpecification{T}">specification</see> for matching <see cref="Type">types</see> against an interface.
    /// </summary>
    public class InterfaceSpecification : SpecificationBase<Type>
    {
        private readonly TypeInfo interfaceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceSpecification"/> class.
        /// </summary>
        /// <param name="interfaceType">The <see cref="Type">type</see> of interface matched by the specification.</param>
        public InterfaceSpecification( Type interfaceType )
        {
            Contract.Requires( interfaceType != null );
            this.interfaceType = interfaceType.GetTypeInfo();
        }

        /// <summary>
        /// Returns a value indicating whether the specification matches the provided item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the specification is matched by the specified <paramref name="item"/>; otherwise, false.</returns>
        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null || !interfaceType.IsInterface )
                return false;

            var t1 = item.GetTypeInfo();

            if ( !t1.IsPublic || t1.IsAbstract || t1.IsInterface )
                return false;

            var t2 = interfaceType;
            var interfaces = from @interface in t1.ImplementedInterfaces.Select( t => t.GetTypeInfo() )
                             let typeDef = @interface.IsGenericType ? @interface.GetGenericTypeDefinition().GetTypeInfo() : @interface
                             where t2.IsAssignableFrom( typeDef ) || typeDef.ImplementedInterfaces.Any( t => t.GetTypeInfo().IsGenericType && t2.IsAssignableFrom( t.GetGenericTypeDefinition().GetTypeInfo() ) )
                             select @interface;
            var match = interfaces.Any();

            return match;
        }
    }
}