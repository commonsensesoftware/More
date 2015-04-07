namespace More.Windows.Data
{
    using More.ComponentModel;
    using More.Collections.Generic;
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
    using global::Windows.Foundation;
    using global::Windows.Foundation.Collections;
    using global::Windows.UI.Xaml.Data;

    /// <summary>
    /// Represents a virtualized collection view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the virtualized collection.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is the standard naming convention for a view of a collection." )]
    [SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The disposable pattern is used to manage event handler management. There are no resources to actually dispose of." )]
    public class VirtualCollectionView<T> : ReadOnlyObservableCollection<T>, IFrozenItemCollectionView, IDeferrable
    {
        private sealed class VectorChangedEventArgs : IVectorChangedEventArgs
        {
            internal VectorChangedEventArgs( NotifyCollectionChangedEventArgs args )
            {
                Contract.Requires( args != null );

                switch ( args.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        this.CollectionChange = CollectionChange.ItemInserted;
                        this.Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.CollectionChange = CollectionChange.ItemChanged;
                        this.Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.CollectionChange = CollectionChange.ItemRemoved;
                        this.Index = (uint) Math.Max( args.OldStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.CollectionChange = CollectionChange.ItemChanged;
                        this.Index = (uint) Math.Max( args.NewStartingIndex, 0 );
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.CollectionChange = CollectionChange.Reset;
                        break;
                }
            }

            public CollectionChange CollectionChange
            {
                get;
                private set;
            }

            public uint Index
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Represents a dummy collection for CollectionGroups that only throws an exception when an attempt is made to add
        /// a group description (as opposed to when the GroupDescriptions property is accessed). All other operations are
        /// either safe as they are or will throw an appropriate exception.
        /// </summary>
        private sealed class GroupCollection : ReadOnlyObservableCollection<object>, IObservableVector<object>
        {
            internal GroupCollection()
                : base( new ObservableCollection<object>() )
            {
            }

            protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
            {
                base.OnCollectionChanged( e );

                var handler = this.VectorChanged;

                if ( handler != null )
                    handler( this, new VectorChangedEventArgs( e ) );
            }

            public event VectorChangedEventHandler<object> VectorChanged;
        }

        private const int CanChangePageFlag = 0;
        private const int IsPageChangingFlag = 1;
        private readonly IEqualityComparer<T> comparer;
        private readonly BitArray flags = new BitArray( 2, false );
        private readonly FrozenItemCollection<T> items;
        private readonly Lazy<GroupCollection> groups = new Lazy<GroupCollection>( () => new GroupCollection() );
        private readonly Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod;
        private readonly EventRegistrationTokenTable<VectorChangedEventHandler<object>> vectorChanged = new EventRegistrationTokenTable<VectorChangedEventHandler<object>>();
        private T currentItem;
        private int deferLevel;
        private int deferredPageIndex = -1;
        private int currentPosition = -1;
        private int currentPageIndex = -1;
        private int pageSize = 10;
        private int itemCount = 0;
        private int totalCount = -1;

        /// <summary>
        /// Finalizes an instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        ~VirtualCollectionView()
        {
            if ( this.items != null )
                this.items.FrozenItems.CollectionChanged -= this.OnFrozenItemsChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        /// <param name="pagingMethod">The <see cref="Func{T1,TResult}">function</see> used to retrieve a data page.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public VirtualCollectionView( Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod )
            : this( pagingMethod, EqualityComparer<T>.Default )
        {
            Contract.Requires<ArgumentNullException>( pagingMethod != null, "pagingMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        /// <param name="pagingMethod">The <see cref="Func{T1,TResult}">function</see> used to retrieve a data page.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items in the collection.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public VirtualCollectionView( Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod, IEqualityComparer<T> comparer )
            : base( new FrozenItemCollection<T>() )
        {
            Contract.Requires<ArgumentNullException>( pagingMethod != null, "pagingMethod" );
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );

            this.items = (FrozenItemCollection<T>) this.Items;
            this.items.FrozenItems.CollectionChanged += this.OnFrozenItemsChanged;
            this.pagingMethod = pagingMethod;
            this.comparer = comparer;
        }

        private bool IsLoadDeferred
        {
            get
            {
                var value = this.deferLevel > 0;
                return value;
            }
        }

        /// <summary>
        /// Gets the comparer used to evaluate the equality of items in the collection.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}"/> object.  The default state is <see cref="P:EqualityComparer{T}.Default"/>.</value>
        protected virtual IEqualityComparer<T> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null );
                return this.comparer;
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
                var value = this.TotalItemCount > -1;
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
                var value = (int) Math.Ceiling( (double) this.ItemCount / (double) this.PageSize );
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
                var value = this.PageCount - 1;
                return value;
            }
        }

        /// <summary>
        /// Gets the current item in the collection view.
        /// </summary>
        /// <value>The current item of type <typeparamref name="T"/> in the collection.</value>
        public T CurrentItem
        {
            get
            {
                return this.currentItem;
            }
            private set
            {
                this.currentItem = value;
                this.OnCurrentChanged( EventArgs.Empty );
                this.OnPropertyChanged( "CurrentItem" );
            }
        }

        /// <summary>
        /// Gets the sequence of items in the source collection.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        public IEnumerable<T> SourceCollection
        {
            get
            {
                return this.Items;
            }
        }

        /// <summary>
        /// Gets or sets the position of frozen items in the collection view.
        /// </summary>
        /// <value>One of the <see cref="FrozenItemPosition"/> values.</value>
        public FrozenItemPosition FrozenItemPosition
        {
            get
            {
                return this.items.FrozenItemPosition;
            }
            set
            {
                if ( this.items.FrozenItemPosition == value )
                    return;

                this.items.FrozenItemPosition = value;
                this.OnPropertyChanged( "FrozenItemPosition" );
            }
        }

        IEnumerable IFrozenItemCollectionView.FrozenItems
        {
            get
            {
                return this.FrozenItems;
            }
        }

        IEnumerable IFrozenItemCollectionView.UnfrozenItems
        {
            get
            {
                return this.UnfrozenItems;
            }
        }

        /// <summary>
        /// Gets an observable collection of frozen items.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}">observable collection</see> of frozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        public ObservableCollection<T> FrozenItems
        {
            get
            {
                return this.items.FrozenItems;
            }
        }

        /// <summary>
        /// Gets a read-only, observable collection of unfrozen items.
        /// </summary>
        /// <value>A read-only, <see cref="IList{T}">observable collection</see> of unfrozen
        /// <typeparamref name="T">items</typeparamref>.</value>
        /// <remarks>The default <see cref="IList{T}">list</see> implementation also implements
        /// <see cref="INotifyCollectionChanged"/> and <see cref="INotifyPropertyChanged"/>.</remarks>
        public IList<T> UnfrozenItems
        {
            get
            {
                return this.items.UnfrozenItems;
            }
        }

        /// <summary>
        /// Gets any collection groups that are associated with the view.
        /// </summary>
        /// <value>A vector collection of possible views.</value>
        [CLSCompliant( false )]
        public virtual IObservableVector<object> CollectionGroups
        {
            get
            {
                return this.groups.Value;
            }
        }

        /// <summary>
        /// Occurs when the current position in the collection has changed.
        /// </summary>
        public event EventHandler<object> CurrentChanged;

        /// <summary>
        /// Occurs when the current position in the collection is about to change.
        /// </summary>
        [CLSCompliant( false )]
        public event CurrentChangingEventHandler CurrentChanging;

        object ICollectionView.CurrentItem
        {
            get
            {
                return this.CurrentItem;
            }
        }

        /// <summary>
        /// Gets the current position in the collection.
        /// </summary>
        /// <value>The zero-based index of the current position in the collection.</value>
        public int CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            private set
            {
                if ( this.currentPosition == value )
                    return;

                this.currentPosition = value;
                this.OnPropertyChanged( "CurrentPosition" );
                this.OnPropertyChanged( "IsCurrentBeforeFirst" );
                this.OnPropertyChanged( "IsCurrentAfterLast" );
            }
        }

        /// <summary>
        /// Returns an object that can be used to defer loaded operations to the collection.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object.</returns>
        public virtual IDisposable DeferLoad()
        {
            return new DeferManager( this );
        }

        /// <summary>
        /// Gets a value indicating whether the view has more, unloaded items.
        /// </summary>
        /// <value>True if additional unloaded items remain in the view; otherwise, false.</value>
        public virtual bool HasMoreItems
        {
            get
            {
                return this.HasBeenLoaded && !this.IsEmpty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current position in the collection is after the last item.
        /// </summary>
        /// <value>True if the current position in the collection is after the last item; otherwise, false.</value>
        public virtual bool IsCurrentAfterLast
        {
            get
            {
                return this.CurrentPosition >= this.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current position in the collection is before the first item.
        /// </summary>
        /// <value>True if the current position in the collection is before the first item; otherwise, false.</value>
        public virtual bool IsCurrentBeforeFirst
        {
            get
            {
                return this.CurrentPosition < 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is empty.
        /// </summary>
        /// <value>True if the collection is empty; otherwise, false.</value>
        public virtual bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the <see cref="LoadMoreItemsResult">results</see>.</returns>
        /// <remarks>The number of items loaded is based on the <see cref="P:PageSize">page size</see>.</remarks>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync()
        {
            return this.LoadMoreItemsAsync( (uint) this.PageSize );
        }

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">The number of items to load.</param>
        /// <returns>An <see cref="IAsyncOperation{T}">asynchronous operation</see> containing the <see cref="LoadMoreItemsResult">results</see>.</returns>
        [CLSCompliant( false )]
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync( uint count )
        {
            if ( count > int.MaxValue )
                throw new ArgumentOutOfRangeException( "count" );

            // update the page size to reflect how many items should be received
            this.PageSize = (int) count;

            // loading is deferred, there is nothing to do
            if ( this.IsLoadDeferred )
                return Task.FromResult( new LoadMoreItemsResult() ).AsAsyncOperation();

            var pageIndex = this.PageIndex;

            // is this a callback from a deferred load?
            if ( this.deferredPageIndex > -1 )
            {
                // load the last deferred page index instead of the current page index
                pageIndex = this.deferredPageIndex;
                this.deferredPageIndex = -1;
            }

            // if the page index is unset, trigger moving to the first page and
            // return the current count
            if ( pageIndex < 0 )
            {
                this.MoveToFirstPageAsync();
                var result = new LoadMoreItemsResult() {
                    Count = (uint) this.Items.Count
                };
                return Task.FromResult( result ).AsAsyncOperation();
            }

            return this.LoadPageAsync( pageIndex ).AsAsyncOperation();

        }

        private async Task<LoadMoreItemsResult> LoadPageAsync( int pageIndex )
        {
            var result = new LoadMoreItemsResult();

            // make sure the page is actually changing
            if ( this.PageIndex != pageIndex )
            {
                var args = new PageChangingEventArgs( pageIndex );
                this.IsPageChanging = true;
                this.OnPageChanging( args );

                // allow the operation to be canceled
                if ( args.Cancel )
                {
                    this.IsPageChanging = false;
                    result.Count = (uint) this.Items.Count;
                    return result;
                }
            }

            // if a load is queued while we're deferring, keep track of the last index.
            // this could triggered by actions other than the LoadMoreItemsAsync method (ex: grouping)
            if ( this.IsLoadDeferred )
            {
                this.deferredPageIndex = pageIndex;
                result.Count = (uint) this.Items.Count;
                return result;
            }

            // subscribe to paged operation
            var actualPageSize = Math.Max( this.PageSize - this.FrozenItems.Count, 1 );
            var arguments = new PagingArguments( pageIndex, actualPageSize );
            var pagedItems = await this.pagingMethod( arguments );

            result.Count = this.OnLoaded( pagedItems );
            this.OnLoadComplete( pageIndex );

            return result;
        }

        private uint OnLoaded( PagedCollection<T> pagedItems )
        {
            Contract.Requires( pagedItems != null, "pagedItems" );

            // replace all items from the provided collection
            this.Items.ReplaceAll( pagedItems );

            // update collection view
            this.ItemCount = (int) pagedItems.TotalCount;
            this.TotalItemCount = this.ItemCount + this.FrozenItems.Count;
            this.MoveCurrentToPosition( -1, false );

            return (uint) this.Items.Count;
        }

        private void OnLoadComplete( int pageIndex )
        {
            if ( !this.IsPageChanging )
                return;

            // update page index when the operation is complete
            this.IsPageChanging = false;
            this.PageIndex = pageIndex;
            this.OnPageChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs args )
        {
            base.OnCollectionChanged( args );

            var handler = this.vectorChanged.InvocationList;

            if ( handler != null )
                handler( this, new VectorChangedEventArgs( args ) );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected override void OnPropertyChanged( PropertyChangedEventArgs args )
        {
            // LEGACY: the base class doesn't have a code contract
            if ( args == null )
                throw new ArgumentNullException( "args" );

            base.OnPropertyChanged( args );

            switch ( args.PropertyName )
            {
                case "Count":
                    {
                        this.OnPropertyChanged( "IsEmpty" );
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
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PageChanging;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="PageChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnPageChanged( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PageChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanging"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CurrentChangingEventArgs"/> event data.</param>
        [CLSCompliant( false )]
        protected virtual void OnCurrentChanging( CurrentChangingEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CurrentChanging;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCurrentChanged( EventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CurrentChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Moves the current position in the collection to the item at the specified index.
        /// </summary>
        /// <param name="position">The zero-based index in the collection set as the current position.</param>
        /// <param name="cancelable">Indicates whether the operation can be canceled.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        protected virtual bool MoveCurrentToPosition( int position, bool cancelable )
        {
            // succeeded, but we're already on this index
            if ( this.CurrentPosition == position && !this.IsPageChanging )
                return false;

            var args = new CurrentChangingEventArgs( cancelable );
            this.OnCurrentChanging( args );

            if ( args.IsCancelable && args.Cancel )
                return false;

            // out of range
            if ( position < 0 || position >= this.Count )
            {
                this.CurrentPosition = position < 0 ? -1 : this.Count;
                this.CurrentItem = default( T );
                return false;
            }

            // update current item
            this.CurrentItem = this.Items[position];
            this.CurrentPosition = position;
            return true;
        }

        bool ICollectionView.MoveCurrentTo( object item )
        {
            return this.MoveCurrentTo( (T) item );
        }

        /// <summary>
        /// Moves the current position in the collection to the specified item.
        /// </summary>
        /// <param name="item">The item to move to.</param>
        /// <returns>True if the current position moves to the specified item; otherwise, false.</returns>
        public bool MoveCurrentTo( T item )
        {
            // not in collection
            if ( !this.Contains( item ) )
                return false;

            // succeeded, but this is already the current item
            if ( this.Comparer.Equals( this.CurrentItem, item ) )
                return false;

            var args = new CurrentChangingEventArgs( true );
            this.OnCurrentChanging( args );

            if ( args.Cancel )
                return false;

            var position = 0;

            // update current position
            using ( var iterator = this.Items.GetEnumerator() )
            {
                while ( iterator.MoveNext() )
                {
                    if ( this.Comparer.Equals( iterator.Current, item ) )
                    {
                        this.CurrentPosition = position;
                        break;
                    }

                    ++position;
                }
            }

            this.CurrentItem = item;
            return true;
        }

        /// <summary>
        /// Moves the current position in the collection to the first item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToFirst()
        {
            return this.MoveCurrentToPosition( 0 );
        }

        /// <summary>
        /// Moves the current position in the collection to the last item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToLast()
        {
            return this.MoveCurrentToPosition( this.Count - 1 );
        }

        /// <summary>
        /// Moves the current position in the collection to the next item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToNext()
        {
            return this.MoveCurrentToPosition( this.CurrentPosition + 1 );
        }

        /// <summary>
        /// Moves the current position in the collection to the item at the specified index.
        /// </summary>
        /// <param name="position">The zero-based index in the collection set as the current position.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPosition( int position )
        {
            return this.MoveCurrentToPosition( position, true );
        }

        /// <summary>
        /// Moves the current position in the collection to the previous item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPrevious()
        {
            return this.MoveCurrentToPosition( this.CurrentPosition - 1 );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection can change pages.
        /// </summary>
        /// <value>True if the collection can change pages; otherwise, false.</value>
        public bool CanChangePage
        {
            get
            {
                return this.flags[CanChangePageFlag];
            }
            protected set
            {
                if ( this.flags[CanChangePageFlag] == value )
                    return;

                this.flags[CanChangePageFlag] = value;
                this.OnPropertyChanged( "CanChangePage" );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection is changing pages.
        /// </summary>
        /// <value>True if the collection is changing pages; otherwise, false.</value>
        public bool IsPageChanging
        {
            get
            {
                return this.flags[IsPageChangingFlag];
            }
            protected set
            {
                if ( this.flags[IsPageChangingFlag] == value )
                    return;

                this.flags[IsPageChangingFlag] = value;
                this.OnPropertyChanged( "IsPageChanging" );
            }
        }

        /// <summary>
        /// Gets the total number of items in the collection before paging is applied.
        /// </summary>
        /// <value>The total number of item in the collection before paging is applied.</value>
        public int ItemCount
        {
            get
            {
                return this.itemCount;
            }
            private set
            {
                if ( this.itemCount == value )
                    return;

                this.itemCount = value;
                this.OnPropertyChanged( "ItemCount" );
                this.OnPropertyChanged( "PageCount" );
                this.CanChangePage = this.PageCount > 1;
            }
        }

        /// <summary>
        /// Moves the collection to the first data page asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToFirstPageAsync()
        {
            return this.MoveToPageAsync( 0 );
        }

        /// <summary>
        /// Moves the collection to the last data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToLastPageAsync()
        {
            if ( this.Count == 0 )
                return Task.FromResult( false );

            return this.MoveToPageAsync( this.LastPageIndex );
        }

        /// <summary>
        /// Moves the collection to the next data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToNextPageAsync()
        {
            return this.MoveToPageAsync( this.PageIndex + 1 );
        }

        /// <summary>
        /// Moves the collection to the data page at the specified page index.
        /// </summary>
        /// <param name="pageIndex">The zero-based page index to move to.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public virtual async Task<bool> MoveToPageAsync( int pageIndex )
        {
            // page index is out of range (note: TotalItemCount = -1 until the collection is loaded at least once)
            if ( pageIndex < 0 || ( this.HasBeenLoaded && pageIndex > this.LastPageIndex ) )
                return false;

            // note: load even if this is the same page index because external conditions may have changed
            await this.LoadPageAsync( pageIndex );
            return true;
        }

        /// <summary>
        /// Moves the collection to the previous data page.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing true if the operation succeeds; otherwise, false.</returns>
        public Task<bool> MoveToPreviousPage()
        {
            return this.MoveToPageAsync( this.PageIndex - 1 );
        }

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
            get
            {
                return this.currentPageIndex;
            }
            private set
            {
                if ( this.currentPageIndex == value )
                    return;

                this.currentPageIndex = value;
                this.OnPropertyChanged( "PageIndex" );
                this.CanChangePage = this.PageCount > 1;
            }
        }

        /// <summary>
        /// Gets or sets the size of a data page.
        /// </summary>
        /// <value>The size (number of items) of a data page.</value>
        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                // LEGACY: IPagedCollectionView doesn't have a code contract
                if ( value < 1 )
                    throw new ArgumentOutOfRangeException( "value" );

                if ( this.pageSize == value )
                    return;

                this.pageSize = value;
                this.OnPropertyChanged( "PageSize" );

                if ( !this.HasBeenLoaded )
                    return;

                this.OnPropertyChanged( "PageCount" );
                this.CanChangePage = this.PageCount > 1;

                if ( this.PageIndex > this.LastPageIndex )
                {
                    var succeeded = this.MoveToLastPageAsync();
                }
                else
                {
                    var result = this.LoadPageAsync( this.PageIndex );
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
            get
            {
                return this.totalCount;
            }
            private set
            {
                if ( this.totalCount == value )
                    return;

                this.totalCount = value;
                this.OnPropertyChanged( "TotalItemCount" );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        void IDeferrable.BeginDefer()
        {
            Interlocked.Increment( ref this.deferLevel );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        async void IDeferrable.EndDefer()
        {
            if ( Interlocked.Decrement( ref this.deferLevel ) == 0 )
                await this.LoadMoreItemsAsync();
        }

        private async void OnFrozenItemsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null, "e" );

            // NOTE: NotifyCollectionChangedAction.Replace does not trigger any changes
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if ( e.NewItems == null || e.NewItems.Count == 0 || this.Items.Count <= this.PageSize )
                            return;

                        // if we're not at the end, then we need to load the page
                        if ( this.PageIndex != this.LastPageIndex )
                            await this.LoadPageAsync( this.PageIndex );

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if ( e.OldItems == null || e.OldItems.Count == 0 )
                            return;

                        // if we're not at the end, then we need to load the page
                        if ( this.Items.Count < this.PageSize && this.PageIndex != this.LastPageIndex )
                            await this.LoadPageAsync( this.PageIndex );

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        // if we're not at the end, then we need to load the page
                        if ( this.Items.Count < this.PageSize && this.PageIndex != this.LastPageIndex )
                            await this.LoadPageAsync( this.PageIndex );

                        break;
                    }
            }
        }

        event VectorChangedEventHandler<object> IObservableVector<object>.VectorChanged
        {
            add
            {
                return vectorChanged.AddEventHandler( value );
            }
            remove
            {
                vectorChanged.RemoveEventHandler( value );
            }
        }

        int IList<object>.IndexOf( object item )
        {
            return ( (IList<T>) this ).IndexOf( (T) item );
        }

        void IList<object>.Insert( int index, object item )
        {
            ( (IList<T>) this ).Insert( index, (T) item );
        }

        object IList<object>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                ( (IList<T>) this )[index] = (T) value;
            }
        }

        void ICollection<object>.Add( object item )
        {
            ( (IList<T>) this ).Add( (T) item );
        }

        bool ICollection<object>.Contains( object item )
        {
            return this.Contains( (T) item );
        }

        void ICollection<object>.CopyTo( object[] array, int arrayIndex )
        {
            if ( array == null )
                throw new ArgumentNullException( "array" );

            var temp = new T[array.Length];
            this.CopyTo( temp, arrayIndex );
            temp.Cast<object>().ToArray().CopyTo( array, arrayIndex );
        }

        bool ICollection<object>.Remove( object item )
        {
            return ( (IList<T>) this ).Remove( (T) item );
        }

        void IList<object>.RemoveAt( int index )
        {
            ( (IList<T>) this ).RemoveAt( index );
        }

        void ICollection<object>.Clear()
        {
            ( (IList<T>) this ).Clear();
        }

        bool ICollection<object>.IsReadOnly
        {
            get
            {
                return ( (IList<T>) this ).IsReadOnly;
            }
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach ( object item in this )
                yield return item;
        }
    }
}
