namespace More.ComponentModel.DataAnnotations
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    sealed class CovariantObjectValidator : IObjectValidator
    {
        readonly IReadOnlyList<IObjectValidator> validators;

        internal CovariantObjectValidator( IReadOnlyList<IObjectValidator> validators )
        {
            Contract.Requires( validators != null );
            this.validators = validators;
        }

        public IReadOnlyList<IValidationResult> ValidateObject( object instance )
        {
            Arg.NotNull( instance, nameof( instance ) );

            var results = new List<IValidationResult>();

            foreach ( var validator in validators )
            {
                results.AddRange( validator.ValidateObject( instance ) );
            }

            return results;
        }

        public IReadOnlyList<IValidationResult> ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( propertyNames, nameof( propertyNames ) );

            var results = new List<IValidationResult>();

            foreach ( var validator in validators )
            {
                results.AddRange( validator.ValidateObject( instance, propertyNames ) );
            }

            return results;
        }

        public IReadOnlyList<IValidationResult> ValidateProperty( string propertyName, object value )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            var results = new List<IValidationResult>();

            foreach ( var validator in validators )
            {
                results.AddRange( validator.ValidateProperty( propertyName, value ) );
            }

            return results;
        }
    }
}