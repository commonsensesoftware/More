namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents an item with a corresponding name and description.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item value.</typeparam>
    public class NamedItem<T> : ObservableObject, INamedComponent
    {
        private string name;
        private string description;
        private T itemValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
        /// </summary>
        public NamedItem()
        {
            this.name = this.GetType().GetTypeInfo().Name;
            this.description = string.Empty;
            this.itemValue = default( T );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The item description.</param>
        public NamedItem( string name, string description )
            : this( name, description, default( T ) )
        {
            Arg.NotNullOrEmpty( name, "name" );
            Arg.NotNull( description, "description" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The item description.</param>
        /// <param name="value">The item value of type <typeparamref name="T"/>.</param>
        public NamedItem( string name, string description, T value )
        {
            Arg.NotNullOrEmpty( name, "name" );
            Arg.NotNull( description, "description" );

            this.name = name;
            this.description = description;
            this.itemValue = value;
        }

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        /// <value>The item value of type <typeparamref name="T"/>.</value>
        public virtual T Value
        {
            get
            {
                return this.itemValue;
            }
            set
            {
                this.SetProperty( ref this.itemValue, value );
            }
        }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        /// <value>The item name.</value>
        public virtual string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.name ) );
                return this.name;
            }
            set
            {
                Arg.NotNullOrEmpty( value, "value" );
                this.SetProperty( ref this.name, value );
            }
        }

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        /// <value>The item description.</value>
        public virtual string Description
        {
            get
            {
                Contract.Ensures( this.description != null );
                return this.description;
            }
            set
            {
                Arg.NotNull( value, "value" );
                this.SetProperty( ref this.description, value );
            }
        }
    }
}
