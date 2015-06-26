namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts; 
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a first-in, first-out collection of objects with change notification.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of elements in the queue.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "A queue is a specialized type of collection." )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "A queue is a specialized type of collection." )]
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( QueueDebugView<> ) )]
    public class ObservableQueue<T> : IQueue<T>, ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Enumerates the elements of a <see cref="ObservableQueue{T}" /> as the internal implementation cannot simply use the actual array size.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        private struct Enumerator : IEnumerator<T>
        {
            private readonly ObservableQueue<T> queue;
            private readonly int version;
            private int index;
            private T current;

            internal Enumerator( ObservableQueue<T> queue )
            {
                Contract.Requires( queue != null ); 
                this.queue = queue;
                this.version = this.queue.version;
                this.index = -1;
                this.current = default( T );
            }

            public void Dispose()
            {
                this.index = -2;
                this.current = default( T );
            }

            public bool MoveNext()
            {
                if ( this.version != this.queue.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                if ( this.index == -2 )
                    return false;

                this.index++;

                if ( this.index == this.queue.size )
                {
                    this.index = -2;
                    this.current = default( T );
                    return false;
                }

                this.current = this.queue.GetElement( this.index );

                return true;
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
                if ( this.version != this.queue.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                this.index = -1;
                this.current = default( T );
            }

            public T Current
            {
                get
                {
                    if ( this.index < 0 )
                        throw new InvalidOperationException( ExceptionMessage.CollectionBeforeFirst );
                    else if ( this.index >= this.queue.Count )
                        throw new InvalidOperationException( ExceptionMessage.CollectionAfterLast );

                    return this.current;
                }
            }
        }

        private static readonly T[] EmptyArray = new T[0];
        private readonly object syncRoot = new object();
        private readonly IEqualityComparer<T> comparer;
        private T[] items;
        private int head;
        private int size;
        private int tail;
        private int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and has the default initial capacity.
        /// </summary>
        public ObservableQueue()
            : this( EmptyArray, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and has the default initial capacity.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        public ObservableQueue( IEqualityComparer<T> comparer )
            : this( EmptyArray, comparer )
        {
            Arg.NotNull( comparer, "comparer" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that contains elements copied from the specified 
        /// sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The sequence whose elements are copied to the new <see cref="ObservableQueue{T}">stack</see>.</param>
        public ObservableQueue( IEnumerable<T> sequence )
            : this( sequence, EqualityComparer<T>.Default )
        {
            Arg.NotNull( sequence, "sequence" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that contains elements copied from the specified 
        /// sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The sequence whose elements are copied to the new <see cref="ObservableQueue{T}">stack</see>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sequence" /> is <see langkeyword="null">null</see>.</exception>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by code contract." )]
        public ObservableQueue( IEnumerable<T> sequence, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( sequence, "sequence" );
            Arg.NotNull( comparer, "comparer" );

            var list = sequence.ToList();
            var count = list.Count;

            this.size = count;
            this.tail = count;
            this.comparer = comparer;
            this.items = new T[count];

            list.CopyTo( this.items, 0 );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and
        /// has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableQueue{T}" /> can contain.</param>
        public ObservableQueue( int capacity )
            : this( capacity, EqualityComparer<T>.Default )
        {
            Arg.GreaterThanOrEqualTo( capacity, 0, "capacity" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and
        /// has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableQueue{T}">stack</see> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        public ObservableQueue( int capacity, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( comparer, "comparer" );
            Arg.GreaterThanOrEqualTo( capacity, 0, "capacity" );

            this.items = new T[capacity];
            this.comparer = comparer;
        }

        /// <summary>
        /// Gets the comparer used by the collection to compare items.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
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
            Arg.NotNull( e, "e" );

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
            Arg.NotNull( e, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        internal T GetElement( int i )
        {
            var index = ( this.head + i ) % this.items.Length;
            Contract.Assume( index >= 0 && index < this.items.Length ); 
            return this.items[index];
        }

        private void SetCapacity( int capacity )
        {
            Contract.Requires( capacity >= 0 ); 
            var destinationArray = new T[capacity];

            if ( this.size > 0 )
            {
                if ( this.head < this.tail )
                {
                    Contract.Assume( this.size <= this.items.GetLowerBound( 0 ) + this.items.Length ); 
                    Array.Copy( this.items, this.head, destinationArray, 0, this.size );
                }
                else
                {
                    Contract.Assume( this.items.Length - this.head <= this.items.GetLowerBound( 0 ) + this.items.Length ); 
                    Array.Copy( this.items, this.head, destinationArray, 0, this.items.Length - this.head );
                    Contract.Assume( this.tail <= this.items.GetLowerBound( 0 ) + this.items.Length ); 
                    Array.Copy( this.items, 0, destinationArray, this.items.Length - this.head, this.tail );
                }
            }

            this.items = destinationArray;
            this.head = 0;
            this.tail = ( this.size == capacity ) ? 0 : this.size;
            this.version++;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <param name="result">The object removed from the begining of the <see cref="ObservableQueue{T}">queue</see>.</param>
        /// <returns>The index of the item that was removed.</returns>
        /// <remarks>
        /// This method performs the dequeue logic without producing any events or other side effects.
        /// Any overrides of <see cref="Dequeue"/> will not be called.
        /// </remarks>
        [SuppressMessageAttribute( "Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "This method returns the value and the count." )]
        protected int DequeueItem( out T result )
        {
            Contract.Ensures( Contract.Result<int>() >= 0 );

            if ( this.Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );

            var local = this.items[this.head];
            this.items[this.head] = default( T );
            this.head = ( this.head + 1 ) % this.items.Length;
            var index = --this.size;
            this.version++;

            result = local;
            return index;
        }

        /// <summary>
        /// Inserts an object at the end of the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IQueue{T}">queue</see>.</param>
        /// <returns>The index the item was added at.</returns>
        /// <remarks>
        /// This method performs the enqueue logic without producing any events or other side effects.
        /// Any overrides of <see cref="Enqueue"/> will not be called.
        /// </remarks>
        protected int EnqueueItem( T item )
        {
            if ( this.size == this.items.Length )
            {
                var capacity = Math.Max( this.items.Length << 1, this.items.Length + 4 );
                this.SetCapacity( capacity );
            }

            this.items[this.tail] = item;
            this.tail = ( this.tail + 1 ) % this.items.Length;
            var index = this.size++;
            this.version++;

            return index;
        }

        /// <summary>
        /// Copies the <see cref="ObservableQueue{T}">queue</see> elements to a new items.
        /// </summary>
        /// <returns>A new items containing elements copied from the <see cref="ObservableQueue{T}">queue</see>.</returns>
        public T[] ToArray()
        {
            var destinationArray = new T[this.size];

            if ( this.size != 0 )
            {
                if ( this.head < this.tail )
                {
                    Array.Copy( this.items, this.head, destinationArray, 0, this.size );
                    return destinationArray;
                }

                Array.Copy( this.items, this.head, destinationArray, 0, this.items.Length - this.head );
                Array.Copy( this.items, 0, destinationArray, this.items.Length - this.head, this.tail );
            }

            return destinationArray;
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have
        /// changed by using either <c>null</c>or <see cref="F:String.Empty"/> as the
        /// property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <remarks>The event handler receives an argument of type <see cref="NotifyCollectionChangedEventArgs"/>,
        /// which contains data that is related to this event.</remarks>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Returns the object at the begining of the <see cref="ObservableQueue{T}">queue</see> without removing it.
        /// </summary>
        /// <returns>The object at the begining of the <see cref="ObservableQueue{T}">queue</see>.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="Dequeue"/> method, but <seealso cref="Peek"/> does not modify the <see cref="ObservableQueue{T}">queue</see>.</para>
        /// <para>A <c>null</c>can be added to the <see cref="ObservableQueue{T}">queue</see> as a placeholder, if needed.</para>
        /// </remarks>
        public virtual T Peek()
        {
            if ( this.Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );
            
            Contract.EndContractBlock();

            return this.items[this.head];
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="IQueue{T}">queue</see>.
        /// </summary>
        /// <returns>The object removed from the begining of the <see cref="IQueue{T}" />.</returns>
        /// <remarks>
        /// <para>This method is similar to the <see cref="IQueue{T}.Peek"/> method, but <seealso cref="IQueue{T}.Peek"/> does not modify the <see cref="IQueue{T}" />.</para>
        /// <para>A <c>null</c>could be dequeued from the <see cref="ObservableQueue{T}">queue</see>.</para>
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
        public virtual T Dequeue()
        {
            if ( this.Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );

            Contract.EndContractBlock();

            T local;
            var index = this.DequeueItem( out local );
            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, local, index ) );
            return local;
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="IQueue{T}">queue</see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="IQueue{T}">queue</see>.</param>
        /// <remarks>
        /// <para>The <see cref="ObservableQueue{T}">queue</see> is implemented as an array.</para>
        /// <para>If the <see cref="Count">count</see> already equals the capacity, the capacity
        /// of the <see cref="ObservableQueue{T}">queue</see> is increased by automatically
        /// reallocating the internal array, and the existing elements are copied to the new
        /// array before the new element is added.</para>
        /// <para>A <c>null</c>can be pushed onto the
        /// <see cref="ObservableQueue{T}">queue</see> as a placeholder, if needed.</para>
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
        public virtual void Enqueue( T item )
        {
            var index = this.EnqueueItem( item );
            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ObservableQueue{T}">queue</see>,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            var num = (int) ( this.items.Length * 0.9 );

            if ( this.size < num )
                this.SetCapacity( this.size );
        }

        /// <summary>
        /// Copies the elements of the <see cref="ObservableQueue{T}">queue</see> to an <see cref="Array">array</see>,
        /// starting at a particular <seealso cref="Array">array</seealso> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array">array</see> that is the destination of the elements
        /// copied from the <see cref="ObservableQueue{T}">queue</see>. The <see cref="Array">array</see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>        
        /// <remarks>
        /// <para>The elements are copied onto the array in first-in-first-out (FIFO) order, similar to the order
        /// of the elements returned by a succession of calls to <see cref="Dequeue"/>.</para>
        /// </remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract." )]
        public virtual void CopyTo( T[] array, int arrayIndex )
        {
            Arg.NotNull( array, "array" );
            Arg.InRange( arrayIndex, 0, array.Length + this.Count, "arrayIndex");

            var length = array.Length;
            var count = ( ( length - arrayIndex ) < this.size ) ? ( length - arrayIndex ) : this.size;

            if ( count == 0 )
                return;

            var len = ( ( this.items.Length - this.head ) < count ) ? ( this.items.Length - this.head ) : count;
            Array.Copy( this.items, this.head, array, arrayIndex, len );
            count -= len;

            if ( count > 0 )
                Array.Copy( this.items, 0, array, ( arrayIndex + this.items.Length ) - this.head, count );
        }

        /// <summary>
        /// Determines whether the <see cref="ObservableQueue{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ObservableQueue{T}">queue</see>.
        /// The value can be null for reference types.</param>
        /// <returns><see langkeyword="true">True</see> if <paramref name="item" /> is found in the
        /// <see cref="ObservableQueue{T}">queue</see>; otherwise, <see langkeyword="false">false</see>.</returns>
        public virtual bool Contains( T item )
        {
            if ( item == null )
                return false;

            var index = this.head;
            var count = this.size;

            while ( count-- > 0 )
            {
                if ( item == null )
                {
                    if ( this.items[index] == null )
                        return true;
                }
                else if ( this.items[index] != null && this.Comparer.Equals( this.items[index], item ) )
                {
                    return true;
                }

                index = ( index + 1 ) % this.items.Length;
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
        /// Removes all objects from the <see cref="ObservableQueue{T}">queue</see>.
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
            if ( this.head < this.tail )
            {
                Array.Clear( this.items, this.head, this.size );
            }
            else
            {
                Array.Clear( this.items, this.head, this.items.Length - this.head );
                Array.Clear( this.items, 0, this.tail );
            }

            this.head = 0;
            this.tail = 0;
            this.size = 0;
            this.version++;

            this.OnPropertyChanged( "Count" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Supports ICollection<T>, but only the Enqueue method should be exposed." )]
        void ICollection<T>.Add( T item )
        {
            this.Enqueue( item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not supported by a queue." )]
        bool ICollection<T>.Remove( T item )
        {
            if ( this.Count > 0 && this.Comparer.Equals( item, this.Peek() ) )
            {
                this.Dequeue();
                return true;
            }

            throw new InvalidOperationException( ExceptionMessage.QueueDoesNotSupportRemove );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This property will always be false." )]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Legacy method." )]
        void ICollection.CopyTo( Array array, int index )
        {
            var typedArray = array as T[];

            if ( typedArray == null )
                throw new ArrayTypeMismatchException( ExceptionMessage.ArrayMismatch );

            this.CopyTo( typedArray, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ObservableQueue{T}">queue</see>.</value>
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
        /// Gets an object that can be used to synchronize access to the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="ObservableQueue{T}">queue</see>.</value>
        public object SyncRoot
        {
            get
            {
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the <see cref="ObservableQueue{T}">queue</see>.</returns>
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
