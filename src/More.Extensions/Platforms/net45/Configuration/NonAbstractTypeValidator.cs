namespace More.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a <see cref="ConfigurationValidatorBase">configuration validator</see> that ensures a type is
    /// non-abstract, not an interface, and not a generic type definition.
    /// </summary>
    /// <remarks>This validator only supports validation of <see cref="Type">type</see> objects.</remarks>
    public class NonAbstractTypeValidator : ConfigurationValidatorBase
    {
        bool allowAbstract;
        bool allowInterface;
        bool allowGenericTypeDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAbstractTypeValidator"/> class with the default settings.
        /// </summary>
        /// <remarks>By default, abstract, interface, and generic type definitions are not allowed.</remarks>
        public NonAbstractTypeValidator() : this( false, false, false ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAbstractTypeValidator"/> class with the supplied settings.
        /// </summary>
        /// <param name="allowAbstract">Indicates if abstract types are allowed.</param>
        /// <param name="allowInterface">Indicates if interface types are allowed.</param>
        /// <param name="allowGenericTypeDefinition">Indicates if generic type definitions are allowed.</param>
        public NonAbstractTypeValidator( bool allowAbstract, bool allowInterface, bool allowGenericTypeDefinition )
        {
            this.allowAbstract = allowAbstract;
            this.allowInterface = allowInterface;
            this.allowGenericTypeDefinition = allowGenericTypeDefinition;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Type.IsAbstract">abstract</see> types are allowed.
        /// </summary>
        /// <value>True if abstract types are allowed; otherwise false.</value>
        public virtual bool AllowAbstract
        {
            get => allowAbstract;
            set => allowAbstract = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Type.IsInterface">interface</see> types are allowed.
        /// </summary>
        /// <value>True if interface types are allowed; otherwise false.</value>
        public virtual bool AllowInterface
        {
            get => allowInterface;
            set => allowInterface = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Type.IsGenericTypeDefinition">generic definition</see> types are allowed.
        /// </summary>
        /// <value>True if generic definition types are allowed; otherwise false.</value>
        public virtual bool AllowGenericTypeDefinition
        {
            get => allowGenericTypeDefinition;
            set => allowGenericTypeDefinition = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object">value</see> is valid.
        /// </summary>
        /// <param name="value">The <see cref="object">object</see> to validate.</param>
        public override void Validate( object value )
        {
            var typeToValidate = value as Type;

            if ( ( value as Type )== null )
            {
                throw new ArgumentException( SR.InvalidArgType.FormatDefault( typeof( Type ) ), nameof( value ) );
            }

            if ( !AllowAbstract && typeToValidate.IsAbstract )
            {
                throw new ConfigurationErrorsException( SR.AbstractNotSupported.FormatDefault( typeToValidate ) );
            }

            if ( !AllowInterface && typeToValidate.IsInterface )
            {
                throw new ConfigurationErrorsException( SR.InterfaceNotSupported.FormatDefault( typeToValidate ) );
            }

            if ( !AllowGenericTypeDefinition && typeToValidate.IsGenericTypeDefinition )
            {
                throw new ConfigurationErrorsException( SR.GenericTypeDefNotSupported.FormatDefault( typeToValidate ) );
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Type">type</see> can be validated.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to evaluate.</param>
        /// <returns>True if the <paramref name="type"/> parameter value matches the expected <see cref="Type">type</see>; otherwise, false.</returns>
        public override bool CanValidate( Type type ) => type == typeof( Type );
    }
}