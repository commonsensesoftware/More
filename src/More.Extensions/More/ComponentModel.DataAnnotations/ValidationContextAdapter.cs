namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;

    sealed class ValidationContextAdapter : IValidationContext
    {
        readonly ValidationContext adapted;

        internal ValidationContextAdapter( ValidationContext adapted )
        {
            Contract.Requires( adapted != null );
            this.adapted = adapted;
        }

        public string DisplayName
        {
            get => adapted.DisplayName;
            set => adapted.DisplayName = value;
        }

        public IDictionary<object, object> Items => adapted.Items;

        public string MemberName
        {
            get => adapted.MemberName;
            set => adapted.MemberName = value;
        }

        public object ObjectInstance => adapted.ObjectInstance;

        public Type ObjectType => adapted.ObjectType;

        public object GetService( Type serviceType )
        {
            if ( typeof( ValidationContext ).Equals( serviceType ) )
            {
                return adapted;
            }

            return adapted.GetService( serviceType );
        }
    }
}