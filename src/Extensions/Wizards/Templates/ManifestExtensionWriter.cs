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
    internal sealed class ManifestExtensionWriter
    {
        private static readonly XNamespace ManifestXmlns = (XNamespace) "http://schemas.microsoft.com/appx/2010/manifest";
        private static readonly XName ExtensionName = ManifestXmlns + "Extension";
        private static readonly IReadOnlyList<Tuple<string, string, Func<XElement>>> mappings = new[]
        {
            new Tuple<string, string, Func<XElement>>( "$enableOpenFile$", "windows.fileOpenPicker", () => CreateExtension( "windows.fileOpenPicker", "FileOpenPicker" ) ),
            new Tuple<string, string, Func<XElement>>( "$enableSaveFile$", "windows.fileSavePicker", () => CreateExtension( "windows.fileSavePicker", "FileSavePicker" ) ),
            new Tuple<string, string, Func<XElement>>( "$enableAppSharing$", "windows.shareTarget", () => CreateExtension( "windows.shareTarget", "ShareTarget" ) ),
            new Tuple<string, string, Func<XElement>>( "$enableSearch$", "windows.search", () => new XElement( ExtensionName, new XAttribute( "Category", "windows.search" ) ) ),
            new Tuple<string, string, Func<XElement>>( "$enableAppSearch$", "windows.search", () => new XElement( ExtensionName, new XAttribute( "Category", "windows.search" ) ) ),
        };
        private readonly IServiceProvider serviceProvider;

        internal ManifestExtensionWriter( IServiceProvider serviceProvider )
        {
            Contract.Requires( serviceProvider != null );
            this.serviceProvider = serviceProvider;
        }

        private static ProjectItem GetAppxManifest( Project project )
        {
            Contract.Requires( project != null );

            var projectItems = project.ProjectItems.Cast<ProjectItem>();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var manifest = projectItems.FirstOrDefault( i => comparer.Equals( Path.GetExtension( i.Name ), ".appxmanifest" ) );

            return manifest;
        }

        private static bool GetOrCreateExtensionsElement( XDocument package, out XElement extensions )
        {
            Contract.Requires( package != null );
            Contract.Ensures( Contract.ValueAtReturn( out extensions ) != null );

            var application = package.Descendants( ManifestXmlns + "Application" ).Single();
            var name = ManifestXmlns + "Extensions";

            extensions = application.Element( name );

            if ( extensions != null )
                return false;

            extensions = new XElement( name );
            application.Add( extensions );
            return true;
        }

        private static XElement CreateExtension( string category, string kind )
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

        private static bool AddExtension( XElement extensions, Func<string, bool> resolveOption, Tuple<string, string, Func<XElement>> mapping )
        {
            Contract.Requires( extensions != null );
            Contract.Requires( resolveOption != null );
            Contract.Requires( mapping != null );

            var option = mapping.Item1;

            // the option is not enabled
            if ( !resolveOption( option ) )
                return false;

            var category = mapping.Item2;

            // the extension is already configured
            if ( extensions.Elements( ExtensionName ).Any( e => ( (string) e.Attribute( "Category" ) ) == category ) )
                return false;

            // create and add extension
            var factory = mapping.Item3;
            var extension = factory();

            extensions.Add( extension );
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
            Contract.Requires<ArgumentNullException>( project != null, "project" );
            Contract.Requires<ArgumentNullException>( resolveOption != null, "resolveOption" );

            var manifest = GetAppxManifest( project );

            // exit if there is no manifest file
            if ( manifest == null )
                return;

            var fileName = manifest.FileNames[1];
            var package = XDocument.Load( fileName );
            XElement extensions;
            var changed = GetOrCreateExtensionsElement( package, out extensions );

            // add extensions based on mappings
            foreach ( var mapping in mappings )
                changed |= AddExtension( extensions, resolveOption, mapping );

            // exit if there are no changes or extensions
            if ( !changed || !extensions.HasElements )
                return;

            var service = (IVsQueryEditQuerySave2) this.serviceProvider.GetService( typeof( SVsQueryEditQuerySave ) );
            var files = new [] { fileName };
            var editFlags = 2U; // tagVSQueryEditFlags. QEF_DisallowInMemoryEdits
            uint verdict;
            uint moreInfo;

            // save changes. inform visual studio that the file is about to edited by querying the edit state. if the file
            // is under source control, appropriate actions and/or prompts will be issued to check the file out.
            if ( ( service.QueryEditFiles( editFlags, 1, files, null, null, out verdict, out moreInfo ) == 0 ) && ( verdict == 0U ) )
                package.Save( fileName );
        }
    }
}
