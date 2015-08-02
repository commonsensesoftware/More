namespace $rootnamespace$
{$if$($modelNamespaceRequired$ == true)
    using $modelNamespace$;$endif$
    using More.ComponentModel;$if$($compose$ == true)
    using More.Composition;$endif$
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;$if$($compose$ == true)
    using ConnectionStringSettings = System.Configuration.ConnectionStringSettings;$endif$

    /// <summary>
    /// Represents a combination of the <see cref="IRepository{T}">Repository</see> and <see cref="IUnitOfWork{T}">Unit Of Work</see> patterns
    /// such that it can be used to query from a data store and group together changes that will then be written back to the store as a unit of
    /// work<seealso cref="IReadOnlyRepository{T}"/>.
    /// </summary>
    public partial class $safeitemrootname$ : DbContext$implementedInterfaces$
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="$safeitemrootname$"/> class.
        /// </summary>$if$($compose$ == true)
        /// <param name="settings">The <see cref="ConnectionStringSettings">connection string settings</see> for the context.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public $safeitemrootname$( [Setting( "$connectionStringKey$" )] ConnectionStringSettings settings )
            : base( settings.ConnectionString )$endif$$if$($compose$ == false)
        public $safeitemrootname$()
            : base( "name=$connectionStringKey$" )$endif$
        {
            Contract.Requires( settings != null );
            Database.SetInitializer<$safeitemrootname$>( null );
        }

        /// <summary>
        /// Overrides the default behavior when the context is creating the underlying model.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder">model builder</see> used by the current context.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The framework will never call this method with a null model builder." )]
        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            Contract.Assume( modelBuilder != null );$if$($showTips$ == true)

            // For more information on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.$endif$
        }
    }
}
