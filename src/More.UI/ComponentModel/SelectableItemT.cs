namespace More.ComponentModel
{
    using More.Collections.Generic;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Represents a selectable item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of selectable value.</typeparam>
    [DebuggerDisplay( "IsSelected = {IsSelected}, Value = {Value}" )]
    public class SelectableItem<T> : ObservableObject, ISelectableItem<T>, IEquatable<ISelectableItem<T>>, IEquatable<T>
    {
        static readonly IEqualityComparer<bool?> NullBoolComparer = new DynamicComparer<bool?>( Nullable.Compare<bool> );
        readonly IEqualityComparer<T> comparer;
        bool? selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        public SelectableItem( T value ) : this( false, value, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public SelectableItem( T value, IEqualityComparer<T> comparer ) : this( false, value, comparer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem{T}"/> class.
        /// </summary>
        /// <param name="selected">A <see cref="Nullable{T}"/> indicating whether the item is selected.</param>
        /// <param name="value">The item value.</param>
        public SelectableItem( bool? selected, T value ) : this( selected, value, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem{T}"/> class.
        /// </summary>
        /// <param name="selected">A <see cref="Nullable{T}"/> indicating whether the item is selected.</param>
        /// <param name="value">The item value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public SelectableItem( bool? selected, T value, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( comparer, nameof( comparer ) );

            Value = value;
            this.selected = selected;
            this.comparer = comparer;
            Select = new Command<object>( p => IsSelected = true );
            Unselect = new Command<object>( p => IsSelected = false );
        }

        /// <summary>
        /// Gets the item comparer.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        protected virtual IEqualityComparer<T> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null );
                return comparer;
            }
        }

        /// <summary>
        /// Raises the <see cref="Selected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnSelected( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Selected?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Unselected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnUnselected( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Unselected?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Indeterminate"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnIndeterminate( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Indeterminate?.Invoke( this, e );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public override bool Equals( object obj ) => Equals( obj as ISelectableItem<T> );

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode() => Value == null ? 0 : Comparer.GetHashCode( Value );

        /// <summary>
        /// Returns the string representation of the current instance.
        /// </summary>
        /// <returns>The string representation of the current instance.</returns>
        public override string ToString() => Value == null ? string.Empty : Value.ToString();

        /// <summary>
        /// Returns a value indicating whether the specified objects are equal.
        /// </summary>
        /// <param name="obj1">The <see cref="SelectableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="ISelectableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( SelectableItem<T> obj1, ISelectableItem<T> obj2 ) =>
            ReferenceEquals( obj1, null ) ? ReferenceEquals( obj2, null ) : obj1.Equals( obj2 );

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="SelectableItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="ISelectableItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( SelectableItem<T> obj1, ISelectableItem<T> obj2 ) =>
            ReferenceEquals( obj1, null ) ? !ReferenceEquals( obj2, null ) : !obj1.Equals( obj2 );

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <value>A <see cref="Nullable{T}"/> object.  The default value is false.</value>
        public virtual bool? IsSelected
        {
            get => selected;
            set
            {
                if ( !SetProperty( ref selected, value, NullBoolComparer ) )
                {
                    return;
                }

                if ( selected == null )
                {
                    OnIndeterminate( EventArgs.Empty );
                }
                else if ( selected.Value )
                {
                    OnSelected( EventArgs.Empty );
                }
                else
                {
                    OnUnselected( EventArgs.Empty );
                }
            }
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        public T Value { get; }

        /// <summary>
        /// Gets a command that can be used to select the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Select { get; }

        /// <summary>
        /// Gets a command that can be used to unselect the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Unselect { get; }

        /// <summary>
        /// Occurs when the item is selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Occurs when the item is unselected.
        /// </summary>
        public event EventHandler Unselected;

        /// <summary>
        /// Occurs when the selected value of the item is indeterminate.
        /// </summary>
        public event EventHandler Indeterminate;

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="other">The <see cref="ISelectableItem{T}"/> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( ISelectableItem<T> other ) => !ReferenceEquals( other, null ) && Comparer.Equals( Value, other.Value );

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified value.
        /// </summary>
        /// <param name="other">The <typeparamref name="T">value</typeparamref> to evaluate.</param>
        /// <returns>True if the specified value equals the current instance; otherwise, false.</returns>
        public bool Equals( T other ) => Comparer.Equals( Value, other );
    }
}