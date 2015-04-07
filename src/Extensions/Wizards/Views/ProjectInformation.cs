namespace More.VisualStudio.Views
{
    using EnvDTE;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Represents information about a project.
    /// </summary>
    public class ProjectInformation : MarshalByRefObject
    {
        private readonly string fileExtension;
        private readonly string projectDirectory;
        private readonly string projectPath;
        private readonly string targetDirectory;
        private readonly string targetPath;
        private readonly string intermediateDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInformation"/> class.
        /// </summary>
        /// <param name="project">The <see cref="Project">project</see> to initialize the information from.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public ProjectInformation( Project project )
        {
            Contract.Requires<ArgumentNullException>( project != null, "project" );

            // note: currently only considering C# and VB; refactor to support other languages as needed
            this.fileExtension = project.IsVisualBasic() ? ".vb" : ".cs";
            this.projectDirectory = Path.GetDirectoryName( project.FullName );
            this.projectPath = project.FullName;
            this.targetDirectory = project.GetTargetDirectory();
            this.targetPath = project.GetTargetPath();
            this.intermediateDirectory = project.GetIntermediateDirectory();
        }

        /// <summary>
        /// Gets the project file extension.
        /// </summary>
        /// <value>The project file extension.</value>
        public string FileExtension
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.fileExtension ) );
                return this.fileExtension;
            }
        }

        /// <summary>
        /// Gets the project directory.
        /// </summary>
        /// <value>The directory for the project.</value>
        public string ProjectDirectory
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.projectDirectory ) );
                return this.projectDirectory;
            }
        }

        /// <summary>
        /// Gets the project path.
        /// </summary>
        /// <value>THe project path.</value>
        public string ProjectPath
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.projectPath ) );
                return this.projectPath;
            }
        }

        /// <summary>
        /// Gets the project target output directory.
        /// </summary>
        /// <value>The target output directory for the project.</value>
        public string TargetDirectory
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.targetDirectory ) );
                return this.targetDirectory;
            }
        }

        /// <summary>
        /// Gets the project target path to the output assembly.
        /// </summary>
        /// <value>THe project target path to the output assembly.</value>
        public string TargetPath
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.targetPath ) );
                return this.targetPath;
            }
        }

        /// <summary>
        /// Gets the project intermediate directory.
        /// </summary>
        /// <value>The project intermediate directory.</value>
        public string IntermediateDirectory
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.intermediateDirectory ) );
                return this.intermediateDirectory;
            }
        }
    }
}
