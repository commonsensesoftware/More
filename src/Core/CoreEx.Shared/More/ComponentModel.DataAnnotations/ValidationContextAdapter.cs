namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    internal sealed class ValidationContextAdapter : IValidationContext
    {
        private readonly ValidationContext adapted;

        internal ValidationContextAdapter( ValidationContext adapted )
        {
            Contract.Requires( adapted != null );
            this.adapted = adapted;
        }

        public string DisplayName
        {
            get
            {
                return adapted.DisplayName;
            }
            set
            {
                adapted.DisplayName = value;
            }
        }

        public IDictionary<object, object> Items
        {
            get
            {
                return adapted.Items;
            }
        }

        public string MemberName
        {
            get
            {
                return adapted.MemberName;
            }
            set
            {
                adapted.MemberName = value;
            }
        }

        public object ObjectInstance
        {
            get
            {
                return adapted.ObjectInstance;
            }
        }

        public Type ObjectType
        {
            get
            {
                return adapted.ObjectType;
            }
        }

        public object GetService( Type serviceType )
        {
            if ( typeof( ValidationContext ).Equals( serviceType ) )
                return adapted;

            return adapted.GetService( serviceType );
        }
    }
}
