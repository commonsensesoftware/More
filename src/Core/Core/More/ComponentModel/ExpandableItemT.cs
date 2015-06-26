namespace More.ComponentModel
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Represents an expandable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of expandable value.</typeparam>
    [DebuggerDisplay( "IsExpanded = {IsExpanded}, Value = {Value}" )]
    public class ExpandableItem<T> : ObservableObject, IExpandableItem<T>, IEquatable<IExpandableItem<T>>, IEquatable<T>
    {
        private readonly IEqualityComparer<T> comparer;
        private bool expanded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        public ExpandableItem( T value )
            : this( false, value, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public ExpandableItem( T value, IEqualityComparer<T> comparer )
            : this( false, value, comparer )
        {
            Arg.NotNull( comparer, "comparer" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableItem{T}"/> class.
        /// </summary>
        /// <param name="expanded">Indicates whether the item is expanded.</param>
        /// <param name="value">The item value.</param>
        public ExpandableItem( bool expanded, T value )
            : this( expanded, value, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableItem{T}"/> class.
        /// </summary>
        /// <param name="expanded">Indicates whether the item is expanded.</param>
        /// <param name="value">The item value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public ExpandableItem( bool expanded, T value, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( comparer, "comparer" );

            this.Value = value;
            this.expanded = expanded;
            this.comparer = comparer;
            this.Expand = new Command<object>( p => this.IsExpanded = true );
            this.Collapse = new Command<object>( p => this.IsExpanded = false );
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
        /// Raises the <see cref="Expanded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnExpanded( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Expanded;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Collapsed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCollapsed( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Collapsed;

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
            return this.Equals( obj as IExpandableItem<T> );
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
        /// <param name="obj1">The <see cref="ExpandableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="IExpandableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( ExpandableItem<T> obj1, IExpandableItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return object.Equals( obj2, null );

            return obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="ExpandableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="IExpandableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( ExpandableItem<T> obj1, IExpandableItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return !object.Equals( obj2, null );

            return !obj1.Equals( obj2 );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded.
        /// </summary>
        /// <value>True if the item is expanded; otherwise, false.  The default value is false.</value>
        public virtual bool IsExpanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                if ( !this.SetProperty( ref this.expanded, value ) )
                    return;

                if ( this.expanded )
                    this.OnExpanded( EventArgs.Empty );
                else
                    this.OnCollapsed( EventArgs.Empty );
            }
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
        /// Gets a command that can be used to expand the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Expand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command that can be used to collapse the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Collapse
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when the item has expanded.
        /// </summary>
        public event EventHandler Expanded;

        /// <summary>
        /// Occurs when the item has collapsed.
        /// </summary>
        public event EventHandler Collapsed;

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="other">The <see cref="IExpandableItem{T}"/> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( IExpandableItem<T> other )
        {
            return !object.Equals( other, null ) && this.Comparer.Equals( this.Value, other.Value );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified value.
        /// </summary>
        /// <param name="other">The <typeparamref name="T">value</typeparamref> to evaluate.</param>
        /// <returns>True if the specified value equals the current instance; otherwise, false.</returns>
        public bool Equals( T other )
        {
            return this.Comparer.Equals( this.Value, other );
        }
    }
}
