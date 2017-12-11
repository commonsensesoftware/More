namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IValidationBuilder<,> ) )]
    abstract class IValidationBuilderContract<TObject, TValue> : IValidationBuilder<TObject, TValue>
    {
        IValidationBuilder<TObject, TValue> IValidationBuilder<TObject, TValue>.Apply( IRule<Property<TValue>, IValidationResult> rule )
        {
            Contract.Requires<ArgumentNullException>( rule != null, nameof( rule ) );
            Contract.Ensures( Contract.Result<IValidationBuilder<TObject, TValue>>() != null );
            return null;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator<TObject>.ValidateObject( TObject instance ) => null;

        string IPropertyValidator.PropertyName => null;

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance ) => null;

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value ) => null;
    }
}