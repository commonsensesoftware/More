namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Activities.Presentation.Hosting;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices.WindowsRuntime;

    /// <summary>
    /// Represents a service that supports assembly and type resolution for multi-targeting scenarios.
    /// </summary>
    sealed class MultiTargetingSupportService : IMultiTargetingSupportService
    {
        readonly List<IRule<AssemblyName, Assembly>> rules;
        readonly List<IRule<ResolveEventArgs, Assembly>> resolveRules;

        ~MultiTargetingSupportService()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
            WindowsRuntimeMetadata.ReflectionOnlyNamespaceResolve -= OnReflectionOnlyNamespaceResolve;
        }

        internal MultiTargetingSupportService( ILocalAssemblySource localAssemblySource )
        {
            Contract.Requires( localAssemblySource != null );

            var preloadedRule = new PreloadedAssemblyResolutionRule();
            var referencedRule = new ReferencedAssemblyResolutionRule( localAssemblySource.LocalAssemblyReferences );
            var defaultRule = new DefaultAssemblyResolutionRule();

            rules = new List<IRule<AssemblyName, Assembly>>()
            {
                new LocalAssemblyResolutionRule( localAssemblySource ),
                preloadedRule,
                referencedRule,
                defaultRule
            };
            resolveRules = new List<IRule<ResolveEventArgs, Assembly>>()
            {
                preloadedRule,
                referencedRule,
                new RelativeAssemblyResolutionRule(),
                defaultRule
            };
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            WindowsRuntimeMetadata.ReflectionOnlyNamespaceResolve += OnReflectionOnlyNamespaceResolve;
        }

        Assembly OnReflectionOnlyAssemblyResolve( object sender, ResolveEventArgs e ) => resolveRules.Select( r => r.Evaluate( e ) ).FirstOrDefault( a => a != null );

        void OnReflectionOnlyNamespaceResolve( object sender, NamespaceResolveEventArgs e )
        {
            // use the winrt apis to resolve the assembly location from the specified namespace
            var paths = WindowsRuntimeMetadata.ResolveNamespace( e.NamespaceName, Enumerable.Empty<string>() ).ToArray();
            var path = paths.FirstOrDefault();

            // if the assembly is found, load it in the reflection-only context
            if ( string.IsNullOrEmpty( path ) )
            {
                Debug.WriteLine( "Failed to load assembly '{0}' into a reflection-only context.", new[] { e.NamespaceName } );
            }
            else
            {
                Debug.WriteLine( "Attempting to load assembly '{0}' into a reflection-only context from {1}.", new[] { e.NamespaceName, path } );
                e.ResolvedAssemblies.Add( Assembly.ReflectionOnlyLoadFrom( path ) );
            }
        }

        public Assembly GetReflectionAssembly( AssemblyName targetAssemblyName ) => rules.Select( r => r.Evaluate( targetAssemblyName ) ).FirstOrDefault( a => a != null );

        public Type GetRuntimeType( Type reflectionType ) => reflectionType;

        public bool IsSupportedType( Type type ) => type != null;
    }
}