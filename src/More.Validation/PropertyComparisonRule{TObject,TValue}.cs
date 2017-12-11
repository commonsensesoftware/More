namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    sealed class PropertyComparisonRule<TObject, TValue> : PropertyRule<TObject, TValue> where TValue : IComparable<TValue>
    {
        readonly Func<TValue, TValue, bool> valid;
        readonly Func<string, string, string> format;

        private PropertyComparisonRule( PropertyComparisonRule<TObject, TValue> other, TObject instance ) : base( other, instance )
        {
            Contract.Requires( other != null );

            valid = other.valid;
            format = other.format;
        }

        internal PropertyComparisonRule( Expression<Func<TObject, TValue>> propertyExpression, Func<TValue, TValue, bool> valid, Func<string, string, string> format ) : base( propertyExpression )
        {
            Contract.Requires( propertyExpression != null );
            Contract.Requires( valid != null );
            Contract.Requires( format != null );

            this.valid = valid;
            this.format = format;
        }

        internal PropertyComparisonRule( Expression<Func<TObject, TValue>> propertyExpression, Func<TValue, TValue, bool> valid, string errorMessage ) : base( propertyExpression, errorMessage )
        {
            Contract.Requires( propertyExpression != null );
            Contract.Requires( valid != null );

            this.valid = valid;
            format = ( s1, s2 ) => null;
        }

        protected override IValidationResult Evaluate( Property<TValue> sourceProperty, Property<TValue> targetProperty )
        {
            if ( sourceProperty == null || targetProperty == null )
            {
                return ValidationResult.Success;
            }

            if ( valid( sourceProperty.Value, targetProperty.Value ) )
            {
                return ValidationResult.Success;
            }

            var message = ErrorMessage ?? format( sourceProperty.Name, targetProperty.Name );
            return new ValidationResult( message, sourceProperty.Name, targetProperty.Name );
        }

        public override IRule<Property<TValue>, IValidationResult> GetPerInstance( TObject instance ) =>
            instance == null ? this : new PropertyComparisonRule<TObject, TValue>( this, instance );
    }
}