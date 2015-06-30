namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;

    /// <content>
    /// Provides additional implementation specified to Windows Desktop applications.
    /// </content>
    public partial class Host
    {
        private static object LocateSetting( string key )
        {
            var value = (object) ConfigurationManager.AppSettings[key] ?? (object) ConfigurationManager.ConnectionStrings[key];
            return value;
        }

        /// <summary>
        /// Creates the underlying container.
        /// </summary>
        /// <returns>The constructed <see cref="CompositionHost">container</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does not have idempoent behavior, which does makes it unsuitable for a property." )]
        protected virtual CompositionHost CreateContainer()
        {
            Contract.Ensures( Contract.Result<CompositionHost>() != null );

            var conventions = this.conventionsHolder.Value;
            var config = this.Configuration;
            var origin = typeof( Host ).Name;

            config.WithAppDomain( conventions );
            config.WithDefaultConventions( conventions );
            config.WithProvider( new HostExportDescriptorProvider( this, origin ) );
            config.WithProvider( new ConfigurationExportProvider( this.configSettingLocator, origin ) );

            var container = config.CreateContainer();

            // register default services directly after the underlying container is created
            // optimization: call base implementation because this object will never be composed
            base.AddService( typeof( IValidator ), ( sc, t ) => new ValidatorAdapter() );

            return container;
        }

        partial void AddPlatformSpecificConventions( ConventionBuilder builder )
        {
            var assembly = new HostAssemblySpecification();
            var window = new AssignableSpecification<Window>().And( new AssignableSpecification<IShellView>().Not() );
            var userControl = new AssignableSpecification<UserControl>().And( new AssignableSpecification<IShellView>().Not() );

            builder.ForTypesMatching( window.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
            builder.ForTypesMatching( userControl.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
        }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <example>This example demonstrates how to host a Windows Presentation Foundation (WPF) application.
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.Composition;
        /// using System.Composition.Hosting;
        /// using System.Windows;
        /// 
        /// public class Program
        /// {
        ///     public class MyShellView : ShellViewBase
        ///     {
        ///     }
        ///     
        ///     public class ShowShellViewTask : ShowShellView<MyShellView>
        ///     {
        ///     }
        ///     
        ///     [STAThread]
        ///     public static void Main()
        ///     {
        ///         using ( var host = new Host() )
        ///         {
        ///             host.Register<ShowShellView>();
        ///             host.Run( new App() );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual void Run( Application application )
        {
            // HACK: WPF doesn't set the Application.Current property until an application object has been created.  This can cause composition issues.
            // Require this method to accept an application object to ensure it's set.  This also simplifies startup code.

            Contract.Requires<ArgumentNullException>( application != null, "application" );

            // set current service provider if unset
            if ( ServiceProvider.Current == ServiceProvider.Default )
                ServiceProvider.SetCurrent( this );

            try
            {
                // build up and execute the startup activities
                foreach ( var activity in this.Activities )
                    activity.Execute( this );
            }
            catch ( HostException ex )
            {
                if ( application.MainWindow != null )
                    MessageBox.Show( application.MainWindow, ex.Message, SR.ActivityFailedCaption, MessageBoxButton.OK );
                else
                    MessageBox.Show( ex.Message, SR.ActivityFailedCaption, MessageBoxButton.OK );

                return;
            }

            // set the default unit of work if unset
            if ( UnitOfWork.Provider == UnitOfWork.DefaultProvider )
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( this.Container.GetExports<IUnitOfWorkFactory> );

            // run the application
            if ( application.MainWindow != null )
                application.Run( application.MainWindow );
            else
                application.Run();
        }
    }
}
