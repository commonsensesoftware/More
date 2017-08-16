namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object that describes an activity.
    /// </summary>
    public class ActivityDescriptor : IActivityDescriptor
    {
        string id;
        string name;
        string desc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDescriptor"/> class.
        /// </summary>
        public ActivityDescriptor()
        {
            id = Guid.NewGuid().ToString();
            name = "Activity";
            desc = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityDescriptor"/> class.
        /// </summary>
        /// <param name="id">The Globally Unique Identifier (GUID) for the activity.</param>
        /// <param name="name">The activity name.</param>
        /// <param name="description">The activity description.</param>
        public ActivityDescriptor( string id, string name, string description )
        {
            Arg.NotNullOrEmpty( id, nameof( id ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNull( description, nameof( description ) );

            if ( !IsValidIdentifier( id ) )
            {
                throw new ArgumentException( ExceptionMessage.InvalidActivityId, nameof( id ) );
            }

            Id = id;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The Globally Unique Identifier (GUID) representing the activity identifier.</value>
        public string Id
        {
            get => id;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );

                if ( !IsValidIdentifier( value ) )
                {
                    throw new ArgumentException( ExceptionMessage.InvalidActivityId, nameof( value ) );
                }

                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        /// <value>The name of the activity.</value>
        public string Name
        {
            get => name;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        public string Description
        {
            get => desc;
            set => desc = value ?? string.Empty;
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
            {
                return false;
            }

            if ( Guid.TryParse( activityId, out var id ) )
            {
                return !Guid.Empty.Equals( id );
            }

            return false;
        }
    }
}