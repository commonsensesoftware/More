namespace More.ComponentModel
{
    using More.Windows.Input;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.Contracts;
    using global::System.Windows.Input;

    /// <summary>
    /// Represents a clickable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of clickable value.</typeparam>
    [DebuggerDisplay( "Value = {Value}" )]
    public class ClickableItem<T> : ObservableObject, IClickableItem<T>, IEquatable<IClickableItem<T>>, IEquatable<T>
    {
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        public ClickableItem( T value, ICommand clickCommand )
            : this( value, clickCommand, EqualityComparer<T>.Default )
        {
            Contract.Requires<ArgumentNullException>( clickCommand != null, "clickCommand" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public ClickableItem( T value, ICommand clickCommand, IEqualityComparer<T> comparer )
        {
            Contract.Requires<ArgumentNullException>( clickCommand != null, "clickCommand" );
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );

            this.Click = new CommandInterceptor<object>( p => this.OnClicked( EventArgs.Empty ), clickCommand );
            this.Value = value;
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets the value comparer.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        protected virtual IEqualityComparer<T> Comparer
        {
            get
            {
                Contract.Ensures( this.comparer != null );
                return this.comparer;
            }
        }

        /// <summary>
        /// Raises the <see cref="Clicked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnClicked( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.Clicked;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public override bool Equals( object obj )
        {
            return this.Equals( obj as IClickableItem<T> );
        }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return this.Value == null ? 0 : this.Comparer.GetHashCode( this.Value );
        }

        /// <summary>
        /// Returns the string representation of the current instance.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override string ToString()
        {
            return this.Value == null ? string.Empty : this.Value.ToString();
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are equal.
        /// </summary>
        /// <param name="obj1">The <see cref="ClickableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="IClickableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( ClickableItem<T> obj1, IClickableItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return object.Equals( obj2, null );

            return obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="IClickableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="IClickableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( ClickableItem<T> obj1, IClickableItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return !object.Equals( obj2, null );

            return !obj1.Equals( obj2 );
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command that can be used to click the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Click
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="other">The <see cref="IClickableItem{T}"/> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( IClickableItem<T> other )
        {
            return !object.Equals( other, null ) && this.Comparer.Equals( this.Value, other.Value );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified value.
        /// </summary>
        /// <param name="other">The other <typeparamref name="T">value</typeparamref> to evaluate.</param>
        /// <returns>True if the specified value equals the current instance; otherwise, false.</returns>
        public bool Equals( T other )
        {
            return this.Comparer.Equals( this.Value, other );
        }
    }
}
