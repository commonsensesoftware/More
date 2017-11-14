namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    sealed class DefaultAssemblyResolutionRule : IRule<ResolveEventArgs, Assembly>, IRule<AssemblyName, Assembly>
    {
        public Assembly Evaluate( ResolveEventArgs item ) => item == null ? null : Evaluate( new AssemblyName( item.Name ) );

        public Assembly Evaluate( AssemblyName item )
        {
            if ( item == null )
            {
                return null;
            }

            try
            {
                // load assembly using standard reflection-only context
                Debug.WriteLine( "Attempting to load assembly '{0}' into a reflection-only context.", new[] { item.Name } );
                return Assembly.ReflectionOnlyLoad( item.FullName );
            }
            catch ( FileNotFoundException )
            {
                // resolution failed
                Debug.WriteLine( "Failed to load assembly '{0}' into a reflection-only context.", new[] { item.Name } );
                return null;
            }
        }
    }
}