namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Defines a helper class that can be used to validate objects, properties, and methods when it is included
    /// in their associated <see cref="T:ValidationAttribute" /> attributes
    /// </summary>
    /// <remarks>This class provides backward compatibility for System.ComponentModel.DataAnnotations.Validator.</remarks>
    public static class Validator
    {
        /// <summary>
        /// Represents a validation error.
        /// </summary>
        private partial class ValidationError
        {
            internal ValidationError( ValidationAttribute validationAttribute, object value, ValidationResult validationResult )
            {
                ValidationAttribute = validationAttribute;
                ValidationResult = validationResult;
                Value = value;
            }

            internal object Value
            {
                get;
                set;
            }

            internal ValidationAttribute ValidationAttribute
            {
                get;
                set;
            }

            internal ValidationResult ValidationResult
            {
                get;
                set;
            }

            internal void ThrowValidationException()
            {
                throw new ValidationException( ValidationResult.ErrorMessage, ValidationAttribute, Value );
            }
        }

        private static ValidationAttributeStore store = ValidationAttributeStore.Instance;

        private static ICollection<KeyValuePair<ValidationContext, object>> GetPropertyValues( object instance, ValidationContext validationContext )
        {
            var properties = instance.GetType().GetRuntimeProperties();
            var list = new List<KeyValuePair<ValidationContext, object>>();

            foreach ( var propertyDescriptor in properties )
            {
                var context = CreateValidationContext( instance, validationContext );

                context.MemberName = propertyDescriptor.Name;

                if ( store.GetPropertyValidationAttributes( context ).Any() )
                    list.Add( new KeyValuePair<ValidationContext, object>( context, propertyDescriptor.GetValue( instance ) ) );
            }

            return list;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <returns>True if the property validates; otherwise, false.</returns>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> cannot be assigned to the property.-or-<paramref name="value " />is null.</exception>
        public static bool TryValidateProperty( object value, ValidationContext validationContext, ICollection<ValidationResult> validationResults )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var propertyType = store.GetPropertyType( validationContext );
            var memberName = validationContext.MemberName;

            EnsureValidPropertyType( memberName, propertyType, value );

            var result = true;
            var breakOnFirstError = validationResults == null;
            var propertyValidationAttributes = store.GetPropertyValidationAttributes( validationContext );

            foreach ( var current in GetValidationErrors( value, validationContext, propertyValidationAttributes, breakOnFirstError ) )
            {
                result = false;

                if ( validationResults != null )
                    validationResults.Add( current.ValidationResult );
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context and validation results collection.
        /// </summary>
        /// <returns>True if the object validates; otherwise, false.</returns>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static bool TryValidateObject( object instance, ValidationContext validationContext, ICollection<ValidationResult> validationResults )
        {
            return TryValidateObject( instance, validationContext, validationResults, false );
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context, validation results collection,
        /// and a value that specifies whether to validate all properties.</summary>
        /// <returns>True if the object validates; otherwise, false.</returns>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold each failed validation.</param>
        /// <param name="validateAllProperties">True to validate all properties; otherwise, false.</param>
        /// <exception cref="T:System.ArgumentNullException">The supplied <paramref name="instance" /> is null.</exception>
        public static bool TryValidateObject( object instance, ValidationContext validationContext, ICollection<ValidationResult> validationResults, bool validateAllProperties )
        {
            if ( instance == null )
                throw new ArgumentNullException( "instance" );

            if ( validationContext != null && instance != validationContext.ObjectInstance )
                throw new ArgumentException( DataAnnotationsResources.InstanceMustMatchValidationContextInstance, "instance" );

            var result = true;
            var breakOnFirstError = validationResults == null;

            foreach ( var current in GetObjectValidationErrors( instance, validationContext, validateAllProperties, breakOnFirstError ) )
            {
                result = false;

                if ( validationResults != null )
                    validationResults.Add( current.ValidationResult );
            }

            return result;
        }

        /// <summary>
        /// Returns a value that indicates whether the specified value is valid with the specified attributes.
        /// </summary>
        /// <returns>True if the object validates; otherwise, false.</returns>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationResults">A collection to hold failed validations. </param>
        /// <param name="validationAttributes">The validation attributes.</param>
        public static bool TryValidateValue( object value, ValidationContext validationContext, ICollection<ValidationResult> validationResults, IEnumerable<ValidationAttribute> validationAttributes )
        {
            var result = true;
            var breakOnFirstError = validationResults == null;

            foreach ( var current in GetValidationErrors( value, validationContext, validationAttributes, breakOnFirstError ) )
            {
                result = false;

                if ( validationResults != null )
                    validationResults.Add( current.ValidationResult );
            }

            return result;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the property to validate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> cannot be assigned to the property.</exception>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The <paramref name="value" /> parameter is not valid.</exception>
        public static void ValidateProperty( object value, ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var propertyType = store.GetPropertyType( validationContext );

            EnsureValidPropertyType( validationContext.MemberName, propertyType, value );

            var propertyValidationAttributes = store.GetPropertyValidationAttributes( validationContext );
            var validationError = GetValidationErrors( value, validationContext, propertyValidationAttributes, false ).FirstOrDefault<ValidationError>();

            if ( validationError != null )
                validationError.ThrowValidationException();
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The object is not valid.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static void ValidateObject( object instance, ValidationContext validationContext )
        {
            ValidateObject( instance, validationContext, false );
        }

        /// <summary>
        /// Determines whether the specified object is valid using the validation context, and a value that specifies whether to validate all properties.
        /// </summary>
        /// <param name="instance">The object to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validateAllProperties">True to validate all properties; otherwise, false.</param>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException"><paramref name="instance" /> is not valid.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="instance" /> is null.</exception>
        public static void ValidateObject( object instance, ValidationContext validationContext, bool validateAllProperties )
        {
            if ( instance == null )
                throw new ArgumentNullException( "instance" );

            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            if ( instance != validationContext.ObjectInstance )
                throw new ArgumentException( DataAnnotationsResources.InstanceMustMatchValidationContextInstance, "instance" );

            var validationError = GetObjectValidationErrors( instance, validationContext, validateAllProperties, false ).FirstOrDefault<ValidationError>();

            if ( validationError != null )
                validationError.ThrowValidationException();
        }

        /// <summary>
        /// Validates the specified attributes.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context that describes the object to validate.</param>
        /// <param name="validationAttributes">The validation attributes.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="validationContext" /> parameter is null.</exception>
        /// <exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The <paramref name="value" /> parameter does not validate with the <paramref name="validationAttributes" /> parameter.</exception>
        public static void ValidateValue( object value, ValidationContext validationContext, IEnumerable<ValidationAttribute> validationAttributes )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var validationError = GetValidationErrors( value, validationContext, validationAttributes, false ).FirstOrDefault<ValidationError>();

            if ( validationError != null )
                validationError.ThrowValidationException();
        }

        internal static ValidationContext CreateValidationContext( object instance, ValidationContext validationContext )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            return new ValidationContext( instance, validationContext, validationContext.Items );
        }

        private static bool CanBeAssigned( Type destinationType, object value )
        {
            if ( destinationType == null )
                throw new ArgumentNullException( "destinationType" );

            var destType = destinationType.GetTypeInfo();

            if ( value == null )
                return !destType.IsValueType || ( destType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof( Nullable<> ) );

            return destType.IsAssignableFrom( value.GetType().GetTypeInfo() );
        }

        private static void EnsureValidPropertyType( string propertyName, Type propertyType, object value )
        {
            if ( !CanBeAssigned( propertyType, value ) )
            {
                var message = DataAnnotationsResources.PropertyValueWrongType.FormatDefault( propertyName, propertyType );
                throw new ArgumentException( message, "value" );
            }
        }

        private static IEnumerable<Validator.ValidationError> GetObjectValidationErrors( object instance, ValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError )
        {
            if ( instance == null )
                throw new ArgumentNullException( "instance" );

            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var list = new List<Validator.ValidationError>();
            list.AddRange( GetObjectPropertyValidationErrors( instance, validationContext, validateAllProperties, breakOnFirstError ) );

            if ( list.Any() )
                return list;

            var typeValidationAttributes = store.GetTypeValidationAttributes( validationContext );
            list.AddRange( GetValidationErrors( instance, validationContext, typeValidationAttributes, breakOnFirstError ) );

            if ( list.Any() )
                return list;

            var validatableObject = instance as IValidatableObject;

            if ( validatableObject != null )
            {
                var source = validatableObject.Validate( validationContext );

                foreach ( var current in source.Where( vr => vr != ValidationResult.Success ) )
                    list.Add( new ValidationError( null, instance, current ) );
            }

            return list;
        }

        private static IEnumerable<ValidationError> GetObjectPropertyValidationErrors( object instance, ValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError )
        {
            var propertyValues = GetPropertyValues( instance, validationContext );
            var list = new List<ValidationError>();

            foreach ( var current in propertyValues )
            {
                var propertyValidationAttributes = store.GetPropertyValidationAttributes( current.Key );

                if ( validateAllProperties )
                {
                    list.AddRange( GetValidationErrors( current.Value, current.Key, propertyValidationAttributes, breakOnFirstError ) );
                }
                else
                {
                    var requiredAttribute = propertyValidationAttributes.OfType<RequiredAttribute>().FirstOrDefault();

                    if ( requiredAttribute != null )
                    {
                        var validationResult = requiredAttribute.GetValidationResult( current.Value, current.Key );

                        if ( validationResult != ValidationResult.Success )
                            list.Add( new Validator.ValidationError( requiredAttribute, current.Value, validationResult ) );
                    }
                }

                if ( breakOnFirstError && list.Any() )
                    break;
            }

            return list;
        }

        private static IEnumerable<Validator.ValidationError> GetValidationErrors( object value, ValidationContext validationContext, IEnumerable<ValidationAttribute> attributes, bool breakOnFirstError )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var list = new List<Validator.ValidationError>();
            var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            ValidationError item;

            if ( requiredAttribute != null && !TryValidate( value, validationContext, requiredAttribute, out item ) )
            {
                list.Add( item );
                return list;
            }

            foreach ( var current in attributes )
            {
                if ( current != requiredAttribute && !TryValidate( value, validationContext, current, out item ) )
                {
                    list.Add( item );

                    if ( breakOnFirstError )
                        break;
                }
            }

            return list;
        }

        private static bool TryValidate( object value, ValidationContext validationContext, ValidationAttribute attribute, out ValidationError validationError )
        {
            if ( validationContext == null )
                throw new ArgumentNullException( "validationContext" );

            var validationResult = attribute.GetValidationResult( value, validationContext );

            if ( validationResult != ValidationResult.Success )
            {
                validationError = new ValidationError( attribute, value, validationResult );
                return false;
            }

            validationError = null;
            return true;
        }
    }
}
