namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal sealed class CovariantObjectValidator : IObjectValidator
    {
        private readonly IReadOnlyList<IObjectValidator> validators;

        internal CovariantObjectValidator( IReadOnlyList<IObjectValidator> validators )
        {
            Contract.Requires( validators != null );
            this.validators = validators;
        }

        public IReadOnlyList<IValidationResult> ValidateObject( object instance )
        {
            Arg.NotNull( instance, "instance" );

            var results = new List<IValidationResult>();

            foreach ( var validator in this.validators )
                results.AddRange( validator.ValidateObject( instance ) );

            return results;
        }

        public IReadOnlyList<IValidationResult> ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            Arg.NotNull( instance, "instance" );
            Arg.NotNull( propertyNames, "propertyNames" );

            var results = new List<IValidationResult>();

            foreach ( var validator in this.validators )
                results.AddRange( validator.ValidateObject( instance, propertyNames ) );

            return results;
        }

        public IReadOnlyList<IValidationResult> ValidateProperty( string propertyName, object value )
        {
            Arg.NotNullOrEmpty( propertyName, "propertyName" );

            var results = new List<IValidationResult>();

            foreach ( var validator in this.validators )
                results.AddRange( validator.ValidateProperty( propertyName, value ) );

            return results;
        }
    }
}
