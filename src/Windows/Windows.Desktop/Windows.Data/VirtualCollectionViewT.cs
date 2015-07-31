namespace More.Windows.Data
{
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
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a virtualized collection view.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item in the virtualized collection.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is the standard naming convention for a view of a collection." )]
    [SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The disposable pattern is used to manage event handler management. There are no resources to actually dispose of." )]
    public class VirtualCollectionView<T> : ReadOnlyObservableCollection<T>, IEnumerable<T>, IFrozenItemCollectionView, IDeferrable
    {
        /// <summary>
        /// Represents a dummy collection for GroupDescription that only throws an exception when an attempt is made to add
        /// a group description (as opposed to when the GroupDescriptions property is accessed). All other operations are
        /// either safe as they are or will throw an appropriate exception.
        /// </summary>
        private sealed class GroupDescriptionCollection : ObservableCollection<GroupDescription>
        {
            protected override void InsertItem( int index, GroupDescription item )
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Provides a disposable subscription to refreshed items provided in a refresh operation.
        /// </summary>
        private sealed class PagingSubscription : IDisposable
        {
            private VirtualCollectionView<T> @this;
            private PagedCollection<T> source;
            private volatile bool disposed;

            internal PagingSubscription( VirtualCollectionView<T> @this, PagedCollection<T> source )
            {
                Contract.Requires( @this != null );
                Contract.Requires( source != null );

                this.@this = @this;
                this.source = source;
                ( (INotifyCollectionChanged) this.source ).CollectionChanged += OnCollectionChanged;
            }

            private void Dispose( bool disposing )
            {
                if ( disposed )
                    return;

                disposed = true;

                if ( !disposing )
                    return;

                if ( source != null )
                {
                    ( (INotifyCollectionChanged) source ).CollectionChanged -= OnCollectionChanged;
                    source = null;
                }

                @this = null;
            }

            private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
            {
                if ( e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null )
                    @this.Items.AddRange( e.NewItems.OfType<T>() );
            }

            public void Dispose()
            {
                Dispose( true );
                GC.SuppressFinalize( this );
            }
        }

        private const int CanFilterFlag = 0;
        private const int CanGroupFlag = 1;
        private const int CanSortFlag = 2;
        private const int CanChangePageFlag = 3;
        private const int IsPageChangingFlag = 4;
        private readonly IEqualityComparer<T> comparer;
        private readonly BitArray flags = new BitArray( 5, false );
        private readonly FrozenItemCollection<T> items;
        private readonly SortDescriptionCollection sortDescriptions = new SortDescriptionCollection();
        private readonly Lazy<GroupDescriptionCollection> groupDescriptions = new Lazy<GroupDescriptionCollection>( () => new GroupDescriptionCollection() );
        private readonly Lazy<ReadOnlyObservableCollection<object>> groups = new Lazy<ReadOnlyObservableCollection<object>>( () => new ReadOnlyObservableCollection<object>( new ObservableCollection<object>() ) );
        private readonly Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod;
        private T currentItem;
        private Predicate<T> filter;
        private Predicate<object> predicate;
        private int deferLevel;
        private int deferredPageIndex = -1;
        private int currentPosition = -1;
        private int currentPageIndex = -1;
        private int pageSize = 10;
        private int itemCount = 0;
        private int totalCount = -1;
        private CultureInfo culture = CultureInfo.CurrentCulture;
        private IDisposable pagedItemSubscription;

        /// <summary>
        /// Finalizes an instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        ~VirtualCollectionView()
        {
            if ( items != null )
                items.FrozenItems.CollectionChanged -= OnFrozenItemsChanged;

            if ( sortDescriptions != null )
                ( (INotifyCollectionChanged) sortDescriptions ).CollectionChanged += OnSortDescriptionsChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionView{T}"/> class.
        /// </summary>
        /// <param name="pagingMethod">The <see cref="Func{T1,TResult}">function</see> used to retrieve a data page.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public VirtualCollectionView( Func<PagingArguments, Task<PagedCollection<T>>> pagingMethod )
            : this( pagingMethod, EqualityComparer<T>.Default )
        {
            Arg.NotNull( pagingMethod, nameof( pagingMethod ) );
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
            Arg.NotNull( pagingMethod, nameof( pagingMethod ) );
            Arg.NotNull( comparer, nameof( comparer ) );

            items = (FrozenItemCollection<T>) Items;
            items.FrozenItems.CollectionChanged += OnFrozenItemsChanged;
            flags[CanSortFlag] = true;
            this.pagingMethod = pagingMethod;
            this.comparer = comparer;

            ( (INotifyCollectionChanged) sortDescriptions ).CollectionChanged += OnSortDescriptionsChanged;
        }

        private bool IsRefreshDeferred
        {
            get
            {
                var value = deferLevel > 0;
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
                return comparer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection has refreshed at least once.
        /// </summary>
        /// <value>True if the collection has refreshed; otherwise, false.</value>
        protected virtual bool HasRefreshed
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
            get
            {
                return currentItem;
            }
            private set
            {
                currentItem = value;
                OnCurrentChanged( EventArgs.Empty );
                OnPropertyChanged( "CurrentItem" );
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
                return Items;
            }
        }

        /// <summary>
        /// Gets or sets the filter applied to the collection.
        /// </summary>
        /// <value>A <see cref="Predicate{T}"/> delegate.</value>
        /// <remarks>This property is not used by the default implementation.</remarks>
        public Predicate<T> Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if ( filter == value )
                    return;

                filter = value;
                predicate = value == null ? (Predicate<object>) null : o => filter( (T) o );
                OnPropertyChanged( "Filter" );
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
                return items.FrozenItems;
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
                return items.UnfrozenItems;
            }
        }

        private async void RefreshPageIndex( int pageIndex )
        {
            if ( PageIndex != pageIndex )
            {
                var args = new PageChangingEventArgs( pageIndex );
                IsPageChanging = true;
                OnPageChanging( args );

                if ( args.Cancel )
                {
                    IsPageChanging = false;
                    return;
                }
            }

            // if a refresh is queued while we're deferring, keep track of the last index.
            // this could triggered by actions other than the Refresh() method (ex: sorting)
            if ( IsRefreshDeferred )
            {
                deferredPageIndex = pageIndex;
                return;
            }

            // subscribe to paged operation
            var actualPageSize = Math.Max( PageSize - FrozenItems.Count, 1 );
            var arguments = new PagingArguments( pageIndex, actualPageSize, SortDescriptions );
            var pagedItems = await pagingMethod( arguments );

            OnRefreshed( pagedItems );
            OnRefreshComplete( pageIndex );
        }

        private void OnRefreshed( PagedCollection<T> pagedItems )
        {
            Contract.Requires( pagedItems != null, "pagedItems" );

            // terminate previous subscription
            if ( pagedItemSubscription != null )
                pagedItemSubscription.Dispose();

            // replace all items from the provided collection
            Items.ReplaceAll( pagedItems );

            // items may be provided all at once or provided asynchronously; subscribe to any additions
            pagedItemSubscription = new PagingSubscription( this, pagedItems );

            // update collection view
            ItemCount = (int) pagedItems.TotalCount;
            TotalItemCount = ItemCount + FrozenItems.Count;
            MoveCurrentToPosition( -1, false );
        }

        private void OnRefreshComplete( int pageIndex )
        {
            if ( !IsPageChanging )
                return;

            // update page index when the operation is complete
            IsPageChanging = false;
            PageIndex = pageIndex;
            OnPageChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged( string propertyName )
        {
            OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
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
        protected virtual void OnCurrentChanging( CurrentChangingEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            CurrentChanging?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CurrentChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
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
            // succeeded, but we're already on this index
            if ( CurrentPosition == position && !IsPageChanging )
                return false;

            var args = new CurrentChangingEventArgs( cancelable );
            OnCurrentChanging( args );

            if ( args.IsCancelable && args.Cancel )
                return false;

            // out of range
            if ( position < 0 || position >= Count )
            {
                CurrentPosition = position < 0 ? -1 : Count;
                CurrentItem = default( T );
                return false;
            }

            // update current item
            CurrentItem = Items[position];
            CurrentPosition = position;
            return true;
        }

        /// <summary>
        /// Moves the current position in the collection to the specified item.
        /// </summary>
        /// <param name="item">The item to move to.</param>
        /// <returns>True if the current position moves to the specified item; otherwise, false.</returns>
        public bool MoveCurrentTo( T item )
        {
            // not in collection
            if ( !Contains( item ) )
                return false;

            // succeeded, but this is already the current item
            if ( Comparer.Equals( CurrentItem, item ) )
                return false;

            var args = new CurrentChangingEventArgs( true );
            OnCurrentChanging( args );

            if ( args.Cancel )
                return false;

            var position = 0;

            // update current position
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

        private void OnFrozenItemsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( e != null, "e" );

            // NOTE: NotifyCollectionChangedAction.Replace does not trigger any changes
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if ( e.NewItems == null || e.NewItems.Count == 0 || Items.Count <= PageSize )
                            return;

                        // if we're not at the end, then we need to refresh the page
                        if ( PageIndex != LastPageIndex )
                            RefreshPageIndex( PageIndex );

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if ( e.OldItems == null || e.OldItems.Count == 0 )
                            return;

                        // if we're not at the end, then we need to refresh the page
                        if ( Items.Count < PageSize && PageIndex != LastPageIndex )
                            RefreshPageIndex( PageIndex );

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        // if we're not at the end, then we need to refresh the page
                        if ( Items.Count < PageSize && PageIndex != LastPageIndex )
                            RefreshPageIndex( PageIndex );

                        break;
                    }
            }
        }

        private void OnSortDescriptionsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        if ( HasRefreshed )
                            MoveToFirstPage();

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if ( SortDescriptions.Count > 0 && HasRefreshed )
                            MoveToFirstPage();

                        break;
                    }
            }
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
                        OnPropertyChanged( "IsEmpty" );
                        break;
                    }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="VirtualCollectionView{T}">collection</see>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object.</returns>
        public new virtual IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection can be filtered.
        /// </summary>
        /// <value>True if the collection can be filtered; otherwise false.  The default implementation always returns false.</value>
        public bool CanFilter
        {
            get
            {
                return flags[CanFilterFlag];
            }
            protected set
            {
                if ( flags[CanFilterFlag] == value )
                    return;

                flags[CanFilterFlag] = value;
                OnPropertyChanged( "CanFilter" );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether items in the collection can be grouped.
        /// </summary>
        /// <value>True if the items in the collection can be grouped; otherwise false.  The default implementation always returns false.</value>
        public bool CanGroup
        {
            get
            {
                return flags[CanGroupFlag];
            }
            protected set
            {
                if ( flags[CanGroupFlag] == value )
                    return;

                flags[CanGroupFlag] = value;
                OnPropertyChanged( "CanGroup" );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection can be sorted.
        /// </summary>
        /// <value>True if the collection can be sorted; otherwise false.  The default implementation always returns true.</value>
        public bool CanSort
        {
            get
            {
                return flags[CanSortFlag];
            }
            protected set
            {
                if ( flags[CanSortFlag] == value )
                    return;

                flags[CanSortFlag] = value;
                OnPropertyChanged( "CanSort" );
            }
        }

        bool ICollectionView.Contains( object item )
        {
            return Contains( (T) item );
        }

        /// <summary>
        /// Gets or sets the culture information used by the collection.
        /// </summary>
        /// <value>A <see cref="CultureInfo"/> object.</value>
        public virtual CultureInfo Culture
        {
            get
            {
                return culture;
            }
            set
            {
                // LEGACY: ICollectionView does not have a code contract
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                if ( object.Equals( culture, value ) )
                    return;

                culture = value;
                OnPropertyChanged( "Culture" );
            }
        }

        /// <summary>
        /// Occurs when the current position in the collection has changed.
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Occurs when the current position in the collection is about to change.
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        object ICollectionView.CurrentItem
        {
            get
            {
                return CurrentItem;
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
                return currentPosition;
            }
            private set
            {
                if ( currentPosition == value )
                    return;

                currentPosition = value;
                OnPropertyChanged( "CurrentPosition" );
                OnPropertyChanged( "IsCurrentBeforeFirst" );
                OnPropertyChanged( "IsCurrentAfterLast" );
            }
        }

        /// <summary>
        /// Returns an object that can be used to defer refresh operations to the collection.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object.</returns>
        public virtual IDisposable DeferRefresh()
        {
            return new DeferManager( this );
        }

        Predicate<object> ICollectionView.Filter
        {
            get
            {
                return predicate;
            }
            set
            {
                if ( predicate == value )
                    return;

                predicate = value;
                filter = value == null ? (Predicate<T>) null : o => predicate( (T) o );
                OnPropertyChanged( "Filter" );
            }
        }

        /// <summary>
        /// Gets a collection of group descriptions for the collection view.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}"/> object.</value>
        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get
            {
                return groupDescriptions.Value;
            }
        }

        /// <summary>
        /// Gets a collection of groups for the collection view.
        /// </summary>
        /// <value>A <see cref="ReadOnlyObservableCollection{T}"/> object.</value>
        public virtual ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return groups.Value;
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
                return CurrentPosition >= Count;
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
                return CurrentPosition < 0;
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
                return Count == 0;
            }
        }

        bool ICollectionView.MoveCurrentTo( object item )
        {
            return MoveCurrentTo( (T) item );
        }

        /// <summary>
        /// Moves the current position in the collection to the first item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToFirst()
        {
            return MoveCurrentToPosition( 0 );
        }

        /// <summary>
        /// Moves the current position in the collection to the last item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToLast()
        {
            return MoveCurrentToPosition( Count - 1 );
        }

        /// <summary>
        /// Moves the current position in the collection to the next item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToNext()
        {
            return MoveCurrentToPosition( CurrentPosition + 1 );
        }

        /// <summary>
        /// Moves the current position in the collection to the item at the specified index.
        /// </summary>
        /// <param name="position">The zero-based index in the collection set as the current position.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPosition( int position )
        {
            return MoveCurrentToPosition( position, true );
        }

        /// <summary>
        /// Moves the current position in the collection to the previous item.
        /// </summary>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool MoveCurrentToPrevious()
        {
            return MoveCurrentToPosition( CurrentPosition - 1 );
        }

        /// <summary>
        /// Refreshes the collection.
        /// </summary>
        public virtual void Refresh()
        {
            if ( IsRefreshDeferred )
                return;

            var pageIndex = PageIndex;

            // is this a callback from a deferred refresh?
            if ( deferredPageIndex > -1 )
            {
                // refresh the last deferred page index instead of the current page index
                pageIndex = deferredPageIndex;
                deferredPageIndex = -1;
            }

            if ( pageIndex < 0 )
                MoveToFirstPage();
            else
                RefreshPageIndex( pageIndex );
        }

        /// <summary>
        /// Gets a collection of sort descriptions for the collection.
        /// </summary>
        /// <value>A <see cref="SortDescriptionCollection"/> object.</value>
        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                return sortDescriptions;
            }
        }

        IEnumerable ICollectionView.SourceCollection
        {
            get
            {
                return SourceCollection;
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
                return items.FrozenItemPosition;
            }
            set
            {
                if ( items.FrozenItemPosition == value )
                    return;

                items.FrozenItemPosition = value;
                OnPropertyChanged( "FrozenItemPosition" );
            }
        }

        IEnumerable IFrozenItemCollectionView.FrozenItems
        {
            get
            {
                return FrozenItems;
            }
        }

        IEnumerable IFrozenItemCollectionView.UnfrozenItems
        {
            get
            {
                return UnfrozenItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection can change pages.
        /// </summary>
        /// <value>True if the collection can change pages; otherwise, false.</value>
        public bool CanChangePage
        {
            get
            {
                return flags[CanChangePageFlag];
            }
            protected set
            {
                if ( flags[CanChangePageFlag] == value )
                    return;

                flags[CanChangePageFlag] = value;
                OnPropertyChanged( "CanChangePage" );
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
                return flags[IsPageChangingFlag];
            }
            protected set
            {
                if ( flags[IsPageChangingFlag] == value )
                    return;

                flags[IsPageChangingFlag] = value;
                OnPropertyChanged( "IsPageChanging" );
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
                return itemCount;
            }
            private set
            {
                if ( itemCount == value )
                    return;

                itemCount = value;
                OnPropertyChanged( "ItemCount" );
                OnPropertyChanged( "PageCount" );
                CanChangePage = PageCount > 1;
            }
        }

        /// <summary>
        /// Moves the collection to the first data page.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool MoveToFirstPage()
        {
            return MoveToPage( 0 );
        }

        /// <summary>
        /// Moves the collection to the last data page.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool MoveToLastPage()
        {
            if ( Count == 0 )
                return false;

            return MoveToPage( LastPageIndex );
        }

        /// <summary>
        /// Moves the collection to the next data page.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool MoveToNextPage()
        {
            return MoveToPage( PageIndex + 1 );
        }

        /// <summary>
        /// Moves the collection to the data page at the specified page index.
        /// </summary>
        /// <param name="pageIndex">The zero-based page index to move to.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public virtual bool MoveToPage( int pageIndex )
        {
            // page index is out of range (note: TotalItemCount = -1 until the collection is refresh at least once)
            if ( pageIndex < 0 || ( HasRefreshed && pageIndex > LastPageIndex ) )
                return false;

            // note: refresh even if this is the same page index because external conditions may have changed
            RefreshPageIndex( pageIndex );
            return true;
        }

        /// <summary>
        /// Moves the collection to the previous data page.
        /// </summary>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public bool MoveToPreviousPage()
        {
            return MoveToPage( PageIndex - 1 );
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
                return currentPageIndex;
            }
            private set
            {
                if ( currentPageIndex == value )
                    return;

                currentPageIndex = value;
                OnPropertyChanged( "PageIndex" );
                CanChangePage = PageCount > 1;
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
                return pageSize;
            }
            set
            {
                // LEGACY: IPagedCollectionView doesn't have a code contract
                if ( value < 1 )
                    throw new ArgumentOutOfRangeException( "value" );

                if ( pageSize == value )
                    return;

                pageSize = value;
                OnPropertyChanged( "PageSize" );

                if ( !HasRefreshed )
                    return;

                OnPropertyChanged( "PageCount" );
                CanChangePage = PageCount > 1;

                if ( PageIndex > LastPageIndex )
                    MoveToLastPage();
                else
                    RefreshPageIndex( PageIndex );
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
                return totalCount;
            }
            private set
            {
                if ( totalCount == value )
                    return;

                totalCount = value;
                OnPropertyChanged( "TotalItemCount" );
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        void IDeferrable.BeginDefer()
        {
            Interlocked.Increment( ref deferLevel );
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is not meant to be publicly visible." )]
        void IDeferrable.EndDefer()
        {
            if ( Interlocked.Decrement( ref deferLevel ) == 0 )
                Refresh();
        }
    }
}
