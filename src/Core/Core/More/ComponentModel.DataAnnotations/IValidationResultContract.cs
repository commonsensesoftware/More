namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IValidationResult ) )]
    internal abstract class IValidationResultContract : IValidationResult
    {
        string IValidationResult.ErrorMessage
        {
            get;
            set;
        }

        IEnumerable<string> IValidationResult.MemberNames
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<string>>() != null );
                return null;
            }
        }
    }
}
