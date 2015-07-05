namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.Design;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using global::Windows.Storage;
    using global::Windows.UI.Popups;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using More.IO;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    public partial class Host
    {
        private static object LocateSetting( string key )
        {
            var data = ApplicationData.Current;
            object value;

            if ( data.LocalSettings.Values.TryGetValue( key, out value ) )
                return value;

            if ( data.RoamingSettings.Values.TryGetValue( key, out value ) )
                return value;

            return null;
        }

        /// <summary>
        /// Creates the underlying container.
        /// </summary>
        /// <returns>The constructed <see cref="CompositionHost">container</see>.</returns>
        [CLSCompliant( false )]
        protected virtual CompositionHost CreateContainer()
        {
            Contract.Ensures( Contract.Result<CompositionHost>() != null );

            var conventions = this.conventionsHolder.Value;
            var config = this.Configuration;
            var part = new PartSpecification();
            var core = typeof( ServiceProvider ).GetTypeInfo().Assembly;
            var ui = typeof( ShellViewBase ).GetTypeInfo().Assembly;
            var uiTypes = ui.ExportedTypes.Where( part.IsSatisfiedBy );
            var host = typeof( Host ).GetTypeInfo().Assembly;
            var hostTypes = host.ExportedTypes.Where( part.IsSatisfiedBy );

            config.WithAssembly( core, conventions );
            config.WithParts( uiTypes, conventions );
            config.WithParts( hostTypes, conventions );
            config.WithDefaultConventions( conventions );
            config.WithProvider( new HostExportDescriptorProvider( this, "Host" ) );
            config.WithProvider( new ConfigurationExportProvider( this.configSettingLocator, "Host" ) );

            var container = config.CreateContainer();

            // register default services directly after the underlying container is created
            // optimization: call base implementation because this object will never be composed
            base.AddService( typeof( IFileSystem ), ( sc, t ) => new FileSystem() );
            base.AddService( typeof( IValidator ), ( sc, t ) => new ValidatorAdapter() );

            return container;
        }

        partial void AddWinRTSpecificConventions( ConventionBuilder builder );

        partial void AddPlatformSpecificConventions( ConventionBuilder builder )
        {
            var assembly = new HostAssemblySpecification();
            var page = new PageSpecification();
            var userControl = new UserControlSpecification();

            builder.ForType<TypeResolutionService>().Export<ITypeResolutionService>().Shared();
            builder.ForTypesMatching( page.IsSatisfiedBy ).Export().Export<Page>().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
            builder.ForTypesMatching( userControl.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
            this.AddWinRTSpecificConventions( builder );
        }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <example>This example demonstrates how to host a Windows Store application.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System.Reflection;
        /// using global::Windows.ApplicationModel;
        /// using global::Windows.ApplicationModel.Activation;
        /// using global::Windows.UI.Xaml;
        /// 
        /// public class MyShellView : ShellViewBase
        /// {
        /// }
        ///     
        /// [Task]
        /// public class ShowShellView : ShowShellView<MyShellView>
        /// {
        /// }
        ///     
        /// public sealed class App : Application
        /// {
        ///     protected override void OnLaunched( LaunchActivatedEventArgs args )
        ///     {
        ///         var configuration = new CompositionConfiguration().WithAssembly( this.GetType().GetTypeInfo().Assembly );
        ///         
        ///         using ( var host = new Host( configuration ) )
        ///         {
        ///             host.Register<ShowShellView>();
        ///             host.Run( this );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public async virtual void Run( Application application )
        {
            Contract.Requires<ArgumentNullException>( application != null, "application" );

            var assembly = application.GetType().GetTypeInfo().Assembly;
            this.Configuration.WithAssembly( assembly, this.conventionsHolder.Value );

            // set current service provider if unset
            if ( ServiceProvider.Current == ServiceProvider.Default )
                ServiceProvider.SetCurrent( this );

            Exception exception = null;

            try
            {
                // build up and execute the startup tasks
                foreach ( var activity in this.Activities.Where( a => a.CanExecute( this ) ) )
                    activity.Execute( this );
            }
            catch ( Exception ex )
            {
                // cannot use 'await' in a catch block
                exception = ex;
            }

            if ( exception != null )
            {
                await new MessageDialog( exception.Message, SR.ActivityFailedCaption ).ShowAsync();
                return;
            }

            // set the default unit of work if unset
            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( this.Container.GetExports<IUnitOfWorkFactory> );
        }
    }
}
