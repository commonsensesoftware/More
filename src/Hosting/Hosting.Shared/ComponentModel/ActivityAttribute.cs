namespace More.ComponentModel
{
    using System;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides metadata that identifies a type is an exported activity.
    /// </summary>
    [CLSCompliant( false )]
    [MetadataAttribute]
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    [SuppressMessage( "Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute is not sealed so that the name and/or description can be localized." )]
    public class ActivityAttribute : ExportAttribute, IActivityDescriptor
    {
        private Lazy<string> id;
        private Lazy<string> name;
        private string desc = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAttribute"/> class.
        /// </summary>
        /// <remarks>This constructor exports the activity with the contract type of <see cref="IActivity"/>.</remarks>
        public ActivityAttribute()
            : base( typeof( IActivity ) )
        {
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAttribute"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the exported activity.</param>
        /// <remarks>This constructor exports the activity with the contract type of <see cref="IActivity"/>.</remarks>
        public ActivityAttribute( string contractName )
            : base( contractName, typeof( IActivity ) )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( contractName ), "contractName" );
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAttribute"/> class.
        /// </summary>
        /// <param name="contractType">The contract <see cref="Type"/> for the exported activity.</param>
        public ActivityAttribute( Type contractType )
            : base( contractType )
        {
            Contract.Requires<ArgumentNullException>( contractType != null, "contractType" );
            this.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAttribute"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the exported activity.</param>
        /// <param name="contractType">The contract <see cref="Type"/> for the exported activity.</param>
        public ActivityAttribute( string contractName, Type contractType )
            : base( contractName, contractType )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( contractName ), "contractName" );
            Contract.Requires<ArgumentNullException>( contractType != null, "contractType" );
            this.Init();
        }

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>The Globally Unique Identifier (GUID) representing the activity identifier.</value>
        public string Id
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return this.id.Value;
            }
            set
            {
                Contract.Requires<ArgumentException>( IsValidIdentifier( value ), "value" );
                this.id = new Lazy<string>( () => value );
            }
        }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        /// <value>The name of the activity.</value>
        public virtual string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return this.name.Value;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( value ), "value" );
                this.name = new Lazy<string>( () => value );
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        public virtual string Description
        {
            get
            {
                Contract.Ensures( this.desc != null );
                return this.desc;
            }
            set
            {
                Contract.Requires<ArgumentNullException>( value != null, "value" );
                this.desc = value;
            }
        }

        /// <summary>
        /// Gets or sets the exported type.
        /// </summary>
        /// <value>The exported <see cref="Type">type</see>.</value>
        public Type ExportedType
        {
            get;
            set;
        }

        private void Init()
        {
            this.id = new Lazy<string>( Guid.NewGuid().ToString );
            this.name = new Lazy<string>( this.GetDefaultName );
        }

        private string GetDefaultName()
        {
            Contract.Ensures( string.IsNullOrEmpty( Contract.Result<string>() ) );

            var type = this.GetType();

            if ( type.Name.EndsWith( "Attribute" ) )
                return type.Name.Substring( 0, type.Name.Length - 9 );

            return type.Name;
        }

        /// <summary>
        /// Returns a value indicating whether the specified activity identifier is valid.
        /// </summary>
        /// <param name="activityId">The Globally Unique Identifier (GUID) representing the activity identifier to validated.</param>
        /// <returns>True if the specified activity identifier is valid; otherwise, false.</returns>
        [Pure]
        public static bool IsValidIdentifier( string activityId )
        {
            if ( string.IsNullOrEmpty( activityId ) )
                return false;

            Guid id;

            if ( Guid.TryParse( activityId, out id ) )
                return !Guid.Empty.Equals( id );

            return false;
        }
    }
}
