namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the context in which a validation check is performed.
    /// </summary>
    public class ValidationContext : IValidationContext
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Lazy<IDictionary<object, object>> items;
        private Type objectType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="items">A user-defined <see cref="IDictionary{TKey,TValue}">collection</see> of items.</param>
        public ValidationContext( object instance, IDictionary<object, object> items )
            : this( instance, ServiceProvider.Current, items )
        {
            Arg.NotNull( instance, "instance" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> associated with the validation context.</param>
        /// <param name="items">A user-defined <see cref="IDictionary{TKey,TValue}">collection</see> of items.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with a code contract" )]
        public ValidationContext( object instance, IServiceProvider serviceProvider, IDictionary<object, object> items )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( serviceProvider, "serviceProvider" );

            var dict = items;

            this.serviceProvider = serviceProvider;
            this.items = new Lazy<IDictionary<object, object>>( () => dict ?? new Dictionary<object, object>() );
            this.ObjectInstance = instance;
            this.ObjectType = instance.GetType();
        }

        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <value>The name of the member to validate.</value>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the dictionary of key/value pairs that is associated with this context.
        /// </summary>
        /// <value>The dictionary of the key/value pairs for this context.</value>
        public IDictionary<object, object> Items
        {
            get
            {
                return this.items.Value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <value>The name of the member to validate.</value>
        public string MemberName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the object to validate.
        /// </summary>
        /// <value>The object to validate.</value>
        public object ObjectInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the object to validate.
        /// </summary>
        /// <value>The <see cref="Type">type</see> of the object to validate.</value>
        public Type ObjectType
        {
            get
            {
                Contract.Ensures(this.objectType !=null);
                return this.objectType;
            }
            set
            {
                Arg.NotNull( value, "value" );
                this.objectType = value;
            }
        }

        /// <summary>
        /// Gets a service with the specified type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>A service of the requested <paramref name="serviceType">type</paramref> or <c>null</c>.</returns>
        public virtual object GetService( Type serviceType )
        {
            return this.serviceProvider.GetService( serviceType );
        }
    }
}
