namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents an object which applies extensions to an application manifest (*.appxmanifest) file.
    /// </summary>
    sealed class ManifestExtensionWriter
    {
        static readonly XNamespace ManifestXmlns = (XNamespace) "http://schemas.microsoft.com/appx/2010/manifest";
        static readonly XName ExtensionName = ManifestXmlns + "Extension";
        static readonly IReadOnlyList<TokenMapping> mappings = new[]
        {
            new TokenMapping( "$enableAppSharing$", "windows.shareTarget", () => CreateExtension( "windows.shareTarget", "ShareTarget" ) ),
            new TokenMapping( "$enableSearch$", "windows.search", () => new XElement( ExtensionName, new XAttribute( "Category", "windows.search" ) ) ),
            new TokenMapping( "$enableAppSearch$", "windows.search", () => new XElement( ExtensionName, new XAttribute( "Category", "windows.search" ) ) )
        };
        readonly IServiceProvider serviceProvider;

        internal ManifestExtensionWriter( IServiceProvider serviceProvider ) => this.serviceProvider = serviceProvider;

        static ProjectItem GetAppxManifest( Project project )
        {
            Contract.Requires( project != null );

            var projectItems = project.ProjectItems.Cast<ProjectItem>();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var manifest = projectItems.FirstOrDefault( i => comparer.Equals( Path.GetExtension( i.Name ), ".appxmanifest" ) );

            return manifest;
        }

        static bool GetOrCreateExtensionsElement( XDocument package, out XElement extensions )
        {
            Contract.Requires( package != null );
            Contract.Ensures( Contract.ValueAtReturn( out extensions ) != null );

            var application = package.Descendants( ManifestXmlns + "Application" ).Single();
            var name = ManifestXmlns + "Extensions";

            extensions = application.Element( name );

            if ( extensions != null )
            {
                return false;
            }

            extensions = new XElement( name );
            application.Add( extensions );
            return true;
        }

        static XElement CreateExtension( string category, string kind )
        {
            Contract.Requires( !string.IsNullOrEmpty( category ) );
            Contract.Requires( !string.IsNullOrEmpty( kind ) );
            Contract.Ensures( Contract.Result<XElement>() != null );

            return new XElement(
                    ExtensionName,
                    new XAttribute( "Category", category ),
                    new XElement( ManifestXmlns + kind,
                        new XElement( ManifestXmlns + "SupportedFileTypes",
                            new XElement( ManifestXmlns + "SupportsAnyFileType" ) ) ) );
        }

        static bool AddExtension( XElement extensions, Func<string, bool> resolveOption, TokenMapping    mapping )
        {
            Contract.Requires( extensions != null );
            Contract.Requires( resolveOption != null );
            Contract.Requires( mapping != null );

            if ( !resolveOption( mapping.Key ) )
            {
                return false;
            }

            if ( extensions.Elements( ExtensionName ).Any( e => ( (string) e.Attribute( "Category" ) ) == mapping.Category ) )
            {
                return false;
            }

            extensions.Add( mapping.NewElement() );
            return true;
        }

        /// <summary>
        /// Applies extensions to the application manifest file in the specified project, if it exists.
        /// </summary>
        /// <param name="project">The <see cref="Project">project</see> to apply the application manifest extensions to.</param>
        /// <param name="resolveOption">The <see cref="Func{T1,TResult}">function</see> used to resolve extension options.</param>
        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intentionally left as an instance member." )]
        internal void ApplyExtensions( Project project, Func<string, bool> resolveOption )
        {
            Arg.NotNull( project, nameof( project ) );
            Arg.NotNull( resolveOption, nameof( resolveOption ) );

            var manifest = GetAppxManifest( project );

            if ( manifest == null )
            {
                return;
            }

            var fileName = manifest.FileNames[1];
            var package = XDocument.Load( fileName );
            var changed = GetOrCreateExtensionsElement( package, out var extensions );

            foreach ( var mapping in mappings )
            {
                changed |= AddExtension( extensions, resolveOption, mapping );
            }

            if ( !changed || !extensions.HasElements )
            {
                return;
            }

            var service = (IVsQueryEditQuerySave2) serviceProvider.GetService( typeof( SVsQueryEditQuerySave ) );
            var files = new[] { fileName };
            var editFlags = 2U; // tagVSQueryEditFlags. QEF_DisallowInMemoryEdits

            // save changes. inform visual studio that the file is about to edited by querying the edit state. if the file
            // is under source control, appropriate actions and/or prompts will be issued to check the file out.
            if ( ( service.QueryEditFiles( editFlags, 1, files, null, null, out var verdict, out var moreInfo ) == 0 ) && ( verdict == 0U ) )
            {
                package.Save( fileName );
            }
        }

        sealed class TokenMapping
        {
            internal TokenMapping( string key, string category, Func<XElement> newElement )
            {
                Key = key;
                Category = category;
                NewElement = newElement;
            }

            internal string Key { get; }

            internal string Category { get; }

            internal Func<XElement> NewElement { get; }
        }
    }
}