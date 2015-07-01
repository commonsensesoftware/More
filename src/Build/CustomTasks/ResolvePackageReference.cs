namespace More.Build.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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

            this.SemanticVersion = attributes.GetSemanticVersion();
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
