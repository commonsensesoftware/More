namespace More.VisualStudio
{
    using EnvDTE;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a project item name validator.
    /// </summary>
    internal sealed class ProjectItemNameValidator : IProjectItemNameValidator
    {
        private const string PhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        private const string PhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        private readonly Project project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItemNameValidator" /> class.
        /// </summary>
        /// <param name="project">The <see cref="Project">project</see> to validate uniqueness in.</param>
        internal ProjectItemNameValidator( Project project )
        {
            this.project = project;
        }

        /// <summary>
        /// Returns a value indicating whether the specified item name is unique.
        /// </summary>
        /// <param name="itemName">The item name to evaluate.</param>
        /// <returns>True if the item name is unique; otherwise, false.</returns>
        public bool IsItemNameUnique( string itemName )
        {
            // short-circuit when possible
            if ( string.IsNullOrWhiteSpace( itemName ) )
                return false;
            else if ( this.project == null )
                return true;

            var comparer = StringComparer.OrdinalIgnoreCase;
            var projectItems = new Queue<ProjectItem>();

            // add initial items for evaluation
            foreach ( var item in this.project.ProjectItems.OfType<ProjectItem>() )
                projectItems.Enqueue( item );

            while ( projectItems.Count > 0 )
            {
                var current = projectItems.Dequeue();

                switch ( current.Kind )
                {
                    case PhysicalFile:
                        {
                            var otherItemName = Path.GetFileNameWithoutExtension( current.Name );

                            // not unique if the specified item matches an existing item
                            if ( comparer.Equals( itemName, otherItemName ) )
                                return false;
                            
                            break;
                        }
                    case PhysicalFolder:
                        {
                            // add child items for evaluation
                            foreach ( var item in current.ProjectItems.OfType<ProjectItem>() )
                                projectItems.Enqueue( item );

                            break;
                        }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the specified connection string name is unique.
        /// </summary>
        /// <param name="name">The connection string name to evaluate.</param>
        /// <returns>True if the connection string name is unique; otherwise, false.</returns>
        public bool IsConnectionStringNameUnique( string name )
        {
            // short-circuit when possible
            if ( string.IsNullOrWhiteSpace( name ) )
                return false;
            else if ( this.project == null )
                return true;

            var fileName = this.project.GetConfigurationFileName();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var configFile = this.project.ProjectItems.Cast<ProjectItem>().FirstOrDefault( pi => comparer.Equals( pi.Name, fileName ) );

            // a *.config file doesn't exist
            if ( configFile == null )
                return true;

            var path = configFile.FileNames[1];
            var xml = XDocument.Load( path );
            var configuration = xml.Root;
            var connectionStrings = configuration.Element( "connectionStrings" );

            // no connection string
            if ( connectionStrings == null )
                return true;

            var items = from add in connectionStrings.Elements( "add" )
                        let otherName = (string) add.Attribute( "name" )
                        where comparer.Equals( name, otherName )
                        select add;

            return items.Any();
        }
    }
}
