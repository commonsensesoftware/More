namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of an object that describes an activity.
    /// </summary>
    [ContractClass( typeof( IActivityDescriptorContract ) )]
    public interface IActivityDescriptor
    {
        /// <summary>
        /// Gets the activity identifier.
        /// </summary>
        /// <value>The Globally Unique Identifier (GUID) representing the activity identifier.</value>
        string Id
        {
            get;
        }

        /// <summary>
        /// Gets the name of the activity.
        /// </summary>
        /// <value>The name of the activity.</value>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        string Description
        {
            get;
        }
    }
}
