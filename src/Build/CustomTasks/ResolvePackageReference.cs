namespace More.Build.Tasks
{
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.MSBuild;
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents a Microsoft Build <see cref="ITask">task</see> which resolves the NuGet
    /// semantic version of a referenced source project.
    /// </summary>
    public class ResolvePackageReference : ITask
    {
        /// <summary>
        /// Gets or sets the build engine associated with the task.
        /// </summary>
        /// <value>The <see cref="IBuildEngine">build engine</see> associated with the task.</value>
        public IBuildEngine BuildEngine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any host object that is associated with the task.
        /// </summary>
        /// <value>The <see cref="ITaskHost">host object</see> associated with the task.</value>
        public ITaskHost HostObject
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>True if the task executed successfully; otherwise, false.</returns>
        public bool Execute()
        {
            var fullPath = this.SourceProjectPath;
            var basePath = Path.GetDirectoryName( fullPath );
            var project = ProjectCollection.GlobalProjectCollection.LoadProject( fullPath );

            try
            {
                var assemblyInfos = from item in project.GetItems( "Compile" )
                                    let include = item.EvaluatedInclude
                                    where include.EndsWith( "AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase )
                                    let itemPath = Path.GetFullPath( Path.Combine( basePath, include ) )
                                    let code = File.ReadAllText( itemPath )
                                    select CSharpSyntaxTree.ParseText( code );
                var references = new[] { MetadataReference.CreateFromAssembly( typeof( object ).Assembly ) };
                var compilation = CSharpCompilation.Create( project.GetPropertyValue( "AssemblyName" ), assemblyInfos, references );

                // find the assembly version attributes. the "informational" flavor supports
                // semantic versioning for nuget and should be preferred if both are specified
                var attributes = from attr in compilation.Assembly.GetAttributes()
                                 let name = attr.AttributeClass.Name
                                 where name == "AssemblyInformationalVersionAttribute" ||
                                       name == "AssemblyVersionAttribute"
                                 select attr;
                var attribute = attributes.FirstOrDefault();
                
                // both attributes have exactly one string parameter containing the version value
                this.SemanticVersion = (string) attribute.ConstructorArguments[0].Value;

                return !string.IsNullOrEmpty( this.SemanticVersion );
            }
            finally
            {
                ProjectCollection.GlobalProjectCollection.UnloadProject( project );
            }
        }

        /// <summary>
        /// Gets or sets the source project to get the NuGet package semantic version from.
        /// </summary>
        /// <value>The path of the source project.</value>
        [Required]
        public string SourceProjectPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the semantic version of NuGet package referenced by the task.
        /// </summary>
        /// <value>A NuGet semantic version.</value>
        [Output]
        public string SemanticVersion
        {
            get;
            private set;
        }
    }
}
