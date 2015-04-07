namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    internal sealed class ViewModelSpecification : SpecificationBase<Type>
    {
        private const string ViewModel = "ViewModel";

        private static bool MatchesTypeNameOrNamespace( TypeInfo typeInfo )
        {
            Contract.Requires( typeInfo != null );
            // matches "MyAssembly.ViewModels" or "MyViewModel", "ViewModel1" and so on
            return typeInfo.Name.Contains( ViewModel, StringComparison.OrdinalIgnoreCase )
                || typeInfo.Namespace.Contains( ViewModel, StringComparison.OrdinalIgnoreCase );
        }

        public override bool IsSatisfiedBy( Type item )
        {
            if ( item == null )
                return false;

            var ti = item.GetTypeInfo();
            return ti.IsPublic && ti.IsClass && !ti.IsAbstract && MatchesTypeNameOrNamespace( ti );
        }
    }
}
