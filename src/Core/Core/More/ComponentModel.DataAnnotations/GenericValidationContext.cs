namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal sealed class GenericValidationContext : IValidationContext
    {
        private readonly Lazy<IDictionary<object, object>> items;

        internal GenericValidationContext( object instance, IDictionary<object, object> items )
        {
            Contract.Requires( instance != null );

            var dict = items;
            this.items = new Lazy<IDictionary<object, object>>( () => dict ?? new Dictionary<object, object>() );
            this.ObjectInstance = instance;
            this.ObjectType = instance.GetType();
        }

        public string DisplayName
        {
            get;
            set;
        }

        public IDictionary<object, object> Items
        {
            get
            {
                return this.items.Value;
            }
        }

        public string MemberName
        {
            get;
            set;
        }

        public object ObjectInstance
        {
            get;
            private set;
        }

        public Type ObjectType
        {
            get;
            private set;
        }

        public object GetService( Type serviceType )
        {
            return null;
        }
    }
}
