namespace More.ComponentModel
{
    using global::System;
    using global::System.ComponentModel;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object that describes an activity.
    /// </summary>
    public class ActivityDescriptor : IActivityDescriptor
    {
        private string id;
        private string name;
        private string desc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDescriptor"/> class.
        /// </summary>
        public ActivityDescriptor()
        {
            this.id = Guid.NewGuid().ToString();
            this.name = "Activity";
            this.desc = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDescriptor"/> class.
        /// </summary>
        /// <param name="id">The Globally Unique Identifier (GUID) for the activity.</param>
        /// <param name="name">The activity name.</param>
        /// <param name="description">The activity description.</param>
        public ActivityDescriptor( string id, string name, string description )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( id ), "id" );
            Contract.Requires<ArgumentException>( IsValidIdentifier( id ), "id" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( description != null, "description" );

            this.Id = id;
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The Globally Unique Identifier (GUID) representing the activity identifier.</value>
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( string.IsNullOrEmpty( value ), "value" );
                Contract.Requires<ArgumentException>( IsValidIdentifier( value ), "value" );
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        /// <value>The name of the activity.</value>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), "value" );
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        public string Description
        {
            get
            {
                return this.desc;
            }
            set
            {
                this.desc = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified activity identifier is valid.
        /// </summary>
        /// <param name="activityId">The Globally Unique Identifier (GUID) representing the activity identifier to validated.</param>
        /// <returns>True if the specified activity identifier is valid; otherwise, false.</returns>
        [Pure]
        public static bool IsValidIdentifier( string activityId )
        {
            if ( string.IsNullOrEmpty( activityId ) )
                return false;

            Guid id;

            if ( Guid.TryParse( activityId, out id ) )
                return !Guid.Empty.Equals( id );

            return false;
        }
    }
}
