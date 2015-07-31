namespace System.ComponentModel.DataAnnotations
{
    using More;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel.Design;
    using global::System.Globalization;

    /// <summary>
    /// Describes the context in which a validation check is performed.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationContext.</remarks>
    public sealed class ValidationContext : IServiceProvider
    {
        private class ValidationContextServiceContainer : IServiceContainer, IServiceProvider
        {
            private readonly object syncRoot = new object();
            private readonly IServiceContainer parentContainer;
            private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

            internal ValidationContextServiceContainer()
            {
            }

            internal ValidationContextServiceContainer( IServiceContainer parentContainer )
            {
                this.parentContainer = parentContainer;
            }

            public void AddService( Type serviceType, ServiceCreatorCallback callback, bool promote )
            {
                if ( promote && parentContainer != null )
                {
                    parentContainer.AddService( serviceType, callback, promote );
                    return;
                }

                lock ( syncRoot )
                {
                    if ( services.ContainsKey( serviceType ) )
                    {
                        var message = DataAnnotationsResources.ServiceAlreadyExists.FormatDefault( serviceType );
                        throw new ArgumentException( message, "serviceType" );
                    }

                    services.Add( serviceType, callback );
                }
            }

            public void AddService( Type serviceType, ServiceCreatorCallback callback )
            {
                AddService( serviceType, callback, true );
            }

            public void AddService( Type serviceType, object serviceInstance, bool promote )
            {
                if ( promote && parentContainer != null )
                {
                    parentContainer.AddService( serviceType, serviceInstance, promote );
                    return;
                }

                lock ( syncRoot )
                {
                    if ( services.ContainsKey( serviceType ) )
                    {
                        var message = DataAnnotationsResources.ServiceAlreadyExists.FormatDefault( serviceType );
                        throw new ArgumentException( message, "serviceType" );
                    }

                    services.Add( serviceType, serviceInstance );
                }
            }

            public void AddService( Type serviceType, object serviceInstance )
            {
                AddService( serviceType, serviceInstance, true );
            }

            public void RemoveService( Type serviceType, bool promote )
            {
                lock ( syncRoot )
                {
                    if ( services.ContainsKey( serviceType ) )
                        services.Remove( serviceType );
                }

                if ( promote && parentContainer != null )
                    parentContainer.RemoveService( serviceType );
            }

            public void RemoveService( Type serviceType )
            {
                RemoveService( serviceType, true );
            }

            public object GetService( Type serviceType )
            {
                if ( serviceType == null )
                    throw new ArgumentNullException( "serviceType" );

                object obj = null;

                services.TryGetValue( serviceType, out obj );

                if ( obj == null && parentContainer != null )
                    obj = parentContainer.GetService( serviceType );

                var serviceCreatorCallback = obj as ServiceCreatorCallback;

                if ( serviceCreatorCallback != null )
                    obj = serviceCreatorCallback( this, serviceType );

                return obj;
            }
        }

        private readonly IServiceProvider serviceProvider;
        private readonly object objectInstance;
        private readonly Dictionary<object, object> items;
        private readonly IServiceContainer serviceContainer;
        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationContext" /> class.
        /// </summary>
        /// <param name="instance">The object to validate. This parameter is required.</param>
        /// <param name="items">A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.</param>
        public ValidationContext( object instance, IDictionary<object, object> items )
            : this( instance, null, items )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationContext" /> class.
        /// </summary>
        /// <param name="instance">The object to validate. This parameter is required.</param>
        /// <param name="serviceProvider">The object that implements the <see cref="T:System.IServiceProvider" /> interface. This parameter is optional.</param>
        /// <param name="items">A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.</param>
        public ValidationContext( object instance, IServiceProvider serviceProvider, IDictionary<object, object> items )
        {
            if ( instance == null )
                throw new ArgumentNullException( "instance" );

            this.serviceProvider = serviceProvider ?? ServiceProvider.Current;
            var container = serviceProvider as IServiceContainer;

            if ( container != null )
                serviceContainer = new ValidationContextServiceContainer( container );
            else
                serviceContainer = new ValidationContextServiceContainer();

            if ( items != null )
                this.items = new Dictionary<object, object>( items );
            else
                this.items = new Dictionary<object, object>();

            objectInstance = instance;
        }

        /// <summary>
        /// Gets the object to validate.
        /// </summary>
        /// <value>The object to validate.</value>
        public object ObjectInstance
        {
            get
            {
                return objectInstance;
            }
        }

        /// <summary>
        /// Gets the type of the object to validate.
        /// </summary>
        /// <value>The type of the object to validate.</value>
        public Type ObjectType
        {
            get
            {
                return ObjectInstance.GetType();
            }
        }

        /// <summary>
        /// Gets or sets the name of the member to validate.
        /// </summary>
        /// <value>The name of the member to validate.</value>
        public string DisplayName
        {
            get
            {
                if ( string.IsNullOrEmpty( displayName ) )
                {
                    if ( string.IsNullOrEmpty( displayName = GetDisplayName() ) )
                    {
                        if ( string.IsNullOrEmpty( displayName = MemberName ) )
                            displayName = ObjectType.Name;
                    }
                }

                return displayName;
            }
            set
            {
                if ( string.IsNullOrEmpty( value ) )
                    throw new ArgumentNullException( "value" );

                displayName = value;
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
        /// Gets the dictionary of key/value pairs that is associated with this context.
        /// </summary>
        /// <value>The dictionary of the key/value pairs for this context.</value>
        public IDictionary<object, object> Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets the validation services container.
        /// </summary>
        /// <value>The validation services container.</value>
        public IServiceContainer ServiceContainer
        {
            get
            {
                return serviceContainer;
            }
        }

        private string GetDisplayName()
        {
            string text = null;
            var instance = ValidationAttributeStore.Instance;
            DisplayAttribute displayAttribute = null;

            if ( string.IsNullOrEmpty( MemberName ) )
            {
                displayAttribute = instance.GetTypeDisplayAttribute( this );
            }
            else
            {
                if ( instance.IsPropertyContext( this ) )
                    displayAttribute = instance.GetPropertyDisplayAttribute( this );
            }

            if ( displayAttribute != null )
                text = displayAttribute.GetName();

            return text ?? MemberName;
        }

        /// <summary>
        /// Returns the service that provides custom validation.
        /// </summary>
        /// <returns>An instance of the service, or null if the service is not available.</returns>
        /// <param name="serviceType">The type of the service to use for validation.</param>
        public object GetService( Type serviceType )
        {
            object obj = null;

            if ( serviceContainer != null )
                obj = serviceContainer.GetService( serviceType );

            if ( obj == null && serviceProvider != null )
                obj = serviceProvider.GetService( serviceType );

            return obj;
        }
    }
}
