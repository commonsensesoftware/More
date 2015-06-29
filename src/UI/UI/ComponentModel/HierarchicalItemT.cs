namespace More.ComponentModel
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Represents a hierarchical item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value the hierarchical item represents.</typeparam>
    [DebuggerTypeProxy( typeof( More.Collections.Generic.CollectionDebugView<> ) )]
    [DebuggerDisplay( "Depth = {Depth}, IsSelected = {IsSelected}, IsExpanded = {IsExpanded}, Count = {Count}, Value = {Value}" )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is class is a collection and an item." )]
    public class HierarchicalItem<T> :
        ObservableCollection<HierarchicalItem<T>>,
        IClickableItem<T>,
        ISelectableItem<T>,
        IExpandableItem<T>,
        IEquatable<T>,
        IEquatable<HierarchicalItem<T>>,
        IEquatable<IExpandableItem<T>>,
        IEquatable<IClickableItem<T>>,
        IEquatable<ISelectableItem<T>>
    {
        private readonly IEqualityComparer<T> comparer;
        private int? hashCode;
        private bool? selected;
        private bool expanded;
        private int depth;
        private HierarchicalItem<T> parent;
        private ICommand click;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="selected">Indicates whether the item is selected.</param>
        protected internal HierarchicalItem( T value, bool? selected )
            : this( value, selected, new Command<object>( DefaultAction.None, p => true ), EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="selected">Indicates whether the item is selected.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        protected internal HierarchicalItem( T value, bool? selected, IEqualityComparer<T> comparer )
            : this( value, selected, new Command<object>( DefaultAction.None, p => true ), comparer )
        {
            Arg.NotNull( comparer, "comparer" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        public HierarchicalItem( T value, ICommand clickCommand )
            : this( value, null, clickCommand, EqualityComparer<T>.Default )
        {
            Arg.NotNull( clickCommand, "clickCommand" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="selected">Indicates whether the item is selected.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        public HierarchicalItem( T value, bool? selected, ICommand clickCommand )
            : this( value, selected, clickCommand, EqualityComparer<T>.Default )
        {
            Arg.NotNull( clickCommand, "clickCommand" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public HierarchicalItem( T value, ICommand clickCommand, IEqualityComparer<T> comparer )
            : this( value, null, clickCommand, comparer )
        {
            Arg.NotNull( clickCommand, "clickCommand" );
            Arg.NotNull( comparer, "comparer" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItem{T}"/> class.
        /// </summary>
        /// <param name="value">The item value.</param>
        /// <param name="selected">Indicates whether the item is selected.</param>
        /// <param name="clickCommand">The <see cref="ICommand">command</see> to execute when the item is clicked.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        public HierarchicalItem( T value, bool? selected, ICommand clickCommand, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( clickCommand, "clickCommand" );
            Arg.NotNull( comparer, "comparer" );

            this.comparer = comparer;
            this.selected = selected;
            this.click = new CommandInterceptor<object>( clickCommand, p => this.OnClicked( EventArgs.Empty ) );
            this.Select = new Command<object>( p => this.IsSelected = true );
            this.Unselect = new Command<object>( p => this.IsSelected = false );
            this.Expand = new Command<object>( p => this.IsExpanded = true );
            this.Collapse = new Command<object>( p => this.IsExpanded = false );
            this.Value = value;
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
                return this.comparer;
            }
        }

        /// <summary>
        /// Gets or sets the depth of the current item.
        /// </summary>
        /// <value>The item depth.  The default value is 0.</value>
        public int Depth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return this.depth;
            }
            protected internal set
            {
                Arg.GreaterThanOrEqualTo( value, 0, "value" );

                if ( this.depth == value )
                    return;

                this.depth = value;
                this.OnPropertyChanged( "Depth" );
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current item is a leaf.
        /// </summary>
        /// <value>True if the current item is a leaf (zero child items); otherwise, false.</value>
        public bool IsLeaf
        {
            get
            {
                return this.Count == 0;
            }
        }

        /// <summary>
        /// Gets the parent of the current item.
        /// </summary>
        /// <value>A <see cref="HierarchicalItem{T}"/> object.  The default value is null.</value>
        public HierarchicalItem<T> Parent
        {
            get
            {
                return this.parent;
            }
            private set
            {
                if ( this.parent == value )
                    return;

                this.parent = value;
                this.OnPropertyChanged( "Parent" );
                UpdateDepth( this );
            }
        }

        private static void UpdateDepth( HierarchicalItem<T> current )
        {
            Contract.Requires( current != null );

            var queue = new Queue<HierarchicalItem<T>>();
            queue.Enqueue( current );

            // recursively apply depth from current item forward
            while ( queue.Count > 0 )
            {
                current = queue.Dequeue();
                Contract.Assume( current != null );
                current.Depth = current.Parent == null ? 0 : current.Parent.Depth + 1;

                foreach ( var item in current )
                    queue.Enqueue( item );
            }
        }

        /// <summary>
        /// Raises the <see cref="ObservableCollection{T}.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="Clicked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnClicked( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Clicked;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Selected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnSelected( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Selected;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Unselected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnUnselected( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Unselected;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="Indeterminate"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnIndeterminate( EventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.Indeterminate;

            if ( handler != null )
                handler( this, e );
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
        /// Sets the hash code for the current instance.
        /// </summary>
        /// <param name="newHashCode">A hash code.</param>
        /// <remarks>This method can be used to assign a previously generated hash code.  For example, if an item
        /// is reconstituted from a persistence medium.</remarks>
        protected internal virtual void SetHashCode( int newHashCode )
        {
            this.hashCode = newHashCode;
        }

        /// <summary>
        /// Overrides the default behavior when a property changes.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by base class" )]
        protected override void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );

            Contract.Assume( e != null );
            switch ( e.PropertyName )
            {
                case "Count":
                    {
                        base.OnPropertyChanged( new PropertyChangedEventArgs( "IsLeaf" ) );
                        break;
                    }
            }
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted into the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="item">The <see cref="HierarchicalItem{T}"/> to insert.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void InsertItem( int index, HierarchicalItem<T> item )
        {
            base.InsertItem( index, item );
            Contract.Assume( item != null );
            item.Parent = this;
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected override void RemoveItem( int index )
        {
            Contract.Assume( index >= 0 && index < this.Count );
            var item = this[index];
            base.RemoveItem( index );

            Contract.Assume( item != null );
            item.Parent = null;
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced in the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the replacement takes place.</param>
        /// <param name="item">The new <see cref="HierarchicalItem{T}"/> item.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void SetItem( int index, HierarchicalItem<T> item )
        {
            Contract.Assume( index >= 0 && index < this.Count );
            base.SetItem( index, item );

            Contract.Assume( item != null );
            item.Parent = this;
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Object">object</see> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public override bool Equals( object obj )
        {
            return this.Equals( obj as HierarchicalItem<T> );
        }

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            // if the hash code has been manually set, use that value
            // instead of recomputing a new hash

            if ( this.hashCode != null )
                return this.hashCode.Value;

            var hash = this.Depth + 1;

            if ( this.Value != null )
                hash += this.Comparer.GetHashCode( this.Value );

            for ( var item = this.Parent; item != null; item = item.Parent )
                hash ^= item.GetHashCode();

            return hash;
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
        /// <param name="obj1">The <see cref="HierarchicalItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="HierarchicalItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> equals <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator ==( HierarchicalItem<T> obj1, HierarchicalItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return object.Equals( obj2, null );

            return obj1.Equals( obj2 );
        }

        /// <summary>
        /// Returns a value indicating whether the specified objects are not equal.
        /// </summary>
        /// <param name="obj1">The <see cref="HierarchicalItem{T}"/> that is the basis of the comparison.</param>
        /// <param name="obj2">The <see cref="HierarchicalItem{T}"/> to compare against.</param>
        /// <returns>True if <paramref name="obj1"/> does not equal <paramref name="obj2"/>; otherwise, false.</returns>
        public static bool operator !=( HierarchicalItem<T> obj1, HierarchicalItem<T> obj2 )
        {
            if ( object.Equals( obj1, null ) )
                return !object.Equals( obj2, null );

            return !obj1.Equals( obj2 );
        }

        /// <summary>
        /// Gets the item value.
        /// </summary>
        /// <value>The item value.</value>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a command that can be used to click the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Click
        {
            get
            {
                return this.click;
            }
            protected internal set
            {
                Arg.NotNull( value, "value" );

                if ( this.click == value )
                    return;

                // wrap the command with an interceptor to ensure the Click event is raised when the command is invoked
                this.click = new CommandInterceptor<object>( value, p => this.OnClicked( EventArgs.Empty ) );
                this.OnPropertyChanged( "Click" );
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        /// <value>A <see cref="Nullable{T}"/> object.  The default value is false.</value>
        public bool? IsSelected
        {
            get
            {
                return this.selected;
            }
            set
            {
                if ( Nullable.Equals( this.selected, value ) )
                    return;

                this.selected = value;

                if ( !this.selected.HasValue )
                    this.OnIndeterminate( EventArgs.Empty );
                else if ( this.selected.Value )
                    this.OnSelected( EventArgs.Empty );
                else
                    this.OnUnselected( EventArgs.Empty );

                this.OnPropertyChanged( "IsSelected" );
            }
        }

        /// <summary>
        /// Gets a command that can be used to select the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Select
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command that can be used to unselect the item.
        /// </summary>
        /// <value>An <see cref="ICommand"/> object.</value>
        public ICommand Unselect
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when the item is selected.
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// Occurs when the item is unselected.
        /// </summary>
        public event EventHandler Unselected;

        /// <summary>
        /// Occurs when the selected state of the item is indeterminate.
        /// </summary>
        public event EventHandler Indeterminate;

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
                if ( this.expanded == value )
                    return;

                this.expanded = value;
                this.OnPropertyChanged( "IsExpanded" );

                if ( this.expanded )
                    this.OnExpanded( EventArgs.Empty );
                else
                    this.OnCollapsed( EventArgs.Empty );
            }
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
        /// <param name="other">The <see cref="HierarchicalItem{T}"/> to evaluate.</param>
        /// <returns>True if the specified object equals the current instance; otherwise, false.</returns>
        public bool Equals( HierarchicalItem<T> other )
        {
            return !object.Equals( other, null ) && this.GetHashCode().Equals( other.GetHashCode() );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This method is hidden because it MAY not consider item Depth." )]
        bool IEquatable<IClickableItem<T>>.Equals( IClickableItem<T> other )
        {
            if ( other == null )
                return false;

            var otherItem = other as HierarchicalItem<T>;

            // we can compare values, but this may not be the same item if it's at a different depth
            return !object.Equals( otherItem, null ) ? this.GetHashCode().Equals( other.GetHashCode() ) : this.Comparer.Equals( this.Value, other.Value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This method is hidden because it MAY not consider item Depth." )]
        bool IEquatable<ISelectableItem<T>>.Equals( ISelectableItem<T> other )
        {
            if ( other == null )
                return false;

            var otherItem = other as HierarchicalItem<T>;

            // we can compare values, but this may not be the same item if it's at a different depth
            return !object.Equals( otherItem, null ) ? this.GetHashCode().Equals( other.GetHashCode() ) : this.Comparer.Equals( this.Value, other.Value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This method is hidden because it MAY not consider item Depth." )]
        bool IEquatable<IExpandableItem<T>>.Equals( IExpandableItem<T> other )
        {
            if ( other == null )
                return false;

            var otherItem = other as HierarchicalItem<T>;

            // we can compare values, but this may not be the same item if it's at a different depth
            return !object.Equals( otherItem, null ) ? this.GetHashCode().Equals( other.GetHashCode() ) : this.Comparer.Equals( this.Value, other.Value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This method is hidden because it WILL not consider item Depth." )]
        bool IEquatable<T>.Equals( T other )
        {
            // we can compare values, but this may not be the same item if it's at a different depth
            return this.Comparer.Equals( this.Value, other );
        }
    }
}
