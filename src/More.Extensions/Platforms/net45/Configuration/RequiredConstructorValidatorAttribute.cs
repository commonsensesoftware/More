namespace More.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A <see cref="ConfigurationValidatorAttribute">configuration attribute</see> that is used to create
    /// <see cref="RequiredConstructorValidator">required constructor validators</see>.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property )]
    [SuppressMessage( "Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "This allows the attribute definition to be CLS-compliant, even though its usage is not." )]
    public sealed class RequiredConstructorValidatorAttribute : ConfigurationValidatorAttribute
    {
        Type[] parameters = Type.EmptyTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredConstructorValidatorAttribute"/> class.
        /// </summary>
        public RequiredConstructorValidatorAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredConstructorValidatorAttribute"/> class.
        /// </summary>
        /// <param name="parameters">The array of parameter <see cref="Type">types</see> for the target constructor, in ordinal order.</param>
        [CLSCompliant( false )]
        public RequiredConstructorValidatorAttribute( params Type[] parameters )
        {
            if ( parameters != null )
            {
                this.parameters = parameters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Type.IsAssignableFrom">subclassed</see> types are allowed
        /// or if the constructor must exactly match. The default is to not allow contravariant constructors.
        /// </summary>
        /// <value>True if contravariance is allowed; otherwise false.</value>
        public bool AllowContravariance { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter <see cref="Type">types</see> the required constructor must use.
        /// </summary>
        /// <value>The list of parameter <see cref="Type">types</see> the required constructor must use.</value>
        [SuppressMessage( "Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required to support a CLS-compliant attribute. The backing field is immutable." )]
        public Type[] Parameters
        {
            get
            {
                Contract.Ensures( parameters != null );
                return (Type[]) parameters.Clone();
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                parameters = (Type[]) value.Clone();
            }
        }

        /// <summary>
        /// Gets a new initialized instance of the <see cref="RequiredConstructorValidator"/> class.
        /// </summary>
        /// <remarks>Every call to this property will create a new instance of the <see cref="RequiredConstructorValidator"/> class.</remarks>
        /// <value>A new initializes instance of the <see cref="RequiredConstructorValidator"/> class.</value>
        public override ConfigurationValidatorBase ValidatorInstance => new RequiredConstructorValidator( AllowContravariance, parameters );
    }
}