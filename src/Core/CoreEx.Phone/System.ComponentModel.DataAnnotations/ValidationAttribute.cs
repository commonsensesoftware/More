namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Globalization;
    using global::System.Reflection;

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
            this.syncLock = new object();
            this.errorMessageResourceAccessor = errorMessageAccessor;
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
                return this.errorMessage;
            }
            set
            {
                this.errorMessage = value;
                this.errorMessageResourceAccessor = null;
                this.CustomErrorMessageSet = true;
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
                return this.errorMessageResourceName;
            }
            set
            {
                this.errorMessageResourceName = value;
                this.errorMessageResourceAccessor = null;
                this.CustomErrorMessageSet = true;
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
                return this.errorMessageResourceType;
            }
            set
            {
                this.errorMessageResourceType = value;
                this.errorMessageResourceAccessor = null;
                this.CustomErrorMessageSet = true;
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
                this.SetupResourceAccessor();
                return this.errorMessageResourceAccessor();
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
            return string.Format( CultureInfo.CurrentCulture, this.ErrorMessageString, new object[] { name } );
        }

        /// <summary>
        /// Checks whether the specified value is valid with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        public ValidationResult GetValidationResult( object value, ValidationContext validationContext )
        {
            if ( validationContext == null )
            {
                throw new ArgumentNullException( "validationContext" );
            }

            var result = this.IsValid( value, validationContext );

            if ( ( result != null ) && !( ( result != null ) ? !string.IsNullOrEmpty( result.ErrorMessage ) : false ) )
            {
                result = new ValidationResult( this.FormatErrorMessage( validationContext.DisplayName ), result.MemberNames );
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

            lock ( this.syncLock )
            {
                if ( this.isCallingOverload )
                {
                    throw new NotImplementedException( DataAnnotationsResources.ValidationAttribute_IsValid_NotImplemented );
                }

                this.isCallingOverload = true;

                try
                {
                    valid = this.IsValid( value, null ) == null;
                }
                finally
                {
                    this.isCallingOverload = false;
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
            ValidationResult result;

            lock ( this.syncLock )
            {
                if ( this.isCallingOverload )
                {
                    throw new NotImplementedException( DataAnnotationsResources.ValidationAttribute_IsValid_NotImplemented );
                }

                this.isCallingOverload = true;

                try
                {
                    var success = ValidationResult.Success;

                    if ( !this.IsValid( value ) )
                    {
                        string[] memberNames = ( validationContext.MemberName != null ) ? new string[] { validationContext.MemberName } : null;
                        success = new ValidationResult( this.FormatErrorMessage( validationContext.DisplayName ), memberNames );
                    }

                    result = success;
                }
                finally
                {
                    this.isCallingOverload = false;
                }
            }

            return result;
        }

        private void SetResourceAccessorByPropertyLookup()
        {
            if ( ( this.errorMessageResourceType == null ) || string.IsNullOrEmpty( this.errorMessageResourceName ) )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName, new object[0] ) );
            }

            var property = this.errorMessageResourceType.GetRuntimeProperty( this.errorMessageResourceName );

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
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_ResourceTypeDoesNotHaveProperty, new object[] { this.errorMessageResourceType.FullName, this.errorMessageResourceName } ) );
            }

            if ( property.PropertyType != typeof( string ) )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_ResourcePropertyNotStringType, new object[] { property.Name, this.errorMessageResourceType.FullName } ) );
            }

            this.errorMessageResourceAccessor = () => (string) property.GetValue( null, null );
        }

        private void SetupResourceAccessor()
        {
            if ( this.errorMessageResourceAccessor != null )
                return;

            Func<string> func = null;
            string localErrorMessage = this.errorMessage;
            bool flag = !string.IsNullOrEmpty( this.errorMessageResourceName );
            bool flag2 = !string.IsNullOrEmpty( localErrorMessage );
            bool flag3 = this.errorMessageResourceType != null;

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
                this.SetResourceAccessorByPropertyLookup();
            }
            else
            {
                if ( func == null )
                {
                    func = () => localErrorMessage;
                }
                this.errorMessageResourceAccessor = func;
            }
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">The <see cref="ValidationContext" /> object that describes the context where the validation checks are performed. This parameter cannot be null.</param>
        public void Validate( object value, ValidationContext validationContext )
        {
            if ( validationContext == null )
            {
                throw new ArgumentNullException( "validationContext" );
            }

            var validationResult = this.GetValidationResult( value, validationContext );

            if ( validationResult != null )
            {
                throw new ValidationException( validationResult, this, value );
            }
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="name">The name to include in the error message.</param>
        public void Validate( object value, string name )
        {
            if ( !this.IsValid( value ) )
            {
                throw new ValidationException( this.FormatErrorMessage( name ), this, value );
            }
        }
    }
}
