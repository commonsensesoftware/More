namespace More.Build.Tasks
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for retrieving attribute information.
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// Returns a single value from an attribute with the specified name.
        /// </summary>
        /// <param name="attributes">The <see cref="IEnumerable{T}">sequence</see> of <see cref="AttributeData">attributes</see> to search.</param>
        /// <param name="attributeName">The name of the attribute to find.</param>
        /// <returns>The single value from the attribute's constructor or <c>null</c> if no match is found.</returns>
        [CLSCompliant( false )]
        public static string GetSingleAttributeValue( this IEnumerable<AttributeData> attributes, string attributeName )
        {
            Contract.Requires( attributes != null );
            Contract.Requires( !string.IsNullOrEmpty( attributeName ) );

            var attribute = attributes.FirstOrDefault( a => a.AttributeClass.Name == attributeName );

            if ( attribute == null )
                return null;

            // attribute has exactly one string parameter containing the value
            return (string) attribute.ConstructorArguments[0].Value;
        }

        /// <summary>
        /// Returns the assembly version from the set of attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IEnumerable{T}">sequence</see> of <see cref="AttributeData">attributes</see> to search.</param>
        /// <returns>The resolved assembly version or <c>null</c>.</returns>
        [CLSCompliant( false )]
        public static string GetDescription( this IEnumerable<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );
            return attributes.GetSingleAttributeValue( "AssemblyDescriptionAttribute" );
        }

        /// <summary>
        /// Returns the assembly informational version from the set of attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IEnumerable{T}">sequence</see> of <see cref="AttributeData">attributes</see> to search.</param>
        /// <returns>The resolved assembly informational version or <c>null</c>.</returns>
        [CLSCompliant( false )]
        public static string GetCompany( this IEnumerable<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );
            return attributes.GetSingleAttributeValue( "AssemblyCompanyAttribute" );
        }

        /// <summary>
        /// Returns the semantic version from the set of attributes.
        /// </summary>
        /// <param name="attributes">The <see cref="IEnumerable{T}">sequence</see> of <see cref="AttributeData">attributes</see> to search.</param>
        /// <returns>The resolved semantic version or <c>null</c>.</returns>
        [CLSCompliant( false )]
        public static string GetSemanticVersion( this IEnumerable<AttributeData> attributes )
        {
            Contract.Requires( attributes != null );

            var version = attributes.GetSingleAttributeValue( "AssemblyInformationalVersionAttribute" );

            if ( string.IsNullOrEmpty( version ) )
                version = attributes.GetSingleAttributeValue( "AssemblyVersionAttribute" );

            return version;
        }
    }
}
