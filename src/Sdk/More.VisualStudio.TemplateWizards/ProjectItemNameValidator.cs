namespace More.VisualStudio
{
    using EnvDTE;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    sealed class ProjectItemNameValidator : IProjectItemNameValidator
    {
        const string PhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        const string PhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        readonly Project project;

        internal ProjectItemNameValidator( Project project ) => this.project = project;

        public bool IsItemNameUnique( string itemName )
        {
            if ( string.IsNullOrWhiteSpace( itemName ) )
            {
                return false;
            }
            else if ( project == null )
            {
                return true;
            }

            var comparer = StringComparer.OrdinalIgnoreCase;
            var projectItems = new Queue<ProjectItem>();

            foreach ( var item in project.ProjectItems.OfType<ProjectItem>() )
            {
                projectItems.Enqueue( item );
            }

            while ( projectItems.Count > 0 )
            {
                var current = projectItems.Dequeue();

                switch ( current.Kind )
                {
                    case PhysicalFile:
                        {
                            var otherItemName = Path.GetFileNameWithoutExtension( current.Name );

                            if ( comparer.Equals( itemName, otherItemName ) )
                            {
                                return false;
                            }

                            break;
                        }
                    case PhysicalFolder:
                        {
                            foreach ( var item in current.ProjectItems.OfType<ProjectItem>() )
                            {
                                projectItems.Enqueue( item );
                            }

                            break;
                        }
                }
            }

            return true;
        }

        public bool IsConnectionStringNameUnique( string name )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                return false;
            }
            else if ( project == null )
            {
                return true;
            }

            var fileName = project.GetConfigurationFileName();
            var comparer = StringComparer.OrdinalIgnoreCase;
            var configFile = project.ProjectItems.Cast<ProjectItem>().FirstOrDefault( pi => comparer.Equals( pi.Name, fileName ) );

            if ( configFile == null )
            {
                return true;
            }

            var path = configFile.FileNames[1];
            var xml = XDocument.Load( path );
            var configuration = xml.Root;
            var connectionStrings = configuration.Element( "connectionStrings" );

            if ( connectionStrings == null )
            {
                return true;
            }

            var items = from add in connectionStrings.Elements( "add" )
                        let otherName = (string) add.Attribute( "name" )
                        where comparer.Equals( name, otherName )
                        select add;

            return items.Any();
        }
    }
}