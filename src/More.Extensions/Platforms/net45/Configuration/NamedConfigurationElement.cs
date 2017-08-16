namespace More.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.Security.Permissions;

    /// <summary>
    /// Represents a named <see cref="ConfigurationElement">configuration element</see> where the name of the element is the key in a collection.
    /// </summary>
    /// <remarks>Facilitates the creation of custom <see cref="ConfigurationElement">configuration elements</see> keyed by name.</remarks>
    [ConfigurationPermission( SecurityAction.InheritanceDemand, Unrestricted = true )]
    public abstract class NamedConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the name of configuration property that contains the name of the <see cref="NamedConfigurationElement"/>.
        /// </summary>
        /// <value>The name of the configuration property.</value>
        protected const string NameProperty = "name";

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedConfigurationElement"/> class.
        /// </summary>
        protected NamedConfigurationElement() => Name = "Name";

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedConfigurationElement"/> class with the supplied name.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        protected NamedConfigurationElement( string name )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        [StringValidator( MinLength = 1 )]
        [ConfigurationProperty( NameProperty, IsKey = true, DefaultValue = "Name", IsRequired = true )]
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return (string) base[NameProperty];
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                base[NameProperty] = value;
            }
        }
    }
}