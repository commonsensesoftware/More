namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using static System.StringComparison;

    sealed class ViewModelSpecification : SpecificationBase<Type>
    {
        const string ViewModel = "ViewModel";

        static bool MatchesTypeNameOrNamespace( TypeInfo typeInfo )
        {
            Contract.Requires( typeInfo != null );

            return typeInfo.Name.Contains( ViewModel, OrdinalIgnoreCase )
                || typeInfo.Namespace.Contains( ViewModel, OrdinalIgnoreCase );
        }

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
            {
                return false;
            }

            var ti = item.GetTypeInfo();
            return ti.IsPublic && ti.IsClass && !ti.IsAbstract && MatchesTypeNameOrNamespace( ti );
        }
    }
}