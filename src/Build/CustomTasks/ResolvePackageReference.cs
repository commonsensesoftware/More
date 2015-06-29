namespace More.Build.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a Microsoft Build <see cref="ITask">task</see> which resolves the NuGet
    /// semantic version of a referenced source project.
    /// </summary>
    public class ResolvePackageReference : AssemblyMetadataTask
    {
        /// <summary>
        /// Populates the metadata provided by the task using the specified attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IReadOnlyList{T}">read-only list</see> of
        /// <see cref="AttributeData">attributes</see> to populate the metadata from.</param>
        protected override void PopulateMetadataFromAttributes( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Assume( attributes != null );

            // find the assembly version attributes. the "informational" flavor supports
            // semantic versioning for nuget and should be preferred if both are specified
            var metadata = from attr in attributes
                           let name = attr.AttributeClass.Name
                           where name == "AssemblyInformationalVersionAttribute" || name == "AssemblyVersionAttribute"
                           select attr;
            var attribute = metadata.FirstOrDefault();
            
            if ( attribute == null )
                this.SemanticVersion = null;
            else
                // both attributes have exactly one string parameter containing the version value
                this.SemanticVersion = (string) attribute.ConstructorArguments[0].Value;

            this.IsValid = !string.IsNullOrEmpty( this.SemanticVersion );
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
