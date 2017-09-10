namespace More.Composition.Hosting
{
    using ComponentModel;
    using IO;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Composition.Convention;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using global::Windows.Storage;
    using global::Windows.UI.Popups;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    public partial class Host
    {
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
        ///         using ( var host = new Host() )
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
            Arg.NotNull( application, nameof( application ) );

            var assembly = application.GetType().GetTypeInfo().Assembly;

            Configuration.WithAssembly( assembly, conventionsHolder.Value );

            if ( ServiceProvider.Current == ServiceProvider.Default )
            {
                ServiceProvider.SetCurrent( this );
            }

            try
            {
                Activities.Where( a => a.CanExecute( this ) ).ForEach( a => a.Execute( this ) );
            }
            catch ( Exception ex )
            {
                await new MessageDialog( ex.Message, SR.ActivityFailedCaption ).ShowAsync();
                return;
            }

            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
            {
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( Container.GetExports<IUnitOfWorkFactory> );
            }
        }

        static object LocateSetting( string key, Type type )
        {
            var data = ApplicationData.Current;

            if ( data.LocalSettings.Values.TryGetValue( key, out var value ) )
            {
                return value;
            }

            if ( data.RoamingSettings.Values.TryGetValue( key, out value ) )
            {
                return value;
            }

            return null;
        }

        partial void AddPlatformSpecificDefaultServices()
        {
            base.AddService( typeof( ITypeResolutionService ), ( sc, t ) => new TypeResolutionService() );
            base.AddService( typeof( ISessionStateManager ), ( sc, t ) => new LocalSessionStateManager( sc.GetRequiredService<IFileSystem>() ) );
            AddWinRTDefaultServices();
        }

        partial void AddWinRTDefaultServices();

        static partial void AddWinRTSpecificConventions( ConventionBuilder builder );

        static partial void AddPlatformSpecificConventions( ConventionBuilder builder )
        {
            var assembly = new PublicKeyTokenSpecification( typeof( Host ) );
            var page = new PageSpecification();
            var userControl = new UserControlSpecification();

            builder.ForTypesMatching( page.IsSatisfiedBy ).Export().Export<Page>().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p?.Name == "Model" );
            builder.ForTypesMatching( userControl.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p?.Name == "Model" );
            AddWinRTSpecificConventions( builder );
        }
    }
}