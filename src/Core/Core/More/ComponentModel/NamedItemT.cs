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
            name = GetType().GetTypeInfo().Name;
            description = string.Empty;
            itemValue = default( T );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The item description.</param>
        public NamedItem( string name, string description )
            : this( name, description, default( T ) )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedItem{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The item description.</param>
        /// <param name="value">The item value of type <typeparamref name="T"/>.</param>
        public NamedItem( string name, string description, T value )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNull( description, nameof( description ) );

            this.name = name;
            this.description = description;
            itemValue = value;
        }

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        /// <value>The item value of type <typeparamref name="T"/>.</value>
        public virtual T Value
        {
            get
            {
                return itemValue;
            }
            set
            {
                SetProperty( ref itemValue, value );
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
                Contract.Ensures( !string.IsNullOrEmpty( name ) );
                return name;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                SetProperty( ref name, value );
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
                Contract.Ensures( description != null );
                return description;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref description, value );
            }
        }
    }
}
