namespace More.Build.Tasks
{
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents the base implemenation for a Microsoft Build <see cref="ITask">task</see> which gets metadata from a source project.
    /// </summary>
    public abstract class AssemblyMetadataTask : ITask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyMetadataTask"/> class.
        /// </summary>
        protected AssemblyMetadataTask()
        {
        }

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
        /// Gets or sets a value indicating whether the task is valid.
        /// </summary>
        /// <value>True if the task is valid; otherwise, false. The default value is <c>true</c>.</value>
        /// <remarks>This proeprty is used to determine whether the task executed successfully.</remarks>
        public bool IsValid
        {
            get;
            protected set;
        }

        /// <summary>
        /// Populates the metadata provided by the task using the specified attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IReadOnlyList{T}">read-only list</see> of
        /// <see cref="AttributeData">attributes</see> to populate the metadata from.</param>
        protected abstract void PopulateMetadataFromAttributes( IReadOnlyList<AttributeData> attributes );

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
                var references = new[] { MetadataReference.CreateFromFile( typeof( object ).Assembly.Location ) };
                var compilation = CSharpCompilation.Create( project.GetPropertyValue( "AssemblyName" ), assemblyInfos, references );
                var attributes = compilation.Assembly.GetAttributes().ToArray();

                this.IsValid = true;
                this.PopulateMetadataFromAttributes( attributes );
            }
            finally
            {
                ProjectCollection.GlobalProjectCollection.UnloadProject( project );
            }

            return this.IsValid;
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
    }
}
