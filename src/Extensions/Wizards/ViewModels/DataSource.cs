namespace More.VisualStudio.ViewModels
{
    using Microsoft.VisualStudio.Data.Services;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a data source.
    /// </summary>
    public sealed class DataSource
    {
        private readonly string displayName;
        private Lazy<IVsDataConnection> dataConnection;

        internal DataSource( string displayName, IVsDataConnection dataConnection )
            : this( displayName, new Lazy<IVsDataConnection>( () => dataConnection ) )
        {
            Contract.Requires( !string.IsNullOrEmpty( displayName ) );
        }

        internal DataSource( string displayName, Lazy<IVsDataConnection> dataConnection )
        {
            Contract.Requires( !string.IsNullOrEmpty( displayName ) );

            this.displayName = displayName;
            this.dataConnection = dataConnection;
        }

        /// <summary>
        /// Gets the data source display name.
        /// </summary>
        /// <value>The data source display name.</value>
        public string DisplayName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.displayName ) );
                return this.displayName;
            }
        }

        /// <summary>
        /// Gets the connection string associated with the data source.
        /// </summary>
        /// <value>The connection string for the data source.</value>
        public string ConnectionString
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                var value = this.Connection;
                return value == null ? string.Empty : value.DecryptedConnectionString();
            }
        }

        internal IVsDataConnection Connection
        {
            get
            {
                Contract.Ensures( Contract.Result<IVsDataConnection>() != null );
                return this.dataConnection.Value;
            }
        }
    }
}
