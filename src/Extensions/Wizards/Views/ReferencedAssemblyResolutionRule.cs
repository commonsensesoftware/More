﻿namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal sealed class ReferencedAssemblyResolutionRule : IRule<ResolveEventArgs, Assembly>, IRule<AssemblyName, Assembly>
    {
        private readonly IEnumerable<AssemblyName> localAssemblyReferences;

        internal ReferencedAssemblyResolutionRule( IEnumerable<AssemblyName> localAssemblyReferences )
        {
            Contract.Requires( localAssemblyReferences != null );
            this.localAssemblyReferences = localAssemblyReferences;
        }

        public Assembly Evaluate( ResolveEventArgs item )
        {
            return item == null ? null : Evaluate( new AssemblyName( item.Name ) );
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The method should return null rather than throw an unhandled exception." )]
        public Assembly Evaluate( AssemblyName item )
        {
            if ( item == null )
                return null;

            // note: AssemblyName.ReferenceMatchesDefinition will report that the assembly names match even if the versions are different.
            // the implementation is 'externed' so it is assume that it simply does not account for version (which matters in for our purposes)
            var assemblyReference = localAssemblyReferences.FirstOrDefault( other => AssemblyName.ReferenceMatchesDefinition( item, other ) && item.Version == other.Version );

            if ( assemblyReference == null )
                return null;

            var location = assemblyReference.GetLocation();

            if ( string.IsNullOrEmpty( location ) || !File.Exists( location ) )
                return null;

            try
            {
                Debug.WriteLine( "Attempting to load assembly '{0}' into a reflection-only context from {1}.", item.Name, location);
                return Assembly.ReflectionOnlyLoadFrom( location );
            }
            catch
            {
                // resolution failed
                Debug.WriteLine( "Failed to load assembly '{0}' into a reflection-only context.", item.Name);
                return null;
            }
        }
    }
}
