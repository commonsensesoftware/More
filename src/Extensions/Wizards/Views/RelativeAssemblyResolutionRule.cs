namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    internal sealed class RelativeAssemblyResolutionRule : IRule<ResolveEventArgs, Assembly>
    {
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The method should return null rather than throw an unhandled exception." )]
        public Assembly Evaluate( ResolveEventArgs item )
        {
            if ( item == null )
                return null;

            var assembly = item.RequestingAssembly;

            if ( assembly == null || string.IsNullOrEmpty( assembly.Location ) )
                return null;

            // try loading the assembly relative to the requesting assembly
            // note: this is expected to always be a *.dll
            var assemblyName = new AssemblyName( item.Name );
            var directory = Path.GetDirectoryName( assembly.Location );
            var assemblyFile = Path.Combine( directory, assemblyName.Name + ".dll" );

            // the file doesn't exist or we can't build the name because the assembly name
            // is not the same as the file name
            if ( !File.Exists( assemblyFile ) )
                return null;

            try
            {
                Debug.WriteLine( "Attempting to load assembly '{0}' into a reflection-only context from {1}.", new[] { item.Name, assemblyFile } );
                return Assembly.ReflectionOnlyLoadFrom( assemblyFile );
            }
            catch
            {
                // resolution failed
                Debug.WriteLine( "Failed to load assembly '{0}' into a reflection-only context.", new[] { item.Name } );
                return null;
            }
        }
    }
}
