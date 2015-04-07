namespace More.VisualStudio.ViewModels
{
    using EnvDTE;
    using Microsoft.VisualStudio.Data.Core;
    using Microsoft.VisualStudio.DataTools.Interop;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.Entity.SqlServer;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    internal static class IVsDataProviderManagerExtensions
    {
        private static readonly Version EFVersion6 = new Version( 6, 0, 0, 0 );

        internal static string GetProviderInvariantName( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            Contract.Requires( dataProviderManager != null );

            string property = null;
            IVsDataProvider dataProvider = null;

            dataProviderManager.Providers.TryGetValue( provider, out dataProvider );

            if ( dataProvider != null )
                property = (string) dataProvider.GetProperty( "InvariantName" );

            return property;
        }

        private static DbProviderFactory GetProviderFactoryForProviderGuid( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            var providerInvariantName = dataProviderManager.GetProviderInvariantName( provider );
            DbProviderFactory factory = null;

            if ( string.IsNullOrEmpty( providerInvariantName ) )
                return factory;

            try
            {
                factory = DbProviderFactories.GetFactory( providerInvariantName );
            }
            catch ( ConfigurationException )
            {
            }
            catch ( ArgumentException )
            {
            }

            return factory;
        }

        private static bool HasLegacyEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            var serviceProvider = dataProviderManager.GetProviderFactoryForProviderGuid( provider ) as IServiceProvider;
            DbProviderServices service;
            return serviceProvider != null && serviceProvider.TryGetService( out service ) && service != null;
        }

        private static bool HasModernEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider, Project project, IServiceProvider serviceProvider )
        {
            string providerInvariantName = dataProviderManager.GetProviderInvariantName( provider );

            if ( string.IsNullOrWhiteSpace( providerInvariantName ) )
                return false;

            return IsModernProviderAvailable( providerInvariantName, project, serviceProvider );
        }

        private static Type GetKnownModernProvider( string providerInvariantName )
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals( providerInvariantName, "System.Data.SqlClient" ) ? typeof( SqlProviderServices ) : null;
        }

        private static IEnumerable<Version> GetInstalledEntityFrameworkAssemblyVersions( this Project project )
        {
            Contract.Requires( project != null );
            Contract.Ensures( Contract.Result<IEnumerable<Version>>() != null );

            var comparer = StringComparer.OrdinalIgnoreCase;
            var versions = from reference in project.GetReferences()
                           where comparer.Equals( reference.Key, "EntityFramework" ) || comparer.Equals( reference.Key, "System.Data.Entity" )
                           orderby reference.Value descending
                           select reference.Value;

            return versions;
        }

        private static Version GetInstalledEntityFrameworkAssemblyVersion( Project project )
        {
            Contract.Requires( project != null );
            return project.GetInstalledEntityFrameworkAssemblyVersions().FirstOrDefault();
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The result should simply be null instead of propagating the exception." )]
        private static string GetModernProviderTypeNameFromProject( string invariantName, Project project, IServiceProvider serviceProvider )
        {
            var installedEntityFrameworkAssemblyVersion = GetInstalledEntityFrameworkAssemblyVersion( project );

            if ( ( installedEntityFrameworkAssemblyVersion == null ) || installedEntityFrameworkAssemblyVersion < EFVersion6 )
                return null;

            try
            {
                using ( var context = ActivatorFactory.NewProjectExecutionContext( project, serviceProvider ) )
                {
                    return context.GetProviderServices( invariantName );
                }
            }
            catch
            {
                return null;
            }
        }

        internal static bool IsModernProviderAvailable( string providerInvariantName, Project project, IServiceProvider serviceProvider )
        {
            if ( GetKnownModernProvider( providerInvariantName ) == null )
                return ( GetModernProviderTypeNameFromProject( providerInvariantName, project, serviceProvider ) != null );

            return true;
        }

        internal static bool HasEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider, Project project, IServiceProvider serviceProvider )
        {
            Contract.Requires( dataProviderManager != null );

            if ( !dataProviderManager.HasLegacyEntityFrameworkProvider( provider ) )
                return dataProviderManager.HasModernEntityFrameworkProvider( provider, project, serviceProvider );

            return true;
        }

        internal static bool IsProjectSupported( this IVsDataProviderManager dataProviderManager, Guid providerGuid, IServiceProvider serviceProvider )
        {
            Contract.Requires( dataProviderManager != null );
            Contract.Requires( serviceProvider != null );

            IVsDataProvider provider;

            if ( !dataProviderManager.Providers.TryGetValue( providerGuid, out provider ) )
                return false;

            IDTAdoDotNetProviderMapper mapper;

            if ( !serviceProvider.TryGetService( out mapper ) )
                return false;

            var invariantProviderName = mapper.MapGuidToInvariantName( providerGuid );

            if ( string.IsNullOrWhiteSpace( invariantProviderName ) )
                return false;

            var sqlCompactProvider = invariantProviderName.StartsWith( "Microsoft.SqlServerCe.Client", StringComparison.OrdinalIgnoreCase );

            return !sqlCompactProvider;
        }
    }
}
