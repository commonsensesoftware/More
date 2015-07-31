namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    internal sealed class LocalAssemblyResolutionRule : IRule<AssemblyName, Assembly>
    {
        private readonly ILocalAssemblySource localAssemblySource;

        internal LocalAssemblyResolutionRule( ILocalAssemblySource localAssemblySource )
        {
            Contract.Requires( localAssemblySource != null );
            this.localAssemblySource = localAssemblySource;
        }

        public Assembly Evaluate( AssemblyName item )
        {
            if ( item == null )
                return null;

            if ( AssemblyName.ReferenceMatchesDefinition( localAssemblySource.LocalAssemblyName, item ) )
                return localAssemblySource.LocalAssembly;

            return null;
        }
    }
}
