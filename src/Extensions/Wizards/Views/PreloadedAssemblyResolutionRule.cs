namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class PreloadedAssemblyResolutionRule : IRule<ResolveEventArgs, Assembly>, IRule<AssemblyName, Assembly>
    {
        public Assembly Evaluate( ResolveEventArgs item )
        {
            return item == null ? null : this.Evaluate( new AssemblyName( item.Name ) );
        }

        public Assembly Evaluate( AssemblyName item )
        {
            if ( item == null )
                return null;

            // use an assembly that's already been loaded
            var assemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
            var assembly = assemblies.SingleOrDefault( a => AssemblyName.ReferenceMatchesDefinition( a.GetName(), item ) );

            if ( assembly != null )
                Debug.WriteLine( "Using previously loaded reflection-only assembly '{0}'.", new[] { item.Name } );

            return assembly;
        }
    }
}
