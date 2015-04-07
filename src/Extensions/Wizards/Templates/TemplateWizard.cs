namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TemplateWizard;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    /// <summary>
    /// Represents the base implementation for a project template wizard.
    /// </summary>
    public abstract class TemplateWizard : IDisposable
    {
        private const string PhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        private bool disposed;
        private DTE dte;
        private TemplateWizardContext context;

        /// <summary>
        /// Releases the managed and unmanaged resources used by the <see cref="TemplateWizard"/> class.
        /// </summary>
        ~TemplateWizard()
        {
            this.Dispose( false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTemplateWizard"/> class.
        /// </summary>
        protected TemplateWizard()
        {
        }

        /// <summary>
        /// Gets the context associated with the template wizard.
        /// </summary>
        /// <value>The <see cref="TemplateWizardContext">context</see> associated with the <see cref="TemplateWizard">wizard</see>.</value>
        protected TemplateWizardContext Context
        {
            get
            {
                Contract.Ensures( Contract.Result<TemplateWizardContext>() != null );
                return this.context;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the wizard has been canceled.
        /// </summary>
        /// <value>True if the wizard has been canceled; otherwise, false.</value>
        public bool IsCanceled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a reference to the design-time environment (DTE) associated with the wizard.
        /// </summary>
        /// <value>The associated <see cref="DTE">design-time environment</see> object.</value>
        protected DTE DesignTimeEnvironment
        {
            get
            {
                Contract.Ensures( this.dte != null );
                return this.dte;
            }
        }

        /// <summary>
        /// Gets the solution associated with the wizard.
        /// </summary>
        /// <value>The <see cref="Solution">solution</see> associated with the wizard.</value>
        public Solution Solution
        {
            get
            {
                Contract.Ensures( Contract.Result<Solution>() != null );
                return this.DesignTimeEnvironment.Solution;
            }
        }

        /// <summary>
        /// Gets the project associated with the wizard.
        /// </summary>
        /// <value>The <see cref="Project">project</see> associated with the wizard.</value>
        public Project Project
        {
            get
            {
                return this.DesignTimeEnvironment.GetActiveProject();
            }
        }

        /// <summary>
        /// Gets the directory for the associated solution.
        /// </summary>
        /// <value>The solution directory.</value>
        public string SolutionDirectory
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.GetString( "$solutiondirectory$" ) ?? this.ProjectDirectory;
            }
        }

        /// <summary>
        /// Gets the directory for the associated project.
        /// </summary>
        /// <value>The project directory.</value>
        public string ProjectDirectory
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.GetString( "$destinationdirectory$", string.Empty );
            }
        }

        /// <summary>
        /// Gets the name of the associated solution.
        /// </summary>
        /// <value>The name of the project solution.</value>
        public string SolutionName
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.GetString( "$specifiedsolutionname$", string.Empty );
            }
        }

        /// <summary>
        /// Gets the name of the associated project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return this.GetString( "$projectname$", string.Empty );
            }
        }

        /// <summary>
        /// Returns a replacement string provided to the wizard.
        /// </summary>
        /// <param name="key">The key of the replacement string to retrieve.</param>
        /// <param name="defaultValue">The value to use if the key is not present. The default value is <c>null</c>.</param>
        /// <returns>The replacement string with the specified <paramref name="key"/> or the provided
        /// <paramref name="defaultValue">default value</paramref> if the key is not present.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Simplified syntactic sugar. Non-overridable, non-public API." )]
        protected string GetString( string key, string defaultValue = null )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );

            string value;

            if ( this.Context.Replacements.TryGetValue( key, out value ) && !string.IsNullOrEmpty( value ) )
                return value;

            return defaultValue;
        }

        /// <summary>
        /// Returns a Boolean value from a replacement string provided to the wizard.
        /// </summary>
        /// <param name="key">The key of the replacement string to retrieve.</param>
        /// <param name="defaultValue">The value to use if the key is not present. The default value is <c>false</c>.</param>
        /// <returns>The parsed <see cref="Boolean"/> value from replacement string with the specified <paramref name="key"/> or the provided
        /// <paramref name="defaultValue">default value</paramref> if the key is not present.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Simplified syntactic sugar. Non-overridable, non-public API." )]
        protected bool GetBoolean( string key, bool defaultValue = false )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );

            var value = this.GetString( key );
            var parsed = false;

            if ( string.IsNullOrEmpty( value ) || !bool.TryParse( value, out parsed ) )
                return defaultValue;

            return parsed;
        }

        /// <summary>
        /// Returns an integer value from a replacement string provided to the wizard.
        /// </summary>
        /// <param name="key">The key of the replacement string to retrieve.</param>
        /// <param name="defaultValue">The value to use if the key is not present. The default value is <c>0</c>.</param>
        /// <returns>The parsed <see cref="Int32">integer</see> value from replacement string with the specified <paramref name="key"/> or the provided
        /// <paramref name="defaultValue">default value</paramref> if the key is not present.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Simplified syntactic sugar. Non-overridable, non-public API." )]
        protected int GetInt32( string key, int defaultValue = 0 )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );

            var value = this.GetString( key );
            var parsed = 0;

            if ( string.IsNullOrEmpty( value ) || !int.TryParse( value, out parsed ) )
                return defaultValue;

            return parsed;
        }

        /// <summary>
        /// Returns a list of strings from a replacement string provided to the wizard.
        /// </summary>
        /// <param name="key">The key of the replacement string to retrieve.</param>
        /// <returns>A <see cref="IList{T}">list</see> of strings from replacement string
        /// with the specified <paramref name="key"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Simplified syntactic sugar. Non-overridable, non-public API." )]
        protected IList<string> GetStrings( string key )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
            Contract.Ensures( Contract.Result<IList<string>>() != null );

            var value = this.GetString( key );

            if ( !string.IsNullOrEmpty( value ) )
                value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToList();

            return new List<string>();
        }

        /// <summary>
        /// Gets or creates a folder in the provided project with the specified name.
        /// </summary>
        /// <param name="project">The <see cref="Project">project</see> to get or create the folder from.</param>
        /// <param name="folderName">The name of the folder to get or create.</param>
        /// <returns>A <see cref="ProjectItem"/> representing the requested folder.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected static ProjectItem GetOrCreateFolder( Project project, string folderName )
        {
            Contract.Requires<ArgumentNullException>( project != null, "project" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( folderName ), "folderName" );
            Contract.Ensures( Contract.Result<ProjectItem>() != null );

            // find or create requested folder
            var items = project.ProjectItems;
            var folders = items.OfType<ProjectItem>().Where( i => i.Kind == PhysicalFolder && i.Name == folderName );
            var folder = folders.SingleOrDefault() ?? items.AddFolder( folderName );

            return folder;
        }

        /// <summary>
        /// Adds one or more files from a template.
        /// </summary>
        /// <param name="templateName">The name of the template to add the files from.</param>
        /// <param name="fileName">The name of the new file generated from template.</param>
        /// <param name="language">The language to generate files for (ex: "CSharp").</param>
        /// <returns>The generated <see cref="ProjectItem"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "C# is the only language used or support at this time. Will reconsider in future release." )]
        protected ProjectItem AddFromTemplate( string templateName, string fileName, string language )
        {
            return this.AddFromTemplate( null, templateName, fileName, language );
        }

        /// <summary>
        /// Adds one or more files from a template.
        /// </summary>
        /// <param name="folderName">The name of the folder in the project add the files to.</param>
        /// <param name="templateName">The name of the template to add the files from.</param>
        /// <param name="fileName">The name of the new file generated from template.</param>
        /// <param name="language">The language to generate files for (ex: "CSharp").</param>
        /// <returns>The generated <see cref="ProjectItem"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "C# is the only language used or support at this time. Will reconsider in future release." )]
        protected ProjectItem AddFromTemplate( string folderName, string templateName, string fileName, string language )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( templateName ), "templateName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( fileName ), "fileName" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( language ), "language" );

            var subitem = !string.IsNullOrEmpty( folderName );

            // provide user feedback
            var format = subitem ? SR.AddTemplateSubItem : SR.AddTemplateItem;
            this.DesignTimeEnvironment.StatusBar.Text = format.FormatDefault( fileName, folderName );

            var solution = (EnvDTE80.Solution2) this.Solution;
            var templatePath = solution.GetProjectItemTemplate( templateName, language );
            var projectItems = subitem ? GetOrCreateFolder( this.Project, folderName ).ProjectItems : this.Project.ProjectItems;

            return projectItems.AddFromTemplate( templatePath, fileName );
        }

        /// <summary>
        /// Releases the managed and, optionally, the unmanaged resources used by the <see cref="TemplateWizard"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether the object is being disposed.</param>
        protected virtual void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            this.dte = null;

            if ( this.context == null )
                return;

            this.context.Dispose();
            this.context = null;
        }

        /// <summary>
        /// Releases the managed resources used by the <see cref="TemplateWizard"/> class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Attempts to run the template wizard.
        /// </summary>
        /// <param name="shell">The <see cref="IVsUIShell">shell</see> associated with the wizard.</param>
        /// <returns>True if the wizard completed successfully; otherwise, false if the wizard was canceled.</returns>
        protected virtual bool TryRunWizard( IVsUIShell shell )
        {
            Contract.Requires<ArgumentNullException>( shell != null, "shell" );
            return true;
        }

        /// <summary>
        /// Occurs when the wizard has been canceled.
        /// </summary>
        protected virtual void OnCanceled()
        {
            this.IsCanceled = true;
            this.Context.Abandon();
        }

        /// <summary>
        /// Occurs when the template wizard has completed.
        /// </summary>
        /// <remarks>This implementation performs no action.</remarks>
        public virtual void RunFinished()
        {
            this.Context.Abandon();
        }

        /// <summary>
        /// Occurs when this template wizard is executed.
        /// </summary>
        /// <param name="automationObject">An automation <see cref="Object">object</see> representing the Visual Studio runtime environment.</param>
        /// <param name="replacementsDictionary">A <see cref="Dictionary{TKey,TValue}">dictionary</see> containing the replacements provided to the template wizard.</param>
        /// <param name="runKind">One of the <see cref="WizardRunKind"/> values.</param>
        /// <param name="customParams">An array of <see cref="Type">type</see> <see cref="Object">object</see> containing custom parameters.  This parameter is for legacy
        /// support and should not be used.</param>
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The wizard should always exit gracefully." )]
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "Matches method signature of Microsoft.VisualStudio.TemplateWizard.IWizard.RunStarted." )]
        [SuppressMessage( "Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Matches method signature of Microsoft.VisualStudio.TemplateWizard.IWizard.RunStarted." )]
        public void RunStarted( object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams )
        {
            this.dte = (DTE) automationObject;
            this.dte.StatusBar.Clear();
            this.context = new TemplateWizardContext( (IOleServiceProvider) automationObject, replacementsDictionary );
            this.context.AddService( typeof( IProjectItemNameValidator ), ( s, t ) => new ProjectItemNameValidator( this.Project ) );

            var shell = this.context.GetRequiredService<IVsUIShell>();

            try
            {
                // try running the wizard. most errors should be handled by the wizard, but a few (such as XAML parsing errors),
                // might not get handled. ensure they are always handled here
                if ( this.TryRunWizard( shell ) )
                    return;
            }
            catch ( Exception ex )
            {
                var aggregateEx = ex as AggregateException;

                if ( aggregateEx != null )
                    ex = aggregateEx.GetBaseException();

                // this might happen if there is something wrong in the XAML. all other errors should be handled by the view model
                shell.ShowError( ExceptionMessage.TemplateWizardErrorCaption, ex.Message );
            }

            this.dte.StatusBar.Clear();
            this.OnCanceled();
        }
    }
}
