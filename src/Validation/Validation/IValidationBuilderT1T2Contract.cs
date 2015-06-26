namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IValidationBuilder<,> ) )]
    internal abstract class IValidationBuilderContract<TObject, TValue> : IValidationBuilder<TObject, TValue>
    {
        IValidationBuilder<TObject, TValue> IValidationBuilder<TObject, TValue>.Apply( IRule<Property<TValue>, IValidationResult> rule )
        {
            Contract.Requires<ArgumentNullException>( rule != null, "rule" );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator<TObject>.ValidateObject( TObject instance )
        {
            return null;
        }

        string IPropertyValidator.PropertyName
        {
            get
            {
                return null;
            }
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance )
        {
            return null;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value )
        {
            return null;
        }
    }
}
