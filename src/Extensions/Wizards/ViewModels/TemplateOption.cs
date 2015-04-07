namespace More.VisualStudio.ViewModels
{
    using More.ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an option for a template.
    /// </summary>
    [DebuggerDisplay( "Name = {Name}, IsEnabled = {IsEnabled}, IsOptional = {IsOptional}" )]
    public class TemplateOption : ObservableObject
    {
        private readonly string id;
        private readonly string name;
        private readonly string description;
        private bool enabled = true;
        private bool optional = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateOption"/> class.
        /// </summary>
        /// <param name="id">The template option identifier.</param>
        /// <param name="name">The name of the option.</param>
        public TemplateOption( string id, string name )
            : this( id, name, string.Empty )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( id ), "id" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateOption"/> class.
        /// </summary>
        /// <param name="id">The template option identifier.</param>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">The option description.</param>
        public TemplateOption( string id, string name, string description )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( id ), "id" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( name ), "name" );
            Contract.Requires<ArgumentNullException>( description != null, "description" );

            this.id = id;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the template option identifier.
        /// </summary>
        /// <value>The option identifier.</value>
        public string Id
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.id ) );
                return this.id;
            }
        }

        /// <summary>
        /// Gets the template option name.
        /// </summary>
        /// <value>The option name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.name ) );
                return this.name;
            }
        }

        /// <summary>
        /// Gets the template option description.
        /// </summary>
        /// <value>The option description.</value>
        public string Description
        {
            get
            {
                Contract.Ensures( this.description != null );
                return this.description;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is enabled.
        /// </summary>
        /// <value>True if the option is enabled; otherwise, false. The default value is <c>true</c>.</value>
        /// <remarks>Some options are just a list of possibilities. The value of this property is ignored in such scenarios.</remarks>
        public bool IsEnabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.SetProperty( ref this.enabled, value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is required.
        /// </summary>
        /// <value>True if the option is not required; otherwise, false. The default value is <c>true</c>.</value>
        /// <remarks>Some options are just a list of possibilities. The value of this property is ignored in such scenarios.</remarks>
        public bool IsOptional
        {
            get
            {
                return this.optional;
            }
            set
            {
                this.SetProperty( ref this.optional, value );
            }
        }
    }
}
