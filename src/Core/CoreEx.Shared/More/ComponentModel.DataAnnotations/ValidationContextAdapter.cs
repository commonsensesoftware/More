namespace More.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel.DataAnnotations;
    using global::System.Diagnostics.Contracts;

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
                return this.adapted.DisplayName;
            }
            set
            {
                this.adapted.DisplayName = value;
            }
        }

        public IDictionary<object, object> Items
        {
            get
            {
                return this.adapted.Items;
            }
        }

        public string MemberName
        {
            get
            {
                return this.adapted.MemberName;
            }
            set
            {
                this.adapted.MemberName = value;
            }
        }

        public object ObjectInstance
        {
            get
            {
                return this.adapted.ObjectInstance;
            }
        }

        public Type ObjectType
        {
            get
            {
                return this.adapted.ObjectType;
            }
        }

        public object GetService( Type serviceType )
        {
            if ( typeof( ValidationContext ).Equals( serviceType ) )
                return this.adapted;

            return this.adapted.GetService( serviceType );
        }
    }
}
