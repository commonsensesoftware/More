namespace More.VisualStudio.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Represents a dialog for picking types.
    /// </summary>
    public partial class TypePicker
    {
        private readonly List<string> restrictedBaseTypeNames = new List<string>();

        [Conditional( "DEBUG" )]
        private static void PrintReflectionOnlyAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            Debug.WriteLine( "Reflection-only assemblies loaded: {0}", assemblies.Length );
            Debug.Indent();

            foreach ( var assembly in assemblies )
                Debug.WriteLine( assembly.GetName().Name );

            Debug.Unindent();
        }

        private static AppDomainSetup GetSetupInformation( AppDomain appDomain )
        {
            Contract.Requires( appDomain != null );
            Contract.Ensures( Contract.Result<AppDomainSetup>() != null );

            var setup = appDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            return setup;
        }

        private static AppDomain CreateAppDomain()
        {
            var currentDomain = AppDomain.CurrentDomain;
            var evidence = currentDomain.Evidence;
            var setup = GetSetupInformation( currentDomain );
            return AppDomain.CreateDomain( "TypePickerSandbox", evidence, setup );
        }

        private async Task<Tuple<AppDomain, Proxy>> CreateProxyAsync( Window window, IProgress<Window> progress )
        {
            Contract.Requires( window != null );
            Contract.Requires( progress != null );
            Contract.Ensures( Contract.Result<Task<Tuple<AppDomain, Proxy>>>() != null );

            var type = typeof( TypePicker.Proxy );
            var assemblyName = type.Assembly.FullName;
            var typeName = type.FullName;
            var appDomain = await Task.Run( new Func<AppDomain>( CreateAppDomain ) ).ConfigureAwait( false );
            TypePicker.Proxy proxy;

            PrintReflectionOnlyAssemblies();

            try
            {
                // pick type from available assemblies in another app domain. this prevents assemblies from being loaded into the current app domain/process,
                // which could prevent prevent the target project from build (because the file is locked by Visual Studio)
                proxy = (TypePicker.Proxy) appDomain.CreateInstanceAndUnwrap( assemblyName, typeName, false, BindingFlags.Default, null, null, null, null );
                proxy.Title = this.Title;
                proxy.NameConvention = this.NameConvention;
                proxy.RestrictedBaseTypeNames = this.RestrictedBaseTypeNames.ToArray();
                proxy.SourceProject = this.SourceProject;

                await appDomain.RunAsync( proxy.CreateLocalAssemblyAsync, this.LocalAssemblyName ).ConfigureAwait( false );
            }
            catch
            {
                AppDomain.Unload( appDomain );
                throw;
            }
            finally
            {
                // note: this is effectively a callback to close the specified window
                progress.Report( window );
            }

            return new Tuple<AppDomain, Proxy>( appDomain, proxy );
        }

        /// <summary>
        /// Shows the type picker using the provided owner.
        /// </summary>
        /// <param name="owner">The <see cref="Window">window</see> that owns the type picker.</param>
        /// <returns>True if the user accepted the dialog, false if the user canceled the dialog, or <c>null</c> if the
        /// user did not provide a response.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        public async Task<bool?> ShowDialogAsync( Window owner )
        {
            var info = await Loader.LoadAsync( this.CreateProxyAsync, owner, SR.StatusInitializing );
            var appDomain = info.Item1;
            var proxy = info.Item2;
            var ownerHandler = owner == null ? IntPtr.Zero : new WindowInteropHelper( owner ).Handle;
            var result = (bool?) null;

            try
            {
                result = proxy.ShowDialog( ownerHandler );
                this.SelectedType = proxy.SelectedType;
            }
            finally
            {
                AppDomain.Unload( appDomain );
                PrintReflectionOnlyAssemblies();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        /// <value>The dialog title.</value>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type selected by the picker.
        /// </summary>
        /// <value>The selected <see cref="Type">type</see> or <c>null</c> is no type is selected.</value>
        public Type SelectedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the name of the local assembly to pick types from.
        /// </summary>
        /// <value>An <see cref="AssemblyName">assembly name</see> of the local assembly.
        /// This property can be <c>null</c>. This is typically true if the local assembly
        /// has not yet been built.</value>
        public AssemblyName LocalAssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source project associated with the type picker.
        /// </summary>
        /// <value>The source <see cref="ProjectInformation">project information</see> used to select a type.</value>
        public ProjectInformation SourceProject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name used to filter types by convention.
        /// </summary>
        /// <value>The name of types to be filtered by convention.</value>
        /// <remarks>If a name convention is not specified, then no conventions
        /// are used.</remarks>
        public string NameConvention
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of restricted base type names.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of base type names.</value>
        /// <remarks>The specified type names can be the type <see cref="P:Type.FullName">full name</see>
        /// or <see cref="P:Type.Name">simple name</see>.</remarks>
        public IList<string> RestrictedBaseTypeNames
        {
            get
            {
                Contract.Ensures( this.restrictedBaseTypeNames != null );
                return this.restrictedBaseTypeNames;
            }
        }
    }
}
