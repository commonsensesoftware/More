namespace More.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a <see cref="ConfigurationValidatorAttribute"/> that can be used to ensure a type is
    /// non-abstract, not an interface, and not a generic type definition.
    /// </summary>
    [Serializable]
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class NonAbstractTypeValidatorAttribute : ConfigurationValidatorAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="P:Type.IsAbstract">abstract</see> types are allowed.
        /// </summary>
        /// <value>True if abstract types are allowed; otherwise false.</value>
        public bool AllowAbstract
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if <see cref="P:Type.IsInterface">interface</see> types are allowed.
        /// </summary>
        /// <value>True if interface types are allowed; otherwise false.</value>
        public bool AllowInterface
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether if <see cref="P:Type.IsGenericTypeDefinition">generic definition</see> types are allowed.
        /// </summary>
        /// <value>True if generic definition types are allowed; otherwise false.</value>
        public bool AllowGenericTypeDefinition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a new, initialized instance of the <see cref="NonAbstractTypeValidator"/> class.
        /// </summary>
        /// <value>A <see cref="NonAbstractTypeValidator"/> object.</value>
        /// <remarks>The underlying <see cref="NonAbstractTypeValidator"/> instance is transient. Each call to this property will
        /// create a new <see cref="NonAbstractTypeValidator"/> instance.</remarks>
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get
            {
                var validator = new NonAbstractTypeValidator( AllowAbstract, AllowInterface, AllowGenericTypeDefinition );
                return validator;
            }
        }
    }
}