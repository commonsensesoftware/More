namespace More.Windows.Data
{
    using global::Windows.Foundation;
    using global::Windows.Foundation.Collections;
    using global::Windows.UI.Xaml.Data;
    using More.Collections.Generic;
    using More.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a virtualized collection view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the virtualized collection.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is the standard naming convention for a view of a collection." )]
    [SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The disposable pattern is used to manage event handler management. There are no resources to actually dispose of." )]
    public class VirtualCollectionView<T> : ReadOnlyObservableCollection<T>, IFrozenItemCollectionView, IDeferrable
    {
        const int CanChangePageFlag = 0;
        const int IsPageChangingFlag = 1;
        readonly IEqualityComparer<T> comparer;
        readonly BitArray flags = new BitArray( 2, false );
        readonly FrozenItemCollection<T> items;
        readonly Lazy<GroupCollection> groups = new Lazy<GroupCollection>( () => new GroupCollection() );
        readonly Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod;
        readonly EventRegistrationTokenTable<VectorChangedEventHandler<object>> vectorChanged = new EventRegistrationTokenTable<VectorChangedEventHandler<object>>();
        T currentItem;
        int deferLevel;
        int deferredPageIndex = -1;
        int currentPosition = -1;
        int currentPageIndex = -1;
        int pageSize = 10;
        int itemCount = 0;
        int totalCount = -1;

        /// <summary>
        /// Finalizes an instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        ~VirtualCollectionView()
        {
            if ( items != null )
            {
                items.FrozenItems.CollectionChanged -= OnFrozenItemsChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        /// <param name="pagingMethod">The <see cref="Func{T1,TResult}">function</see> used to retrieve a data page.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public VirtualCollectionView( Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod )
            : this( pagingMethod, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        /// <param name="pagingMethod">The <see cref="Func{T1,TResult}">function</see> used to retrieve a data page.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public VirtualCollectionView( Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod, IEqualityComparer<T> comparer )
            : base( new FrozenItemCollection<T>() )
        {
            Arg.NotNull( pagingMethod, nameof( pagingMethod ) );
            Arg.NotNull( comparer, nameof( comparer ) );

            items = (FrozenItemCollection<T>) Items;
            items.FrozenItems.CollectionChanged += OnFrozenItemsChanged;
            this.pagingMethod = pagingMethod;
            this.comparer = comparer;
        }

        bool IsLoadDeferred => deferLevel > 0;

        /// <summary>
        /// Gets the comparer used to evaluate the equality of items in the collection.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}"/> object.  The default state is <see cref="EqualityComparer{T}.Default"/>.</value>
        protected virtual IEqualityComparer<T> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null );
                return comparer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection has loaded at least once.
        /// </summary>
        /// <value>True if the collection has loaded; otherwise, false.</value>
        protected virtual bool HasBeenLoaded
        {
            get
            {
                var value = TotalItemCount > -1;
                return value;
            }
        }

        /// <summary>
        /// Gets the total page count for the collection.
        /// </summary>
        /// <value>The total page count for the collection.</value>
        public int PageCount
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                var value = (int) Math.Ceiling( (double) ItemCount / (double) PageSize );
                return value;
            }
        }

        /// <summary>
        /// Gets the last page index in the collection.
        /// </summary>
        /// <value>The last page index in the collection.</value>
        protected int LastPageIndex
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= -1 );
                var value = PageCount - 1;
                return value;
            }
        }

        /// <summary>
        /// Gets the current item in the collection view.
        /// </summary>
        /// <value>The current item of type <typeparamref name="T"/> in the collection.</value>
        public T CurrentItem
        {
            get => currentItem;
            private set
            {
                currentItem = value;
                OnCurrentChanged( EventArgs.Empty );
                OnPropertyChanged( nameof( CurrentItem ) );
            }
        }

        /// <summary>
        /// Gets the sequence of items in the source collection.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        public IEnumerable<T> SourceCollection => Items;

        /// <summary>
        /// Gets or sets the position of frozen items in the collection view.
        /// </summary>
        /// <value>One of the <see cref="FrozenItemPosition"/> values.</value>
        public FrozenItemPosition FrozenItemPosition
        {
            get => items.FrozenItemPosition;
            set
            {
                if ( items.FrozenItemPosition == value )
                {
                    return;
                }

                items.FrozenItemPosition = value;
                OnPropertyChanged( nameof( FrozenItemPosition ) );
            }
        }

        IEnumerable IFrozenItemCollectionView.FrozenItems => FrozenItems;

        IEnumerable IFrozenItemCollectionView.UnfrozenItems => UnfrozenItems;

        /// <summary>
        /// Gets an observable collection of frozen items.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">observable collection</see> of frozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        public ObservableCollection<T> FrozenItems => items.FrozenItems;

        /// <summary>
        /// Gets a read-only, observable collection of unfrozen items.
        /// </summary>
        /// <value>A read-only, <see cref="IList{T}">observable collection</see> of unfrozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        /// <remarks>The default <see cref="IList{T}">list</see> implementation also implements
        /// <see cref="INotifyCollectionChanged"/> and <see cref="System.ComponentModel.INotifyPropertyChanged"/>.</remarks>
        public IList<T> UnfrozenItems => items.UnfrozenItems;

        /// <summary>
        /// Gets any collection groups that are associated with the view.
        /// </summary>
        /// <value>A vector collection of possible views.</value>
        [CLSCompliant( false )]
        public virtual IObservableVector<object> CollectionGroups => groups.Value;

        /// <summary>
        /// Occurs when the current position in the collection has changed.
        /// </summary>
        public event EventHandler<object> CurrentChanged;

        /// <summary>
        /// Occurs when the current position in the collection is about to change.
        /// </summary>
        [CLSCompliant( false )]
        public event CurrentChangingEventHandler CurrentChanging;

        object ICollectionView.CurrentItem => CurrentItem;

        /// <summary>
        /// Gets the current position in the collection.
        /// </summary>
        /// <value>The zero-based index of the current position in the collection.</value>
        public int CurrentPosition
        {
            get => currentPosition;
            private set
            {
                if ( currentPosition == value )
                {
                    return;
                }

                currentPosition = value;
                OnPropertyChanged( nameof( CurrentPosition ) );
                OnPropertyChanged( nameof( IsCurrentBeforeFirst ) );
                OnPropertyChanged( nameof( IsCurrentAfterLast ) );
            }
        }

        /// <summary>
        /// Returns an object that can be used to defer loaded operations to the collection.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object.</returns>
        public virtual IDisposable DeferLoad() => new DeferManager( this );

        /// <summary>
        /// Gets a value indicating whether the view has more, unloaded items.
        /// </summary>
        /// <value>True if additional unloaded items remain in the view; otherwise, false.</value>
        public virtual bool HasMoreItems => HasBeenLoaded && !IsEmpty;

        /// <summary>
        /// Gets a value indicating whether the current position in the collection is after the last item.
        /// </summary>
        /// <value>True if the current position in the collection is after the last item; otherwise, false.</value>
        public virtual bool IsCurrentAfterLast => CurrentPosition >= Count;

        /// <summary>
        /// Gets a value indicating whether the current position in the collection is before the first item.
        /// </summary>
        /// <value>True if the current position in the collection is before the first item; otherwise, false.</value>
        public virtual bool IsCurrentBeforeFirst => CurrentPosition < 0;

        /// <summary>
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        /// <value>True if the collection is empty; otherwise, false.</value>
        public virtual bool IsEmpty => Count == 0;

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the <see cref="LoadMoreItemsResult">results</see>.</returns>
        /// <remarks>The number of items loaded is based on the <see cref="PageSize">page size</see>.</remarks>
        [CLSCompliant( false )]
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync() => LoadMoreItemsAsync( (uint) PageSize );

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">The number of items to load.</param>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the <see cref="LoadMoreItemsResult">results</see>.</returns>
        [CLSCompliant( false )]
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync( uint count )
        {
            Arg.LessThanOrEqualTo( count, (uint) int.MaxValue, nameof( count ) );

            PageSize = (int) count;

            if ( IsLoadDeferred )
            {
#pragma warning disable SA1129 // Do not use default value type constructor
                return Task.FromResult( new LoadMoreItemsResult() ).AsAsyncOperation();
#pragma warning restore SA1129 // Do not use default value type constructor
            }

            var pageIndex = PageIndex;

            // is this a callback from a deferred load?
            if ( deferredPageIndex > -1 )
            {
                pageIndex = deferredPageIndex;
                deferredPageIndex = -1;
            }

            // if the page index is unset, trigger moving to the first page and return the current count
            if ( pageIndex < 0 )
            {
                MoveToFirstPageAsync();
                var result = new LoadMoreItemsResult() { Count = (uint) Items.Count };
                return Task.FromResult( result ).AsAsyncOperation();
            }

            return LoadPageAsync( pageIndex ).AsAsyncOperation();
        }

        async Task<LoadMoreItemsResult> LoadPageAsync( int pageIndex )
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            var result = new LoadMoreItemsResult();
#pragma warning restore SA1129 // Do not use default value type constructor

            if ( PageIndex != pageIndex )
            {
                var args = new PageChangingEventArgs( pageIndex );
                IsPageChanging = true;
                OnPageChanging( args );

                if ( args.Cancel )
                {
                    IsPageChanging = false;
                    result.Count = (uint) Items.Count;
                    return result;
                }
            }

            // if a load is queued while we're deferring, keep track of the last index.
            // this could triggered by actions other than the LoadMoreItemsAsync method (ex: grouping)
            if ( IsLoadDeferred )
            {
                deferredPageIndex = pageIndex;
                result.Count = (uint) Items.Count;
                return result;
            }

            var actualPageSize = Math.Max( PageSize - FrozenItems.Count, 1 );
            var arguments = new PagingArguments( pageIndex, actualPageSize );
            var pagedItems = await pagingMethod( arguments ).ConfigureAwait( true );

            result.Count = OnLoaded( pagedItems );
            OnLoadComplete( pageIndex );

            return result;
        }

        uint OnLoaded( PagedCollection<T> pagedItems )
        {
            Contract.Requires( pagedItems != null );

            Items.ReplaceAll( pagedItems );
            ItemCount = (int) pagedItems.TotalCount;
            TotalItemCount = ItemCount + FrozenItems.Count;
            MoveCurrentToPosition( -1, false );

            return (uint) Items.Count;
        }

        void OnLoadComplete( int pageIndex )
        {
            if ( !IsPageChanging )
            {
                return;
            }

            IsPageChanging = false;
            PageIndex = pageIndex;
            OnPageChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs args )
        {
            base.OnCollectionChanged( args );
            vectorChanged.InvocationList?.Invoke( this, new VectorChangedEventArgs( args ) );
        }

        /// <summary>
        /// Raises the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

        /// <summary>
        /// Raises the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected override void OnPropertyChanged( PropertyChangedEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            base.OnPropertyChanged( args );

            switch ( args.PropertyName )
            {
                case nameof( Count ):
                    {
                        OnPropertyChanged( nameof( IsEmpty ) );
                        break;
                    }
            }
        }

        /// <summary>
        /// Raises the <see cref="PageChangingEventArgs"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PageChangingEventArgs"/> event data.</param>
        protected virtual void OnPageChanging( PageChangingEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            PageChanging?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="PageChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnPageChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            PageChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CurrentChangingEventArgs"/> event data.</param>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnCurrentChanging( CurrentChangingEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CurrentChanging?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "This is the standard raise event signature." )]
        protected virtual void OnCurrentChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CurrentChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Moves the current position in the collection to the item at the specified index.
        /// </summary>
        /// <param name="position">The zero-based index in the collection set as the current position.</param>
        /// <param name="cancelable">Indicates whether the operation can be canceled.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        protected virtual bool MoveCurrentToPosition( int position, bool cancelable )
        {
            if ( CurrentPosition == position && !IsPageChanging )
            {
                return false;
            }

            var args = new CurrentChangingEventArgs( cancelable );
            OnCurrentChanging( args );

            if ( args.IsCancelable && args.Cancel )
            {
                return false;
            }

            if ( position < 0 || position >= Count )
            {
                CurrentPosition = position < 0 ? -1 : Count;
                CurrentItem = default( T );
                return false;
            }

            CurrentItem = Items[position];
            CurrentPosition = position;
            return true;
        }

        bool ICollectionView.MoveCurrentTo( object item ) => MoveCurrentTo( (T) item );

        /// <summary>
        /// Moves the current position in the collection to the specified item.
        /// </summary>
        /// <param name="item">The item to move to.</param>
        /// <returns>True if the current position moves to the specified item; otherwise, false.</returns>
        public bool MoveCurrentTo( T item )
        {
            if ( !Contains( item ) )
            {
                return false;
            }

            if ( Comparer.Equals( CurrentItem, item ) )
            {
                return false;
            }

            var args = new CurrentChangingEventArgs( true );
            OnCurrentChanging( args );

            if ( args.Cancel )
            {
                return false;
            }

            var position = 0;

            using ( var iterator = Items.GetEnumerator() )
            {
                while ( iterator.MoveNext() )
                {
                    if ( Comparer.Equals( iterator.Current, item ) )
                    {
                        CurrentPosition = position;
                        break;
                    }

                    ++position;
                }
            }

            CurrentItem = item;
            return true;
        }

        /// <summary>
        /// Moves the current position in the collection to the first item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToFirst() => MoveCurrentToPosition( 0 );

        /// <summary>
        /// Moves the current position in the collection to the last item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToLast() => MoveCurrentToPosition( Count - 1 );

        /// <summary>
        /// Moves the current position in the collection to the next item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToNext() => MoveCurrentToPosition( CurrentPosition + 1 );

        /// <summary>
        /// Moves the current position in the collection to the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index in the collection set as the current position.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPosition( int index ) => MoveCurrentToPosition( index, true );

        /// <summary>
        /// Moves the current position in the collection to the previous item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPrevious() => MoveCurrentToPosition( CurrentPosition - 1 );

        /// <summary>
        /// Gets or sets a value indicating whether the collection can change pages.
        /// </summary>
        /// <value>True if the collection can change pages; otherwise, false.</value>
        public bool CanChangePage
        {
            get => flags[CanChangePageFlag];
            protected set
            {
                if ( flags[CanChangePageFlag] == value )
                {
                    return;
                }

                flags[CanChangePageFlag] = value;
                OnPropertyChanged( nameof( CanChangePage ) );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection is changing pages.
        /// </summary>
        /// <value>True if the collection is changing pages; otherwise, false.</value>
        public bool IsPageChanging
        {
            get => flags[IsPageChangingFlag];
            protected set
            {
                if ( flags[IsPageChangingFlag] == value )
                {
                    return;
                }

                flags[IsPageChangingFlag] = value;
                OnPropertyChanged( nameof( IsPageChanging ) );
            }
        }

        /// <summary>
        /// Gets the total number of items in the collection before paging is applied.
        /// </summary>
        /// <value>The total number of item in the collection before paging is applied.</value>
        public int ItemCount
        {
            get => itemCount;
            private set
            {
                if ( itemCount == value )
                {
                    return;
                }

                itemCount = value;
                OnPropertyChanged( nameof( ItemCount ) );
                OnPropertyChanged( nameof( PageCount ) );
                CanChangePage = PageCount > 1;
            }
        }

        /// <summary>
        /// Moves the collection to the first data page asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToFirstPageAsync() => MoveToPageAsync( 0 );

        /// <summary>
        /// Moves the collection to the last data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToLastPageAsync() => Count == 0 ? Task.FromResult( false ) : MoveToPageAsync( LastPageIndex );

        /// <summary>
        /// Moves the collection to the next data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToNextPageAsync() => MoveToPageAsync( PageIndex + 1 );

        /// <summary>
        /// Moves the collection to the data page at the specified page index.
        /// </summary>
        /// <param name="pageIndex">The zero-based page index to move to.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public virtual async Task<bool> MoveToPageAsync( int pageIndex )
        {
            // page index is out of range (note: TotalItemCount = -1 until the collection is loaded at least once)
            if ( pageIndex < 0 || ( HasBeenLoaded && pageIndex > LastPageIndex ) )
            {
                return false;
            }

            // note: load even if this is the same page index because external conditions may have changed
            await LoadPageAsync( pageIndex ).ConfigureAwait( true );
            return true;
        }

        /// <summary>
        /// Moves the collection to the previous data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToPreviousPage() => MoveToPageAsync( PageIndex - 1 );

        /// <summary>
        /// Occurs when the current data page has changed.
        /// </summary>
        public event EventHandler<EventArgs> PageChanged;

        /// <summary>
        /// Occurs when the current data page is about to change.
        /// </summary>
        public event EventHandler<PageChangingEventArgs> PageChanging;

        /// <summary>
        /// Gets the current data page index.
        /// </summary>
        /// <value>The zero-based index of the current data page.</value>
        public int PageIndex
        {
            get => currentPageIndex;
            private set
            {
                if ( currentPageIndex == value )
                {
                    return;
                }

                currentPageIndex = value;
                OnPropertyChanged( nameof( PageIndex ) );
                CanChangePage = PageCount > 1;
            }
        }

        /// <summary>
        /// Gets or sets the size of a data page.
        /// </summary>
        /// <value>The size (number of items) of a data page.</value>
        public int PageSize
        {
            get => pageSize;
            set
            {
                Arg.GreaterThan( value, 0, nameof( value ) );

                if ( pageSize == value )
                {
                    return;
                }

                pageSize = value;
                OnPropertyChanged( nameof( PageSize ) );

                if ( !HasBeenLoaded )
                {
                    return;
                }

                OnPropertyChanged( nameof( PageCount ) );
                CanChangePage = PageCount > 1;

                if ( PageIndex > LastPageIndex )
                {
                    MoveToLastPageAsync();
                }
                else
                {
#pragma warning disable 4014
                    // this operation triggers an async operation and async operations cannot be awaited within properties
                    LoadPageAsync( PageIndex );
#pragma warning restore 4014
                }
            }
        }

        /// <summary>
        /// Gets the total, virtualized number of items in the collection.
        /// </summary>
        /// <value>The total, virtualized number of items in the collection or -1 if the total count is unknown.</value>
        /// <remarks>This property will always return -1 until the first data page is retrieved.</remarks>
        public int TotalItemCount
        {
            get => totalCount;
            private set
            {
                if ( totalCount == value )
                {
                    return;
                }

                totalCount = value;
                OnPropertyChanged( nameof( TotalItemCount ) );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        void IDeferrable.BeginDefer() => Interlocked.Increment( ref deferLevel );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        async void IDeferrable.EndDefer()
        {
            if ( Interlocked.Decrement( ref deferLevel ) == 0 )
            {
                await LoadMoreItemsAsync();
            }
        }

        async void OnFrozenItemsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // NOTE: NotifyCollectionChangedAction.Replace does not trigger any changes
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    if ( e.NewItems == null || e.NewItems.Count == 0 || Items.Count <= PageSize )
                    {
                        return;
                    }

                    if ( PageIndex != LastPageIndex )
                    {
                        await LoadPageAsync( PageIndex ).ConfigureAwait( true );
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if ( e.OldItems == null || e.OldItems.Count == 0 )
                    {
                        return;
                    }

                    if ( Items.Count < PageSize && PageIndex != LastPageIndex )
                    {
                        await LoadPageAsync( PageIndex ).ConfigureAwait( true );
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    if ( Items.Count < PageSize && PageIndex != LastPageIndex )
                    {
                        await LoadPageAsync( PageIndex ).ConfigureAwait( true );
                    }

                    break;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use INotifyCollectionChanged." )]
        event VectorChangedEventHandler<object> IObservableVector<object>.VectorChanged
        {
            add => vectorChanged.AddEventHandler( value );
            remove => vectorChanged.RemoveEventHandler( value );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        int IList<object>.IndexOf( object item ) => ( (IList<T>) this ).IndexOf( (T) item );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        void IList<object>.Insert( int index, object item ) => ( (IList<T>) this ).Insert( index, (T) item );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        object IList<object>.this[int index]
        {
            get => this[index];
            set => ( (IList<T>) this )[index] = (T) value;
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        void ICollection<object>.Add( object item ) => ( (IList<T>) this ).Add( (T) item );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        bool ICollection<object>.Contains( object item ) => Contains( (T) item );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        void ICollection<object>.CopyTo( object[] array, int arrayIndex )
        {
            Arg.NotNull( array, nameof( array ) );
            var temp = new T[array.Length];
            CopyTo( temp, arrayIndex );
            temp.Cast<object>().ToArray().CopyTo( array, arrayIndex );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        bool ICollection<object>.Remove( object item ) => ( (IList<T>) this ).Remove( (T) item );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        void IList<object>.RemoveAt( int index ) => ( (IList<T>) this ).RemoveAt( index );

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        void ICollection<object>.Clear() => ( (IList<T>) this ).Clear();

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Intentionally hidden. Inheritors should use the typed overload." )]
        bool ICollection<object>.IsReadOnly => ( (IList<T>) this ).IsReadOnly;

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach ( object item in this )
            {
                yield return item;
            }
        }

        sealed class VectorChangedEventArgs : IVectorChangedEventArgs
        {
            internal VectorChangedEventArgs( NotifyCollectionChangedEventArgs args )
            {
                Contract.Requires( args != null );

                switch ( args.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        CollectionChange = CollectionChange.ItemInserted;
                        Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Move:
                        CollectionChange = CollectionChange.ItemChanged;
                        Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        CollectionChange = CollectionChange.ItemRemoved;
                        Index = (uint) Math.Max( args.OldStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        CollectionChange = CollectionChange.ItemChanged;
                        Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        CollectionChange = CollectionChange.Reset;
                        break;
                }
            }

            public CollectionChange CollectionChange { get; private set; }

            public uint Index { get; private set; }
        }

        /// <summary>
        /// Represents a dummy collection for CollectionGroups that only throws an exception when an attempt is made to add
        /// a group description (as opposed to when the GroupDescriptions property is accessed). All other operations are
        /// either safe as they are or will throw an appropriate exception.
        /// </summary>
        sealed class GroupCollection : ReadOnlyObservableCollection<object>, IObservableVector<object>
        {
            internal GroupCollection() : base( new ObservableCollection<object>() ) { }

            protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
            {
                base.OnCollectionChanged( e );
                VectorChanged?.Invoke( this, new VectorChangedEventArgs( e ) );
            }

            public event VectorChangedEventHandler<object> VectorChanged;
        }
    }
}