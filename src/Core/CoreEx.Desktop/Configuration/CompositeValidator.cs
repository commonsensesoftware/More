namespace More.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a <see cref="ConfigurationValidatorBase">configuration validator</see> that supports multiple
    /// validators for a single configuration property.
    /// </summary>
    public class CompositeValidator : ConfigurationValidatorBase
    {
        private readonly ConfigurationValidatorBase[] validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">An <see cref="IEnumerable{T}">sequence</see> of <see cref="ConfigurationValidatorBase">validators</see> that should be composed.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public CompositeValidator( IEnumerable<ConfigurationValidatorBase> validators )
        {
            Arg.NotNull( validators, nameof( validators ) );
            this.validators = validators.ToArray();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object">value</see> is valid.
        /// </summary>
        /// <remarks>This implementation will invoke the <seealso cref="Validate"/> on each composed validator instance.</remarks>
        /// <param name="value">The <see cref="Object">object</see> to validate.</param>
        public override void Validate( object value )
        {
            var typeToValidate = value as Type;

            if ( typeToValidate == null )
                throw new ArgumentException( SR.InvalidArgType.FormatDefault( typeof( Type ) ), "value" );

            foreach ( var validator in validators )
                validator.Validate( value );
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type">type</see> can be validated.
        /// </summary>
        /// <remarks>This implementation will invoke the <seealso cref="M:CanValidate"/> method on each validator instance
        /// and stops on the first failure.</remarks>
        /// <param name="type">The <see cref="Object">value</see> type.</param>
        /// <returns>True if the <paramref name="type"/> parameter matches the expected <see cref="Type">type</see>; otherwise, false.</returns>
        public override bool CanValidate( Type type )
        {
            var valid = validators.All( validator => validator.CanValidate( type ) );
            return valid;
        }
    }
}