namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the behavior of an objet that can be used to validate objects, properties, and methods.
    /// </summary>
    public class Validator : IValidator
    {
        private readonly Dictionary<Type, IObjectValidator> validators = new Dictionary<Type, IObjectValidator>();

        /// <summary>
        /// Attempts to retrieve a validator for the specified type.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> of object to get a validator for.</param>
        /// <param name="validator">The retrieved <see cref="IObjectValidator">validator</see> or <c>null</c>.</param>
        /// <returns>True if the <paramref name="validator"/> was successfully retrieved; otherwise, false.</returns>
        protected virtual bool TryGetValidator( Type type, out IObjectValidator validator )
        {
            Arg.NotNull( type, nameof( type ) );
            Contract.Ensures( ( Contract.Result<bool>() && Contract.ValueAtReturn( out validator ) != null ) || Contract.ValueAtReturn( out validator ) == null );

            // try retrieving an exact match
            if ( validators.TryGetValue( type, out validator ) )
                return true;

            // if there isn't an exact match, aggregate covariant validators
            // note: this allows the support of interfaces and inheritance scenarios
            var typeInfo = type.GetTypeInfo();
            var covariantValidators = ( from entry in validators
                                        let otherTypeInfo = entry.Key.GetTypeInfo()
                                        where otherTypeInfo.IsAssignableFrom( typeInfo )
                                        select entry.Value ).ToArray();

            // no compatible validator
            if ( covariantValidators.Length == 0 )
                return false;

            // create composite validator; don't cache the validator since the other validators
            // could change between calls and tracking the changes is complex
            validator = new CovariantObjectValidator( covariantValidators );
            return true;
        }

        /// <summary>
        /// Gets the validator for the specified type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of object to create a validator for.</typeparam>
        /// <returns>An <see cref="IObjectValidator{T}">object validator</see> for the request <typeparamref name="T">type</typeparamref>.</returns>
        [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "For", Justification = "Generic method that is unambiguous" )]
        public virtual IObjectValidator<T> For<T>()
        {
            Contract.Ensures( Contract.Result<IObjectValidator<T>>() != null );

            var type = typeof( T );
            IObjectValidator validator;

            if ( validators.TryGetValue( type, out validator ) )
                return (IObjectValidator<T>) validator;

            var newValidator = new ObjectValidator<T>();
            validators[type] = newValidator;
            return newValidator;
        }

        /// <summary>
        /// Creates and returns a new validation context.
        /// </summary>
        /// <param name="instance">The object to create the context for.</param>
        /// <param name="items">The dictionary of key/value pairs that is associated with this context.</param>
        /// <returns>A new <see cref="IValidationContext">validation context</see>.</returns>
        public virtual IValidationContext CreateContext( object instance, IDictionary<object, object> items )
        {
            Arg.NotNull( instance, nameof( instance ) );
            return new ValidationContext( instance, items );
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context
        /// and validation results collection.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <param name="validateAllProperties">True to validate all properties; if false, only required attributes are validated.</param>
        /// <returns>True if the object validates; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public virtual bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults, bool validateAllProperties )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( instance == null )
                return true;

            var type = validationContext.ObjectType;
            IObjectValidator validator;

            if ( !TryGetValidator( type, out validator ) )
                return true;

            var results = validateAllProperties ?
                          validator.ValidateObject( instance ) :
                          validator.ValidateObject( instance, new[] { validationContext.MemberName } );

            if ( validationResults != null )
                validationResults.AddRange( results );

            return results.Count == 0;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context,
        /// validation results collection, and a value that specifies whether to validate
        /// all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the object validates; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public bool TryValidateObject( object instance, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );
            var validateAllProperties = string.IsNullOrEmpty( validationContext.MemberName );
            return TryValidateObject( instance, validationContext, validationResults, validateAllProperties );
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <returns>True if the property validates; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public virtual bool TryValidateProperty( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            var type = validationContext.ObjectType;
            IObjectValidator validator;

            if ( !TryGetValidator( type, out validator ) )
                return true;

            var results = validator.ValidateProperty( validationContext.MemberName, value );

            if ( validationResults != null )
                validationResults.AddRange( results );

            return results.Count == 0;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context,
        /// and a value that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validateAllProperties">True to validate all properties; otherwise, false.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public virtual void ValidateObject( object instance, IValidationContext validationContext, bool validateAllProperties )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            if ( instance == null )
                return;

            var type = validationContext.ObjectType;
            IObjectValidator validator;

            if ( !TryGetValidator( type, out validator ) )
                return;

            var results = validateAllProperties ?
                          validator.ValidateObject( instance ) :
                          validator.ValidateObject( instance, new[] { validationContext.MemberName } );

            if ( results.Count > 0 )
                throw new ValidationException( results.First(), null );
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public void ValidateObject( object instance, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );
            var validateAllProperties = string.IsNullOrEmpty( validationContext.MemberName );
            ValidateObject( instance, validationContext, validateAllProperties );
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public virtual void ValidateProperty( object value, IValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            var type = validationContext.ObjectType;
            IObjectValidator validator;

            if ( !TryGetValidator( type, out validator ) )
                return;

            var results = validator.ValidateProperty( validationContext.MemberName, value );

            if ( results.Count > 0 )
                throw new ValidationException( results.First(), value );
        }
    }
}
