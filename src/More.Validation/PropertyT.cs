namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a property with a given name and value.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of property value.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property", Justification = "Representation of a property with a typed value." )]
    public class Property<T>
    {
        readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the property,.</param>
        /// <param name="value">The property value.</param>
        public Property( string name, T value )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );

            this.name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The property name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( name ) );
                return name;
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <value>The property value of type <typeparamref name="T"/>.</value>
        public T Value { get; }
    }
}