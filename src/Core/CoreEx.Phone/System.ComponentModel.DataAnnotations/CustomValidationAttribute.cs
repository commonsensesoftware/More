namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Globalization;
    using global::System.Linq;
    using global::System.Reflection;

    /// <summary>
    /// Specifies a custom validation method that is used to validate a property or class instance.
    /// </summary>
    /// <remarks>This class provides backward compatibility for System.ComponentModel.DataAnnotations.CustomValidationAttribute.</remarks>
    [AttributeUsage( AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true )]
    public sealed class CustomValidationAttribute : ValidationAttribute
    {
        private bool isSingleArgumentMethod;
        private string lastMessage;
        private Lazy<string> malformedErrorMessage;
        private string method;
        private MethodInfo methodInfo;
        private Type validatorType;
        private Type valuesType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidationAttribute"/> class.
        /// </summary>
        /// <param name="validatorType">The type that contains the method that performs custom validation.</param>
        /// <param name="method">The method that performs custom validation.</param>
        public CustomValidationAttribute( Type validatorType, string method )
            : base( (Func<string>) ( () => DataAnnotationsResources.CustomValidationAttribute_ValidationError ) )
        {
            this.validatorType = validatorType;
            this.method = method;
            this.malformedErrorMessage = new Lazy<string>( new Func<string>( this.CheckAttributeWellFormed ) );
        }

        /// <summary>
        /// Gets the validation method.
        /// </summary>
        /// <value>The name of the validation method.</value>
        public string Method
        {
            get
            {
                return this.method;
            }
        }

        /// <summary>
        /// Gets the type that performs custom validation.
        /// </summary>
        /// <value>The type that performs custom validation.</value>
        public Type ValidatorType
        {
            get
            {
                return this.validatorType;
            }
        }

        private string CheckAttributeWellFormed()
        {
            return ( this.ValidateValidatorTypeParameter() ?? this.ValidateMethodParameter() );
        }

        private void ThrowIfAttributeNotWellFormed()
        {
            string message = this.malformedErrorMessage.Value;
            if ( message != null )
            {
                throw new InvalidOperationException( message );
            }
        }

        private bool TryConvertValue( object value, out object convertedValue )
        {
            convertedValue = null;
            var conversionType = this.valuesType;
            var conversionTypeInfo = conversionType.GetTypeInfo();

            if ( value == null )
            {
                return ( !conversionTypeInfo.IsValueType || ( conversionTypeInfo.IsGenericType && !( conversionType.GetGenericTypeDefinition() != typeof( Nullable<> ) ) ) );
            }

            if ( conversionTypeInfo.IsAssignableFrom( value.GetType().GetTypeInfo() ) )
            {
                convertedValue = value;
                return true;
            }

            try
            {
                convertedValue = Convert.ChangeType( value, conversionType, CultureInfo.CurrentCulture );
                return true;
            }
            catch ( FormatException )
            {
                return false;
            }
            catch ( InvalidCastException )
            {
                return false;
            }
            catch ( NotSupportedException )
            {
                return false;
            }
        }

        private string ValidateMethodParameter()
        {
            if ( string.IsNullOrEmpty( this.method ) )
            {
                return DataAnnotationsResources.CustomValidationAttribute_Method_Required;
            }

            var validationMethod = ( from runtimeMethod in this.validatorType.GetRuntimeMethods()
                                     where runtimeMethod.Name == this.method &&
                                           runtimeMethod.ReturnType == typeof( ValidationResult )
                                     let args = runtimeMethod.GetParameters()
                                     where args.Length > 0 && args.Length <= 2
                                     select runtimeMethod ).FirstOrDefault();

            if ( validationMethod == null )
            {
                return string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Not_Found, new object[] { this.method, this.validatorType.Name } );
            }

            if ( validationMethod.ReturnType != typeof( ValidationResult ) )
            {
                return string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Must_Return_ValidationResult, new object[] { this.method, this.validatorType.Name } );
            }

            ParameterInfo[] parameters = validationMethod.GetParameters();

            if ( ( parameters.Length == 0 ) || parameters[0].ParameterType.IsByRef )
            {
                return string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Signature, new object[] { this.method, this.validatorType.Name } );
            }

            this.isSingleArgumentMethod = parameters.Length == 1;

            if ( !this.isSingleArgumentMethod && ( ( parameters.Length != 2 ) || ( parameters[1].ParameterType != typeof( ValidationContext ) ) ) )
            {
                return string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Signature, new object[] { this.method, this.validatorType.Name } );
            }

            this.methodInfo = validationMethod;
            this.valuesType = parameters[0].ParameterType;
            return null;
        }

        private string ValidateValidatorTypeParameter()
        {
            if ( this.validatorType == null )
            {
                return DataAnnotationsResources.CustomValidationAttribute_ValidatorType_Required;
            }

            if ( !this.validatorType.GetTypeInfo().IsVisible )
            {
                return string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Type_Must_Be_Public, new object[] { this.validatorType.Name } );
            }

            return null;
        }

        /// <summary>
        /// Formats a validation error message.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage( string name )
        {
            this.ThrowIfAttributeNotWellFormed();

            if ( !string.IsNullOrEmpty( this.lastMessage ) )
            {
                return string.Format( CultureInfo.CurrentCulture, this.lastMessage, new object[] { name } );
            }

            return base.FormatErrorMessage( name );
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            object obj2;
            ValidationResult result2;
            this.ThrowIfAttributeNotWellFormed();
            MethodInfo info = this.methodInfo;

            if ( !this.TryConvertValue( value, out obj2 ) )
            {
                return new ValidationResult( string.Format( CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Type_Conversion_Failed, new object[] { ( value != null ) ? value.GetType().ToString() : "null", this.valuesType, this.validatorType, this.method } ) );
            }

            try
            {
                object[] parameters = this.isSingleArgumentMethod ? new object[] { obj2 } : new object[] { obj2, validationContext };
                ValidationResult result = (ValidationResult) info.Invoke( null, parameters );
                this.lastMessage = null;
                if ( result != null )
                {
                    this.lastMessage = result.ErrorMessage;
                }
                result2 = result;
            }
            catch ( TargetInvocationException exception )
            {
                if ( exception.InnerException != null )
                {
                    throw exception.InnerException;
                }
                throw;
            }

            return result2;
        }
    }
}
