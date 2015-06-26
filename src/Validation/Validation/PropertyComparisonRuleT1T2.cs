namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    internal sealed class PropertyComparisonRule<TObject, TValue> : PropertyRule<TObject, TValue> where TValue : IComparable<TValue>
    {
        private readonly Func<TValue, TValue, bool> valid;
        private readonly Func<string, string, string> format;

        private PropertyComparisonRule( PropertyComparisonRule<TObject, TValue> other, TObject instance )
            : base( other, instance )
        {
            Contract.Requires( other != null );

            this.valid = other.valid;
            this.format = other.format;
        }

        internal PropertyComparisonRule( Expression<Func<TObject, TValue>> propertyExpression, Func<TValue, TValue, bool> valid, Func<string, string, string> format )
            : base( propertyExpression )
        {
            Contract.Requires( propertyExpression != null );
            Contract.Requires( valid != null );
            Contract.Requires( format != null );

            this.valid = valid;
            this.format = format;
        }

        internal PropertyComparisonRule( Expression<Func<TObject, TValue>> propertyExpression, Func<TValue, TValue, bool> valid, string errorMessage )
            : base( propertyExpression, errorMessage )
        {
            Contract.Requires( propertyExpression != null );
            Contract.Requires( valid != null );

            this.valid = valid;
            this.format = ( s1, s2 ) => null;
        }

        protected override IValidationResult Evaluate( Property<TValue> sourceProperty, Property<TValue> targetProperty )
        {
            if ( sourceProperty == null || targetProperty == null )
                return ValidationResult.Success;

            if ( this.valid( sourceProperty.Value, targetProperty.Value ) )
                return ValidationResult.Success;

            var message = this.ErrorMessage ?? this.format( sourceProperty.Name, targetProperty.Name );
            return new ValidationResult( message, sourceProperty.Name, targetProperty.Name );
        }

        public override IRule<Property<TValue>, IValidationResult> GetPerInstance( TObject instance )
        {
            return instance == null ? this : new PropertyComparisonRule<TObject, TValue>( this, instance );
        }
    }
}
