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
    using System.Linq;

    static class IVsDataProviderManagerExtensions
    {
        static readonly Version EFVersion6 = new Version( 6, 0, 0, 0 );

        internal static string GetProviderInvariantName( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            Contract.Requires( dataProviderManager != null );

            var property = default( string );

            dataProviderManager.Providers.TryGetValue( provider, out var dataProvider );

            if ( dataProvider != null )
            {
                property = (string) dataProvider.GetProperty( "InvariantName" );
            }

            return property;
        }

        static DbProviderFactory GetProviderFactoryForProviderGuid( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            var providerInvariantName = dataProviderManager.GetProviderInvariantName( provider );
            var factory = default( DbProviderFactory );

            if ( string.IsNullOrEmpty( providerInvariantName ) )
            {
                return factory;
            }

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

        static bool HasLegacyEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider )
        {
            if ( dataProviderManager.GetProviderFactoryForProviderGuid( provider ) is IServiceProvider serviceProvider )
            {
                return serviceProvider.TryGetService( out DbProviderServices service ) && service != null;
            }

            return false;
        }

        static bool HasModernEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider, Project project, IServiceProvider serviceProvider )
        {
            var providerInvariantName = dataProviderManager.GetProviderInvariantName( provider );

            if ( string.IsNullOrWhiteSpace( providerInvariantName ) )
            {
                return false;
            }

            return IsModernProviderAvailable( providerInvariantName, project, serviceProvider );
        }

        static Type GetKnownModernProvider( string providerInvariantName ) => StringComparer.InvariantCultureIgnoreCase.Equals( providerInvariantName, "System.Data.SqlClient" ) ? typeof( SqlProviderServices ) : null;

        static IEnumerable<Version> GetInstalledEntityFrameworkAssemblyVersions( this Project project )
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

        static Version GetInstalledEntityFrameworkAssemblyVersion( Project project )
        {
            Contract.Requires( project != null );
            return project.GetInstalledEntityFrameworkAssemblyVersions().FirstOrDefault();
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The result should simply be null instead of propagating the exception." )]
        static string GetModernProviderTypeNameFromProject( string invariantName, Project project, IServiceProvider serviceProvider )
        {
            var installedEntityFrameworkAssemblyVersion = GetInstalledEntityFrameworkAssemblyVersion( project );

            if ( ( installedEntityFrameworkAssemblyVersion == null ) || installedEntityFrameworkAssemblyVersion < EFVersion6 )
            {
                return null;
            }

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
            {
                return ( GetModernProviderTypeNameFromProject( providerInvariantName, project, serviceProvider ) != null );
            }

            return true;
        }

        internal static bool HasEntityFrameworkProvider( this IVsDataProviderManager dataProviderManager, Guid provider, Project project, IServiceProvider serviceProvider )
        {
            Contract.Requires( dataProviderManager != null );

            if ( !dataProviderManager.HasLegacyEntityFrameworkProvider( provider ) )
            {
                return dataProviderManager.HasModernEntityFrameworkProvider( provider, project, serviceProvider );
            }

            return true;
        }

        internal static bool IsProjectSupported( this IVsDataProviderManager dataProviderManager, Guid providerGuid, IServiceProvider serviceProvider )
        {
            Contract.Requires( dataProviderManager != null );
            Contract.Requires( serviceProvider != null );

            if ( !dataProviderManager.Providers.TryGetValue( providerGuid, out var provider ) )
            {
                return false;
            }

            if ( !serviceProvider.TryGetService( out IDTAdoDotNetProviderMapper mapper ) )
            {
                return false;
            }

            var invariantProviderName = mapper.MapGuidToInvariantName( providerGuid );

            if ( string.IsNullOrWhiteSpace( invariantProviderName ) )
            {
                return false;
            }

            var sqlCompactProvider = invariantProviderName.StartsWith( "Microsoft.SqlServerCe.Client", StringComparison.OrdinalIgnoreCase );

            return !sqlCompactProvider;
        }
    }
}