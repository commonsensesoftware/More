namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Defines the behavior of the context in which a validation check is performed.
    /// </summary>
    [ContractClass( typeof( IValidationContextContract ) )]
    public interface IValidationContext : IServiceProvider
    {
        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <value>The name of the member to validate.</value>
        string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the dictionary of key/value pairs that is associated with this context.
        /// </summary>
        /// <value>The dictionary of the key/value pairs for this context.</value>
        IDictionary<object, object> Items
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <value>The name of the member to validate.</value>
        string MemberName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the object to validate.
        /// </summary>
        /// <value>The object to validate.</value>
        object ObjectInstance
        {
            get;
        }

        /// <summary>
        /// Gets the type of the object to validate.
        /// </summary>
        /// <value>The type of the object to validate.</value>
        Type ObjectType
        {
            get;
        }
    }
}
