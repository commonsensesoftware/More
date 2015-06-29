namespace More.Build.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a Microsoft Build <see cref="ITask">task</see> which gets metadata for a NuGet package from a source project.
    /// </summary>
    public class GetPackageMetadata : AssemblyMetadataTask
    {
        private static string GetSemanticVersion( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );

            // find the assembly version attributes. the "informational" flavor supports
            // semantic versioning for nuget and should be preferred if both are specified
            var metadata = from attr in attributes
                           let name = attr.AttributeClass.Name
                           where name == "AssemblyInformationalVersionAttribute" || name == "AssemblyVersionAttribute"
                           select attr;
            var attribute = metadata.FirstOrDefault();

            if ( attribute == null )
                return null;

            // both attributes have exactly one string parameter containing the version value
            return (string) attribute.ConstructorArguments[0].Value;
        }

        private static string GetAuthor( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );

            var attribute = attributes.FirstOrDefault( a => a.AttributeClass.Name == "AssemblyCompanyAttribute" );

            if ( attribute == null )
                return null;

            // attribute has exactly one string parameter containing the value
            return (string) attribute.ConstructorArguments[0].Value;
        }

        private static string GetDescription( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );

            var attribute = attributes.FirstOrDefault( a => a.AttributeClass.Name == "AssemblyCompanyAttribute" );

            if ( attribute == null )
                return null;

            // attribute has exactly one string parameter containing the value
            return (string) attribute.ConstructorArguments[0].Value;
        }

        /// <summary>
        /// Populates the metadata provided by the task using the specified attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IReadOnlyList{T}">read-only list</see> of
        /// <see cref="AttributeData">attributes</see> to populate the metadata from.</param>
        protected override void PopulateMetadataFromAttributes( IReadOnlyList<AttributeData> attributes )
        {
            Contract.Assume( attributes != null );

            this.SemanticVersion = GetSemanticVersion( attributes );
            this.Author = GetAuthor( attributes );
            this.Description = GetDescription( attributes );
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
