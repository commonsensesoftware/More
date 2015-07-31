namespace More.Build.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a Microsoft Build <see cref="ITask">task</see> which gets metadata for a NuGet package from a source project.
    /// </summary>
    [CLSCompliant( false )]
    public class GetPackageMetadata : AssemblyMetadataTask
    {
        /// <summary>
        /// Populates the metadata provided by the task using the specified attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IReadOnlyList{T}">read-only list</see> of
        /// <see cref="AttributeData">attributes</see> to populate the metadata from.</param>
        protected override void PopulateMetadataFromAttributes( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Assume( attributes != null );

            SemanticVersion = attributes.GetSemanticVersion();
            Author = attributes.GetCompany();
            Description = attributes.GetDescription();
        }

        /// <summary>
        /// Gets the NuGet semantic version of the assembly referenced by the task.
        /// </summary>
        /// <value>A NuGet semantic version.</value>
        [Output]
        public string SemanticVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the author of the assembly referenced by the task.
        /// </summary>
        /// <value>The assembly author.</value>
        [Output]
        public string Author
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description of the assembly referenced by the task.
        /// </summary>
        /// <value>The assembly description.</value>
        [Output]
        public string Description
        {
            get;
            private set;
        }
    }
}
