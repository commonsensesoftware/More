namespace System.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Serves as the base class for all validation attributes.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationAttribute.</remarks>
    public abstract class ValidationAttribute : Attribute
    {
        private string errorMessage;
        private Func<string> errorMessageResourceAccessor;
        private string errorMessageResourceName;
        private Type errorMessageResourceType;
        private bool isCallingOverload;
        private object syncLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" /> class.
        /// </summary>
        protected ValidationAttribute()
            : this( (Func<string>) ( () => DataAnnotationsResources.ValidationAttribute_ValidationError ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" /> class by using the function that enables access to validation resources.
        /// </summary>
        /// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
        protected ValidationAttribute( Func<string> errorMessageAccessor )
        {
            syncLock = new object();
            errorMessageResourceAccessor = errorMessageAccessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationAttribute" /> class by using the error message to associate with a validation control.
        /// </summary>
        /// <param name="errorMessage">The error message to associate with a validation control.</param>
        protected ValidationAttribute( string errorMessage )
            : this( () => errorMessage )
        {
        }

        internal bool CustomErrorMessageSet
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        /// <value>The error message that is associated with the validation control.</value>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
                errorMessageResourceAccessor = null;
                CustomErrorMessageSet = true;
            }
        }

        /// <summary>
        /// Gets or sets the error message resource name to use in order to look up the <see cref="ValidationAttribute" />.ErrorMessageResourceType property value if validation fails.
        /// </summary>
        /// <value>The error message resource that is associated with a validation control.</value>
        public string ErrorMessageResourceName
        {
            get
            {
                return errorMessageResourceName;
            }
            set
            {
                errorMessageResourceName = value;
                errorMessageResourceAccessor = null;
                CustomErrorMessageSet = true;
            }
        }

        /// <summary>
        /// Gets or sets the resource type to use for error-message lookup if validation fails.
        /// </summary>
        /// <value>The type of error message that is associated with a validation control.</value>
        public Type ErrorMessageResourceType
        {
            get
            {
                return errorMessageResourceType;
            }
            set
            {
                errorMessageResourceType = value;
                errorMessageResourceAccessor = null;
                CustomErrorMessageSet = true;
            }
        }

        /// <summary>
        /// Gets the localized validation error message.
        /// </summary>
        /// <value>The localized validation error message.</value>
        protected string ErrorMessageString
        {

            get
            {
                SetupResourceAccessor();
                return errorMessageResourceAccessor();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the attribute requires validation context.
        /// </summary>
        /// <value>True if the attribute requires validation context; otherwise, false.</value>
        public virtual bool RequiresValidationContext
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public virtual string FormatErrorMessage( string name )
        {
            return string.Format( CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name } );
        }

        /// <summary>
        /// Checks whether the specified value is valid with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        public ValidationResult GetValidationResult( object value, ValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            var result = IsValid( value, validationContext );

            if ( ( result != null ) && !( ( result != null ) ? !string.IsNullOrEmpty( result.ErrorMessage ) : false ) )
            {
                result = new ValidationResult( FormatErrorMessage( validationContext.DisplayName ), result.MemberNames );
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <returns>True if the specified value is valid; otherwise, false.</returns>
        public virtual bool IsValid( object value )
        {
            var valid = false;

            lock ( syncLock )
            {
                if ( isCallingOverload )
                {
                    throw new NotImplementedException( DataAnnotationsResources.ValidationAttribute_IsValid_NotImplemented );
                }

                isCallingOverload = true;

                try
                {
                    valid = IsValid( value, null ) == ValidationResult.Success;
                }
                finally
                {
                    isCallingOverload = false;
                }
            }

            return valid;
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        protected virtual ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            var result = ValidationResult.Success;

            lock ( syncLock )
            {
                if ( isCallingOverload )
                {
                    throw new NotImplementedException( DataAnnotationsResources.ValidationAttribute_IsValid_NotImplemented );
                }

                isCallingOverload = true;

                try
                {
                    if ( !IsValid( value ) )
                    {
                        if ( validationContext == null )
                        {
                            result = new ValidationResult( DataAnnotationsResources.ValidationAttribute_InvalidValue );
                        }
                        else
                        {
                            var memberNames = validationContext.MemberName != null ? new [] { validationContext.MemberName } : null;
                            result = new ValidationResult( FormatErrorMessage( validationContext.DisplayName ), memberNames );
                        }
                    }
                }
                finally
                {
                    isCallingOverload = false;
                }
            }

            return result;
        }

        private void SetResourceAccessorByPropertyLookup()
        {
            if ( ( errorMessageResourceType == null ) || string.IsNullOrEmpty( errorMessageResourceName ) )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName, new object[0] ) );
            }

            var property = errorMessageResourceType.GetRuntimeProperty( errorMessageResourceName );

            if ( property != null )
            {
                var getMethod = property.GetMethod;

                if ( getMethod == null || ( !getMethod.IsAssembly && !getMethod.IsPublic ) )
                {
                    property = null;
                }
            }

            if ( property == null )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_ResourceTypeDoesNotHaveProperty, new object[] { errorMessageResourceType.FullName, errorMessageResourceName } ) );
            }

            if ( property.PropertyType != typeof( string ) )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_ResourcePropertyNotStringType, new object[] { property.Name, errorMessageResourceType.FullName } ) );
            }

            errorMessageResourceAccessor = () => (string) property.GetValue( null, null );
        }

        private void SetupResourceAccessor()
        {
            if ( errorMessageResourceAccessor != null )
                return;

            Func<string> func = null;
            string localErrorMessage = errorMessage;
            bool flag = !string.IsNullOrEmpty( errorMessageResourceName );
            bool flag2 = !string.IsNullOrEmpty( localErrorMessage );
            bool flag3 = errorMessageResourceType != null;

            if ( flag == flag2 )
            {
                throw new InvalidOperationException( DataAnnotationsResources.ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource );
            }

            if ( flag3 != flag )
            {
                throw new InvalidOperationException( DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName );
            }

            if ( flag )
            {
                SetResourceAccessorByPropertyLookup();
            }
            else
            {
                if ( func == null )
                {
                    func = () => localErrorMessage;
                }
                errorMessageResourceAccessor = func;
            }
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext" /> object that describes the context where the validation checks are performed. This parameter cannot be null.</param>
        public void Validate( object value, ValidationContext validationContext )
        {
            Arg.NotNull( validationContext, nameof( validationContext ) );

            var validationResult = GetValidationResult( value, validationContext );

            if ( validationResult != ValidationResult.Success )
                throw new ValidationException( validationResult, this, value );
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="name">The name to include in the error message.</param>
        public void Validate( object value, string name )
        {
            if ( !IsValid( value ) )
                throw new ValidationException( FormatErrorMessage( name ), this, value );
        }
    }
}
