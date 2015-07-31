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
            Arg.NotNull( project, nameof( project ) );

            // note: currently only considering C# and VB; refactor to support other languages as needed
            fileExtension = project.IsVisualBasic() ? ".vb" : ".cs";
            projectDirectory = Path.GetDirectoryName( project.FullName );
            projectPath = project.FullName;
            targetDirectory = project.GetTargetDirectory();
            targetPath = project.GetTargetPath();
            intermediateDirectory = project.GetIntermediateDirectory();
        }

        /// <summary>
        /// Gets the project file extension.
        /// </summary>
        /// <value>The project file extension.</value>
        public string FileExtension
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( fileExtension ) );
                return fileExtension;
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
                Contract.Ensures( !string.IsNullOrEmpty( projectDirectory ) );
                return projectDirectory;
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
                Contract.Ensures( !string.IsNullOrEmpty( projectPath ) );
                return projectPath;
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
                Contract.Ensures( !string.IsNullOrEmpty( targetDirectory ) );
                return targetDirectory;
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
                Contract.Ensures( !string.IsNullOrEmpty( targetPath ) );
                return targetPath;
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
                Contract.Ensures( !string.IsNullOrEmpty( intermediateDirectory ) );
                return intermediateDirectory;
            }
        }
    }
}
