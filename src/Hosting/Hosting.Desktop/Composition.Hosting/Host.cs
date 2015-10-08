namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    public partial class Host
    {
        private static object LocateSetting( string key )
        {
            object value = ConfigurationManager.AppSettings[key];
            return value ?? ConfigurationManager.ConnectionStrings[key];
        }

        static partial void AddPlatformSpecificConventions( ConventionBuilder builder )
        {
            var assembly = new PublicKeyTokenSpecification( typeof( Host ) );
            var window = new AssignableSpecification<Window>().And( new AssignableSpecification<IShellView>().Not() );
            var userControl = new AssignableSpecification<UserControl>().And( new AssignableSpecification<IShellView>().Not() );

            builder.ForTypesMatching( window.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
            builder.ForTypesMatching( userControl.IsSatisfiedBy ).Export().ExportInterfaces( assembly.IsSatisfiedBy ).ImportProperties( p => p != null && p.Name == "Model" );
        }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <param name="hostedAssemblies">An <see cref="Array">array</see> of hosted, composable <see cref="Assembly">assemblies</see>.</param>
        /// <remarks>The <see cref="Assembly">assembly</see> the <paramref name="application"/> is defined in is always added by default.</remarks>
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
        public void Run( Application application, params Assembly[] hostedAssemblies )
        {
            Arg.NotNull( application, nameof( application ) );
            Arg.NotNull( hostedAssemblies, nameof( hostedAssemblies ) );
            Run( application, hostedAssemblies.AsEnumerable() );
        }

        /// <summary>
        /// Runs the host.
        /// </summary>
        /// <param name="application">The <see cref="Application">application</see> associated with the host.</param>
        /// <param name="hostedAssemblies">An <see cref="IEnumerable{T}">sequence</see> of hosted, composable <see cref="Assembly">assemblies</see>.</param>
        /// <remarks>The <see cref="Assembly">assembly</see> the <paramref name="application"/> is defined in is always added by default.</remarks>
        /// <example>This example demonstrates how to host a Windows Presentation Foundation (WPF) application with explicitly hosted <see cref="Assembly">assemblies</see>.
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
        ///             host.Run( new App(), new []{ type( MyExternalLib ).Assembly } );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public virtual void Run( Application application, IEnumerable<Assembly> hostedAssemblies )
        {
            Arg.NotNull( application, nameof( application ) );
            Arg.NotNull( hostedAssemblies, nameof( hostedAssemblies ) );

            // HACK: WPF doesn't set the Application.Current property until an application object has been created.  This can cause composition issues.
            // Require this method to accept an application object to ensure it's set.  This also simplifies startup code.

            // add hosted assemblies and guard against double registration (which can occur via Configure or using WithAppDomain)
            var applicationAssembly = application.GetType().Assembly;
            var assemblies = new HashSet<Assembly>( hostedAssemblies ) { applicationAssembly }.Where( a => !Configuration.IsRegistered( a ) ).ToArray();

            Configuration.WithAssemblies( assemblies );

            // set current service provider if unset
            if ( ServiceProvider.Current == ServiceProvider.Default )
                ServiceProvider.SetCurrent( this );

            try
            {
                // build up and execute the startup activities
                foreach ( var activity in Activities.Where( a => a.CanExecute( this ) ) )
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
                UnitOfWork.Provider = new UnitOfWorkFactoryProvider( Container.GetExports<IUnitOfWorkFactory> );

            // run the application
            if ( application.MainWindow != null )
                application.Run( application.MainWindow );
            else
                application.Run();
        }
    }
}
