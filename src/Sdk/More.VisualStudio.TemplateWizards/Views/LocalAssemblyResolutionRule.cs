namespace More.VisualStudio.Views
{
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    sealed class LocalAssemblyResolutionRule : IRule<AssemblyName, Assembly>
    {
        readonly ILocalAssemblySource localAssemblySource;

        internal LocalAssemblyResolutionRule( ILocalAssemblySource localAssemblySource )
        {
            Contract.Requires( localAssemblySource != null );
            this.localAssemblySource = localAssemblySource;
        }

        public Assembly Evaluate( AssemblyName item )
        {
            if ( item == null )
            {
                return null;
            }

            if ( AssemblyName.ReferenceMatchesDefinition( localAssemblySource.LocalAssemblyName, item ) )
            {
                return localAssemblySource.LocalAssembly;
            }

            return null;
        }
    }
}