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
        struct Enumerator : IEnumerator<T>
        {
            readonly ObservableQueue<T> queue;
            readonly int version;
            int index;
            T current;

            internal Enumerator( ObservableQueue<T> queue )
            {
                Contract.Requires( queue != null );
                this.queue = queue;
                version = this.queue.version;
                index = -1;
                current = default( T );
            }

            public void Dispose()
            {
                index = -2;
                current = default( T );
            }

            public bool MoveNext()
            {
                if ( version != queue.version )
                {
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );
                }

                if ( index == -2 )
                {
                    return false;
                }

                index++;

                if ( index == queue.size )
                {
                    index = -2;
                    current = default( T );
                    return false;
                }

                current = queue.GetElement( index );

                return true;
            }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if ( version != queue.version )
                {
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );
                }

                index = -1;
                current = default( T );
            }

            public T Current
            {
                get
                {
                    if ( index < 0 )
                    {
                        throw new InvalidOperationException( ExceptionMessage.CollectionBeforeFirst );
                    }
                    else if ( index >= queue.Count )
                    {
                        throw new InvalidOperationException( ExceptionMessage.CollectionAfterLast );
                    }

                    return current;
                }
            }
        }

        static readonly T[] EmptyArray = new T[0];
        readonly IEqualityComparer<T> comparer;
        T[] items;
        int head;
        int size;
        int tail;
        int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and has the default initial capacity.
        /// </summary>
        public ObservableQueue() : this( EmptyArray, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and has the default initial capacity.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        public ObservableQueue( IEqualityComparer<T> comparer ) : this( EmptyArray, comparer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that contains elements copied from the specified
        /// sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The sequence whose elements are copied to the new <see cref="ObservableQueue{T}">stack</see>.</param>
        public ObservableQueue( IEnumerable<T> sequence ) : this( sequence, EqualityComparer<T>.Default ) { }

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
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( comparer, nameof( comparer ) );

            var list = sequence.ToList();
            var count = list.Count;

            size = count;
            tail = count;
            this.comparer = comparer;
            items = new T[count];

            list.CopyTo( items, 0 );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and
        /// has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableQueue{T}" /> can contain.</param>
        public ObservableQueue( int capacity ) : this( capacity, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableQueue{T}" /> class that is empty and
        /// has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableQueue{T}">stack</see> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        public ObservableQueue( int capacity, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.GreaterThanOrEqualTo( capacity, 0, nameof( capacity ) );

            items = new T[capacity];
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
                return comparer;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            PropertyChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CollectionChanged?.Invoke( this, e );
        }

        internal T GetElement( int i )
        {
            var index = ( head + i ) % items.Length;
            Contract.Assume( index >= 0 && index < items.Length );
            return items[index];
        }

        void SetCapacity( int capacity )
        {
            Contract.Requires( capacity >= 0 );

            var destinationArray = new T[capacity];

            if ( size > 0 )
            {
                if ( head < tail )
                {
                    Contract.Assume( size <= items.GetLowerBound( 0 ) + items.Length );
                    Array.Copy( items, head, destinationArray, 0, size );
                }
                else
                {
                    Contract.Assume( items.Length - head <= items.GetLowerBound( 0 ) + items.Length );
                    Array.Copy( items, head, destinationArray, 0, items.Length - head );
                    Contract.Assume( tail <= items.GetLowerBound( 0 ) + items.Length );
                    Array.Copy( items, 0, destinationArray, items.Length - head, tail );
                }
            }

            items = destinationArray;
            head = 0;
            tail = ( size == capacity ) ? 0 : size;
            version++;
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

            if ( Count <= 0 )
            {
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );
            }

            var local = items[head];
            items[head] = default( T );
            head = ( head + 1 ) % items.Length;
            var index = --size;
            version++;

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
            if ( size == items.Length )
            {
                var capacity = Math.Max( items.Length << 1, items.Length + 4 );
                SetCapacity( capacity );
            }

            items[tail] = item;
            tail = ( tail + 1 ) % items.Length;
            var index = size++;
            version++;

            return index;
        }

        /// <summary>
        /// Copies the <see cref="ObservableQueue{T}">queue</see> elements to a new items.
        /// </summary>
        /// <returns>A new items containing elements copied from the <see cref="ObservableQueue{T}">queue</see>.</returns>
        public T[] ToArray()
        {
            var destinationArray = new T[size];

            if ( size != 0 )
            {
                if ( head < tail )
                {
                    Array.Copy( items, head, destinationArray, 0, size );
                    return destinationArray;
                }

                Array.Copy( items, head, destinationArray, 0, items.Length - head );
                Array.Copy( items, 0, destinationArray, items.Length - head, tail );
            }

            return destinationArray;
        }

        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have
        /// changed by using either <c>null</c>or <see cref="string.Empty"/> as the
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
            if ( Count <= 0 )
            {
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );
            }

            Contract.EndContractBlock();

            return items[head];
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
        ///         <description>The <see cref="PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="NotifyCollectionChangedEventArgs.Action"/> as <see cref="NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual T Dequeue()
        {
            if ( Count <= 0 )
            {
                throw new InvalidOperationException( ExceptionMessage.QueueIsEmpty );
            }

            Contract.EndContractBlock();

            T local;
            var index = DequeueItem( out local );
            OnPropertyChanged( nameof( Count ) );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, local, index ) );
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
        ///         <description>The <see cref="PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="NotifyCollectionChangedEventArgs.Action"/> as <see cref="NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public virtual void Enqueue( T item )
        {
            var index = EnqueueItem( item );
            OnPropertyChanged( nameof( Count ) );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ObservableQueue{T}">queue</see>,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            var num = (int) ( items.Length * 0.9 );

            if ( size < num )
            {
                SetCapacity( size );
            }
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
            Arg.NotNull( array, nameof( array ) );
            Arg.InRange( arrayIndex, 0, array.Length + Count, nameof( arrayIndex ) );

            var length = array.Length;
            var count = ( ( length - arrayIndex ) < size ) ? ( length - arrayIndex ) : size;

            if ( count == 0 )
            {
                return;
            }

            var len = ( ( items.Length - head ) < count ) ? ( items.Length - head ) : count;
            Array.Copy( items, head, array, arrayIndex, len );
            count -= len;

            if ( count > 0 )
            {
                Array.Copy( items, 0, array, ( arrayIndex + items.Length ) - head, count );
            }
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
            {
                return false;
            }

            var index = head;
            var count = size;

            while ( count-- > 0 )
            {
                if ( item == null )
                {
                    if ( items[index] == null )
                    {
                        return true;
                    }
                }
                else if ( items[index] != null && Comparer.Equals( items[index], item ) )
                {
                    return true;
                }

                index = ( index + 1 ) % items.Length;
            }

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Required because IStack<T>.CopyTo has different code contract semantics." )]
        void ICollection<T>.CopyTo( T[] array, int arrayIndex ) => CopyTo( array, arrayIndex );

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
        ///         <description>The <see cref="PropertyChangedEventArgs.PropertyName"/> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="NotifyCollectionChangedEventArgs.Action"/> as <see cref="NotifyCollectionChangedAction.Reset"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public virtual void Clear()
        {
            if ( head < tail )
            {
                Array.Clear( items, head, size );
            }
            else
            {
                Array.Clear( items, head, items.Length - head );
                Array.Clear( items, 0, tail );
            }

            head = 0;
            tail = 0;
            size = 0;
            version++;

            OnPropertyChanged( nameof( Count ) );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Supports ICollection<T>, but only the Enqueue method should be exposed." )]
        void ICollection<T>.Add( T item ) => Enqueue( item );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not supported by a queue." )]
        bool ICollection<T>.Remove( T item )
        {
            if ( Count > 0 && Comparer.Equals( item, Peek() ) )
            {
                Dequeue();
                return true;
            }

            throw new InvalidOperationException( ExceptionMessage.QueueDoesNotSupportRemove );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This property will always be false." )]
        bool ICollection<T>.IsReadOnly => false;

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Legacy method." )]
        void ICollection.CopyTo( Array array, int index )
        {
            var typedArray = array as T[];

            if ( typedArray == null )
            {
                throw new ArrayTypeMismatchException( ExceptionMessage.ArrayMismatch );
            }

            CopyTo( typedArray, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ObservableQueue{T}">queue</see>.</value>
        public int Count => size;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This class does not support synchronization." )]
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the <see cref="ObservableQueue{T}">queue</see>.</value>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Returns an enumerator for the <see cref="ObservableQueue{T}">queue</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}">iterator</see> for the <see cref="ObservableQueue{T}">queue</see>.</returns>
        public virtual IEnumerator<T> GetEnumerator() => new Enumerator( this );

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}