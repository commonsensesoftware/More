namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;    
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a node collection.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of node value.</typeparam>
    [DebuggerDisplay( "Depth = {Depth}, Count = {Count}, Value = {Value}" )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is a special case where the node is an item and a collection." )]
    public class Node<T> : ObservableCollection<Node<T>>
    {
        private readonly IComparer<T> valueComparer;
        private readonly IEqualityComparer<Node<T>> itemComparer;
        private T currentValue;
        private int depth;
        private Node<T> parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class with the supplied <see cref="IEqualityComparer{T}">comparer</see>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}">comparer</see> used to evaluate item values.</param>
        public Node( IComparer<T> comparer )
            : this( default( T ), comparer )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        public Node()
            : this( default( T ), Comparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">The node value.</param>
        public Node( T value )
            : this( value, Comparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">The node value.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> used to evaluate item values.</param>
        public Node( T value, IComparer<T> comparer )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            valueComparer = comparer;
            itemComparer = new ComparerAdapter<Node<T>, T>( comparer, i => i.Value );
            currentValue = value;
        }

        /// <summary>
        /// Gets the value comparer for the current node.
        /// </summary>
        /// <value>The <see cref="IComparer{T}"/> used to compare node values.</value>
        protected IComparer<T> ValueComparer
        {
            get
            {
                Contract.Ensures( valueComparer != null );
                return valueComparer;
            }
        }

        /// <summary>
        /// Gets the item comparer for the current node.
        /// </summary>
        /// <value>The <see cref="IEqualityComparer{T}"/> used to compare nodes.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generic support" )]
        protected virtual IEqualityComparer<Node<T>> ItemComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<Node<T>>>() != null );
                return itemComparer;
            }
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <value>The node value.</value>
        public T Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                if ( ValueComparer.Compare( currentValue, value ) == 0 )
                    return;

                currentValue = value;
                OnPropertyChanged( "Value" );
            }
        }

        /// <summary>
        /// Gets the depth of the current node.
        /// </summary>
        /// <value>The node depth.  The default value is 0.</value>
        public int Depth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return depth;
            }
            private set
            {
                Contract.Requires( value >= 0 );

                if ( depth == value )
                    return;

                depth = value;
                OnPropertyChanged( "Depth" );
            }
        }

        /// <summary>
        /// Gets the parent of the current node.
        /// </summary>
        /// <value>A <see cref="Node{T}"/> object.  The default value is null.</value>
        public Node<T> Parent
        {
            get
            {
                return parent;
            }
            private set
            {
                if ( ItemComparer.Equals( parent, value ) )
                    return;

                parent = value;
                OnPropertyChanged( "Parent" );
                UpdateDepth( this );
            }
        }

        private static void UpdateDepth( Node<T> current )
        {
            Contract.Requires( current != null );
            var queue = new Queue<Node<T>>();

            queue.Enqueue( current );

            while ( queue.Count > 0 )
            {
                current = queue.Dequeue();
                Contract.Assume( current != null );
                current.Depth = current.Parent == null ? 0 : current.Parent.Depth + 1;
                current.ForEach( queue.Enqueue );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged( string propertyName )
        {
            OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Inserts a value to the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="value">The value of type <typeparamref name="T"/> to insert.</param>
        public virtual void Insert( int index, T value )
        {
            Insert( index, new Node<T>( value, ValueComparer ) );
        }

        /// <summary>
        /// Adds a value to the collection.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to add.</param>
        public virtual void Add( T value )
        {
            Add( new Node<T>( value, ValueComparer ) );
        }

        /// <summary>
        /// Adds a sequence of values to the collection.
        /// </summary>
        /// <param name="values">The <see cref="IEnumerable{T}">sequence</see> of values to add.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public void AddRange( IEnumerable<T> values )
        {
            Arg.NotNull( values, nameof( values ) );
            this.AddRange( values.ToList().Select( value => new Node<T>( value, ValueComparer ) ) );
        }

        /// <summary>
        /// Removes a value from the collection.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to remove.</param>
        /// <returns>True if the value is removed; otherwise, false.</returns>
        public virtual bool Remove( T value )
        {
            var index = this.FindIndex( item => ValueComparer.Compare( item.Value, value ) == 0 );

            if ( index < 0 )
                return false;

            RemoveAt( index );
            return true;
        }

        /// <summary>
        /// Overrides the default behavior when an item is removed from the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the removal takes place.</param>
        protected override void RemoveItem( int index )
        {
            Contract.Assume( index >= 0 && index < Count );
            var item = this[index];
            base.RemoveItem( index );

            Contract.Assume( item != null );
            item.Parent = null;
        }

        /// <summary>
        /// Overrides the default behavior when an item is inserted into the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the insertion takes place.</param>
        /// <param name="item">The <see cref="Node{T}"/> to insert.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void InsertItem( int index, Node<T> item )
        {
            base.InsertItem( index, item );
            Contract.Assume( item != null );
            item.Parent = this;
        }

        /// <summary>
        /// Overrides the default behavior when an item is replaced in the collection.
        /// </summary>
        /// <param name="index">The zero-based index where the replacement takes place.</param>
        /// <param name="item">The new <see cref="Node{T}"/> item.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Facilitated by sub and super types." )]
        protected override void SetItem( int index, Node<T> item )
        {
            base.SetItem( index, item );
            Contract.Assume( item != null );
            item.Parent = this;
        }
    }
}
