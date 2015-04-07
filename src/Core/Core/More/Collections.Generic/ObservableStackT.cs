namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Collections.Specialized;
    using global::System.ComponentModel;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Linq;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// Represents a variable size last-in-first-out (LIFO) collection of instances of the same arbitrary type
    /// with support for change notification.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the stack.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( StackDebugView<> ) )]
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an appropriate name." )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A stack is a well-known collection type." )]
    public class ObservableStack<T> : IStack<T>, ICollection<T>, INotifyPropertyChanged, INotifyCollectionChanged where T : class
    {
        /// <summary>
        /// Enumerates the elements of a <see cref="ObservableStack{T}" /> as the internal implementation cannot simply use the actual array size.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        private struct Enumerator : IEnumerator<T>
        {
            private readonly ObservableStack<T> stack;
            private readonly int version;
            private int index;
            private T currentElement;

            internal Enumerator( ObservableStack<T> stack )
            {
                Contract.Requires( stack != null, "stack" );
                this.stack = stack;
                this.version = this.stack.version;
                this.index = -2;
                this.currentElement = default( T );
            }

            public void Dispose()
            {
                this.index = -1;
            }

            public bool MoveNext()
            {
                if ( this.version != this.stack.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                var flag = false;

                if ( this.index == -2 )
                {
                    this.index = this.stack.size - 1;
                    flag = this.index >= 0;

                    if ( flag )
                    {
                        Contract.Assume( this.stack.items != null );
                        this.currentElement = this.stack.items[this.index];
                    }

                    return flag;
                }

                if ( this.index == -1 )
                    return false;

                flag = --this.index >= 0;

                if ( flag )
                {
                    Contract.Assume( this.stack.items != null );
                    this.currentElement = this.stack.items[this.index];
                    return true;
                }

                this.currentElement = default( T );
                return false;
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            void IEnumerator.Reset()
            {
                if ( this.version != this.stack.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                this.index = -2;
                this.currentElement = default( T );
            }

            public T Current
            {
                get
                {
                    if ( this.index < 0 )
                        throw new InvalidOperationException( ExceptionMessage.CollectionBeforeFirst );
                    else if ( this.index >= this.stack.Count )
                        throw new InvalidOperationException( ExceptionMessage.CollectionAfterLast );

                    return this.currentElement;
                }
            }
        }

        private static readonly T[] EmptyArray = new T[0];
        private readonly object syncRoot = new object();
        private readonly IEqualityComparer<T> comparer;
        private T[] items;
        private int size;
        private int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class
        /// that is empty and has the default initial capacity.</summary>
        public ObservableStack()
            : this( EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class with the specified comparer
        /// that is empty and has the default initial capacity.</summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        public ObservableStack( IEqualityComparer<T> comparer )
        {
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );
            this.items = EmptyArray;
            this.comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that contains elements copied
        /// from the specified sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to copy elements from.</param>
        public ObservableStack( IEnumerable<T> sequence )
            : this( sequence, EqualityComparer<T>.Default )
        {
            Contract.Requires<ArgumentNullException>( sequence != null, "sequence" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that contains elements copied
        /// from the specified sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to copy elements from.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public ObservableStack( IEnumerable<T> sequence, IEqualityComparer<T> comparer )
        {
            Contract.Requires<ArgumentNullException>( sequence != null, "sequence" );
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );

            var list = sequence.ToList();
            var count = list.Count;

            this.size = count;
            this.comparer = comparer;
            this.items = new T[count];

            list.CopyTo( this.items, 0 );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that is empty and has the specified
        /// initial capacity or the default initial capacity, whichever is greater.</summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableStack{T}">stack</see> can contain.</param>
        public ObservableStack( int capacity )
            : this( capacity, EqualityComparer<T>.Default )
        {
            Contract.Requires<ArgumentOutOfRangeException>( capacity >= 0, "capacity" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that is empty and has the specified
        /// initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableStack{T}">stack</see> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        public ObservableStack( int capacity, IEqualityComparer<T> comparer )
        {
            Contract.Requires<ArgumentOutOfRangeException>( capacity >= 0, "capacity" );
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );

            this.items = new T[capacity];
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets the comparer used by the collection to compare items.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}"/> object.</value>
        protected IEqualityComparer<T> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null );
                return this.comparer;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Removes and returns the object at the top of the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <param name="result">The object removed from the top of the <see cref="ObservableStack{T}" />.</param>
        /// <returns>The index the item was removed from.</returns>
        /// <remarks>This method performs the the pop logic without producing any events
        /// or other side effects. Any overrides of <see cref="Pop"/> will not be called.
        /// </remarks>
        [SuppressMessageAttribute( "Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method returns the value and the count." )]
        protected int PopItem( out T result )
        {
            Contract.Requires<InvalidOperationException>( this.Count > 0 );
            Contract.Ensures( Contract.Result<int>() >= 0 );

            this.version++;
            var index = --this.size;
            Contract.Assume( index >= 0 && index < this.items.Length );
            var local = this.items[index];
            this.items[this.size] = default( T );

            result = local;
            return index;
        }

        /// <summary>
        /// Inserts an object at the top of the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <param name="item">The object to push onto the <see cref="ObservableStack{T}">stack</see>.
        /// The value can be <see langkeyword="null">null</see>.</param>
        /// <returns>The index the item was added at.</returns>
        /// <remarks>This method is implemented to perform the push logic without producing any events or other side effects.
        /// Any overrides of <see cref="Push"/> will not be called.
        /// </remarks>
        protected int PushItem( T item )
        {
            if ( this.size == this.items.Length )
            {
                var capacity = Math.Max( this.items.Length << 1, 4 );
                var destinationArray = new T[capacity];
                Array.Copy( this.items, 0, destinationArray, 0, this.size );
                this.items = destinationArray;
            }

            var index = this.size++;
            Contract.Assume( index >= 0 && index < this.items.Length );
            this.items[index] = item;
            this.version++;

            return index;
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have
        /// changed by using either <c>null</c>or <see cref="F:String.Empty"/> as the property
        /// name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <remarks>The event handler receives an argument of type <see cref="NotifyCollectionChangedEventArgs"/>,
        /// which contains data that is related to this event.</remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Returns the object at the top of the <see cref="ObservableStack{T}">stack</see> without removing it.
        /// </summary>
        /// <returns>The object at the top of the <see cref="ObservableStack{T}" />.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="Pop"/> method, but <seealso cref="Peek"/> does not modify the <see cref="ObservableStack{T}">stack</see>.</para>
        /// <para>A <c>null</c>can be pushed onto the <see cref="ObservableStack{T}">stack</see> as a placeholder, if needed.</para>
        /// </remarks>
        public virtual T Peek()
        {
            return this.items[this.size - 1];
        }

        /// <summary>
        /// Removes and returns the object at the top of the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <returns>The object removed from the top of the <see cref="ObservableStack{T}">stack</see>.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="Peek"/> method, but <seealso cref="Peek"/> does not modify the <see cref="ObservableStack{T}">stack</see>.</para>
        /// <para>A <c>null</c>could be popped from the <see cref="ObservableStack{T}">stack</see>.</para>
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual T Pop()
        {
            T local;
            var index = this.PopItem( out local );

            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, local, index ) );

            return local;
        }

        /// <summary>
        /// Inserts an object at the top of the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <param name="item">The object to push onto the <see cref="ObservableStack{T}">stack</see>. The value can be <see langkeyword="null">null</see>.</param>
        /// <remarks>
        /// <para>If the <see cref="Count">count</see> already equals the capacity, the capacity of the <see cref="ObservableStack{T}">stack</see>
        /// is increased by automatically reallocating the internal array, and the existing elements are copied to the new array before the
        /// new element is added.</para>
        /// <para>A <c>null</c>can be pushed onto the <seealso cref="ObservableStack{T}">stack</seealso> as a placeholder,
        /// if needed.</para>
        /// <para>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual void Push( T item )
        {
            var index = this.PushItem( item );

            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ObservableStack{T}">stack</see>,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            int num = (int) ( this.items.Length * 0.9 );

            if ( this.size >= num )
                return;

            var destinationArray = new T[this.size];
            Contract.Assume( destinationArray.Length <= this.items.GetLowerBound( 0 ) + this.items.Length ); 
            Array.Copy( this.items, 0, destinationArray, 0, this.size );
            this.items = destinationArray;
            this.version++;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ObservableStack{T}">stack</see> to an <see cref="Array">array</see>,
        /// starting at a particular <see cref="Array">array</see> index.
        /// </summary>
        /// <remarks>
        /// <para>The elements are copied onto the array in last-in-first-out (LIFO) order, similar to the order
        /// of the elements returned by a succession of calls to <see cref="Pop"/>.</para>
        /// </remarks>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the elements copied
        /// from the <see cref="ObservableStack{T}">stack</see>. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public virtual void CopyTo( T[] array, int arrayIndex )
        {
            Array.Copy( this.items, 0, array, arrayIndex, this.size );
            Array.Reverse( array, arrayIndex, this.size );
        }

        /// <summary>
        /// Determines whether the <see cref="ObservableStack{T}">stack</see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ObservableStack{T}">stack</see>. The value can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the <see cref="ObservableStack{T}">stack</see>;
        /// otherwise, <see langkeyword="false">false</see>.</returns>
        public virtual bool Contains( T item )
        {
            if ( item == null )
                return false;

            var index = this.size;

            while ( index-- > 0 )
            {
                if ( item == null )
                {
                    Contract.Assume( index >= 0 && index < this.items.Length ); 
                    if ( this.items[index] == null )
                        return true;
                }
                else
                {
                    Contract.Assume( index >= 0 && index < this.items.Length ); 
                    if ( this.items[index] != null && this.Comparer.Equals( this.items[index], item ) )
                        return true;
                }
            }

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Required because IStack<T>.CopyTo has different code contract semantics." )]
        void ICollection<T>.CopyTo( T[] array, int arrayIndex )
        {
            this.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Removes all objects from the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <remarks>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Reset"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual void Clear()
        {
            Contract.Assume( ( this.items.Length - this.items.GetLowerBound( 0 ) ) >= this.size );
            Array.Clear( this.items, 0, this.size );
            this.size = 0;
            this.version++;
            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Supports ICollection<T>, but only the Push method should be exposed." )]
        void ICollection<T>.Add( T item )
        {
            this.Push( item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not supported by a stack." )]
        bool ICollection<T>.Remove( T item )
        {
            if ( this.Count > 0 && this.Comparer.Equals( item, this.Peek() ) )
            {
                this.Pop();
                return true;
            }

            throw new InvalidOperationException( ExceptionMessage.StackDoesNotSupportRemove );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This collection is never read-only." )]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The strongly typed CopyTo version should be used." )]
        void ICollection.CopyTo( Array array, int index )
        {
            var typedArray = array as T[];

            if ( typedArray == null )
                throw new ArrayTypeMismatchException( ExceptionMessage.ArrayMismatch );

            this.CopyTo( typedArray, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ObservableStack{T}">stack</see>.</value>
        public int Count
        {
            get
            {
                return this.size;
            }
        }
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class does not support synchronization." )]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="ObservableStack{T}">stack</see>.</value>
        public object SyncRoot
        {
            get
            {
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the <see cref="ObservableStack{T}">stack</see>.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return new Enumerator( this );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
