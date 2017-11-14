namespace More.VisualStudio.ViewModels
{
    using More.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an option for a template.
    /// </summary>
    [DebuggerDisplay( "Name = {Name}, IsEnabled = {IsEnabled}, IsOptional = {IsOptional}" )]
    public class TemplateOption : ObservableObject
    {
        readonly string id;
        readonly string name;
        readonly string description;
        bool enabled = true;
        bool optional = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateOption"/> class.
        /// </summary>
        /// <param name="id">The template option identifier.</param>
        /// <param name="name">The name of the option.</param>
        public TemplateOption( string id, string name ) : this( id, name, string.Empty ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateOption"/> class.
        /// </summary>
        /// <param name="id">The template option identifier.</param>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">The option description.</param>
        public TemplateOption( string id, string name, string description )
        {
            Arg.NotNullOrEmpty( id, nameof( id ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNull( description, nameof( description ) );

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
                Contract.Ensures( !string.IsNullOrEmpty( id ) );
                return id;
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
                Contract.Ensures( !string.IsNullOrEmpty( name ) );
                return name;
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
                Contract.Ensures( description != null );
                return description;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is enabled.
        /// </summary>
        /// <value>True if the option is enabled; otherwise, false. The default value is <c>true</c>.</value>
        /// <remarks>Some options are just a list of possibilities. The value of this property is ignored in such scenarios.</remarks>
        public bool IsEnabled
        {
            get => enabled;
            set => SetProperty( ref enabled, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is required.
        /// </summary>
        /// <value>True if the option is not required; otherwise, false. The default value is <c>true</c>.</value>
        /// <remarks>Some options are just a list of possibilities. The value of this property is ignored in such scenarios.</remarks>
        public bool IsOptional
        {
            get => optional;
            set => SetProperty( ref optional, value );
        }
    }
}