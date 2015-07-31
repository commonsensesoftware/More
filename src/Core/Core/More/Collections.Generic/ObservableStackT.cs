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
                Contract.Requires( stack != null );
                this.stack = stack;
                version = this.stack.version;
                index = -2;
                currentElement = default( T );
            }

            public void Dispose()
            {
                index = -1;
            }

            public bool MoveNext()
            {
                if ( version != stack.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                var flag = false;

                if ( index == -2 )
                {
                    index = stack.size - 1;
                    flag = index >= 0;

                    if ( flag )
                    {
                        Contract.Assume( stack.items != null );
                        currentElement = stack.items[index];
                    }

                    return flag;
                }

                if ( index == -1 )
                    return false;

                flag = --index >= 0;

                if ( flag )
                {
                    Contract.Assume( stack.items != null );
                    currentElement = stack.items[index];
                    return true;
                }

                currentElement = default( T );
                return false;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if ( version != stack.version )
                    throw new InvalidOperationException( ExceptionMessage.CollectionModified );

                index = -2;
                currentElement = default( T );
            }

            public T Current
            {
                get
                {
                    if ( index < 0 )
                        throw new InvalidOperationException( ExceptionMessage.CollectionBeforeFirst );
                    else if ( index >= stack.Count )
                        throw new InvalidOperationException( ExceptionMessage.CollectionAfterLast );

                    return currentElement;
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
            : this( EmptyArray, comparer )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that contains elements copied
        /// from the specified sequence and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="sequence">The <see cref="IEnumerable{T}">sequence</see> to copy elements from.</param>
        public ObservableStack( IEnumerable<T> sequence )
            : this( sequence, EqualityComparer<T>.Default )
        {
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
            Arg.NotNull( sequence, nameof( sequence ) );
            Arg.NotNull( comparer, nameof( comparer ) );

            var list = sequence.ToList();
            var count = list.Count;

            size = count;
            this.comparer = comparer;
            items = new T[count];

            list.CopyTo( items, 0 );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that is empty and has the specified
        /// initial capacity or the default initial capacity, whichever is greater.</summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableStack{T}">stack</see> can contain.</param>
        public ObservableStack( int capacity )
            : this( capacity, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableStack{T}" /> class that is empty and has the specified
        /// initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="ObservableStack{T}">stack</see> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        public ObservableStack( int capacity, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.GreaterThanOrEqualTo( capacity, 0, "capacity" );

            items = new T[capacity];
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
                return comparer;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

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
            Contract.Ensures( Contract.Result<int>() >= 0 );

            if ( Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.StackIsEmpty );

            version++;
            var index = --size;
            Contract.Assume( index >= 0 && index < items.Length );
            var local = items[index];
            items[size] = default( T );

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
            if ( size == items.Length )
            {
                var capacity = Math.Max( items.Length << 1, 4 );
                var destinationArray = new T[capacity];
                Array.Copy( items, 0, destinationArray, 0, size );
                items = destinationArray;
            }

            var index = size++;
            Contract.Assume( index >= 0 && index < items.Length );
            items[index] = item;
            version++;

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
            if ( Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.StackIsEmpty );

            Contract.EndContractBlock();
            return items[size - 1];
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
            if ( Count <= 0 )
                throw new InvalidOperationException( ExceptionMessage.StackIsEmpty );
            
            Contract.EndContractBlock();

            T local;
            var index = PopItem( out local );

            OnPropertyChanged( "Count" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, local, index ) );

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
            var index = PushItem( item );

            OnPropertyChanged( "Count" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ObservableStack{T}">stack</see>,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            int num = (int) ( items.Length * 0.9 );

            if ( size >= num )
                return;

            var destinationArray = new T[size];
            Contract.Assume( destinationArray.Length <= items.GetLowerBound( 0 ) + items.Length ); 
            Array.Copy( items, 0, destinationArray, 0, size );
            items = destinationArray;
            version++;
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
            Arg.NotNull( array, nameof( array ) );
            Arg.InRange( arrayIndex, 0, array.Length + Count, "arrayIndex" );

            Array.Copy( items, 0, array, arrayIndex, size );
            Array.Reverse( array, arrayIndex, size );
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

            var index = size;

            while ( index-- > 0 )
            {
                if ( item == null )
                {
                    Contract.Assume( index >= 0 && index < items.Length ); 
                    if ( items[index] == null )
                        return true;
                }
                else
                {
                    Contract.Assume( index >= 0 && index < items.Length ); 
                    if ( items[index] != null && Comparer.Equals( items[index], item ) )
                        return true;
                }
            }

            return false;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Required because IStack<T>.CopyTo has different code contract semantics." )]
        void ICollection<T>.CopyTo( T[] array, int arrayIndex )
        {
            CopyTo( array, arrayIndex );
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
            Contract.Assume( ( items.Length - items.GetLowerBound( 0 ) ) >= size );
            Array.Clear( items, 0, size );
            size = 0;
            version++;
            OnPropertyChanged( "Count" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Supports ICollection<T>, but only the Push method should be exposed." )]
        void ICollection<T>.Add( T item )
        {
            Push( item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not supported by a stack." )]
        bool ICollection<T>.Remove( T item )
        {
            if ( Count > 0 && Comparer.Equals( item, Peek() ) )
            {
                Pop();
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

            CopyTo( typedArray, index );
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableStack{T}">stack</see>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ObservableStack{T}">stack</see>.</value>
        public int Count
        {
            get
            {
                return size;
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
                return syncRoot;
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
            return GetEnumerator();
        }
    }
}
