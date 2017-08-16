namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IValidationContext ) )]
    abstract class IValidationContextContract : IValidationContext
    {
        string IValidationContext.DisplayName { get; set; }

        IDictionary<object, object> IValidationContext.Items
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<object, object>>() != null );
                return null;
            }
        }

        string IValidationContext.MemberName { get; set; }

        object IValidationContext.ObjectInstance
        {
            get
            {
                Contract.Ensures( Contract.Result<object>() != null );
                return null;
            }
        }

        Type IValidationContext.ObjectType
        {
            get
            {
                Contract.Ensures( Contract.Result<Type>() != null );
                return null;
            }
        }

        object IServiceProvider.GetService( Type serviceType ) => null;
    }
}