namespace More.ComponentModel
{
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a collection of selectable items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to make selectable.</typeparam>
    /// <example>This example demonstrates to create a collection of library books and then check-out the selected books.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.ComponentModel;
    /// using System.Linq;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// 
    /// public class Book
    /// {
    ///     public string Isbn
    ///     {
    ///         get;
    ///         set;
    ///     }
    ///     
    ///     public string Name
    ///     {
    ///         get;
    ///         set;
    ///     }
    /// }
    /// 
    /// public class LibraryViewModel
    /// {
    ///     public SelectableItemCollection<Book> Books
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     public ICommand CheckOut
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     private void OnCheckOut( object parameter )
    ///     {
    ///         foreach ( var book in this.Books.SelectedValues )
    ///             ; // TODO: invoke service to check out book
    ///     }
    ///     
    ///     public LibraryViewModel( IEnumerable<Book> books )
    ///     {
    ///         this.Books = new SelectableItemCollection<Book>( books );
    ///         this.CheckOut = new Command<object>( this.OnCheckOut );
    ///     }
    /// }
    /// 
    /// public partial class MyUserControl : UserControl
    /// {
    ///     private readonly LibraryViewModel viewModel;
    ///     
    ///     public MyUserControl()
    ///     {
    ///         this.InitializeComponent();
    ///         var books = new Book[]
    ///             {
    ///                 new Book(){ Isbn = "12345", Name = "Object Thinking" },
    ///                 new Book(){ Isbn = "67890", Name = "Code Complete" },
    ///                 new Book(){ Isbn = "24680", Name = "Enterprise Design Patterns" }
    ///             };
    ///         this.viewModel = new LibraryViewModel( books );
    ///         this.DataContext = viewModel;
    ///     }
    /// }
    /// ]]></code></example>
    /// <example>This example expands upon the previous example and demonstrates how to also specify a item that will automatically
    /// select or unselect all items.  The 'All' item itself will never be included in the <see cref="P:SelectedItems"/> or
    /// <see cref="P:SelectedValues"/> properties.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.ComponentModel;
    /// using System.Linq;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// 
    /// public class Book
    /// {
    ///     public string Isbn
    ///     {
    ///         get;
    ///         set;
    ///     }
    ///     
    ///     public string Name
    ///     {
    ///         get;
    ///         set;
    ///     }
    /// }
    /// 
    /// public class LibraryViewModel
    /// {
    ///     public SelectableItemCollection<Book> Books
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     public ICommand CheckOut
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     private void OnCheckOut( object parameter )
    ///     {
    ///         foreach ( var book in this.Books.SelectedValues )
    ///             ; // TODO: invoke service to check out book
    ///     }
    ///     
    ///     public LibraryViewModel( IEnumerable<Book> books )
    ///     {
    ///         this.Books = new SelectableItemCollection<Book>( books, new Book(){ Name = "(Select All)" } );
    ///         this.CheckOut = new Command<object>( this.OnCheckOut );
    ///     }
    /// }
    /// 
    /// public partial class MyUserControl : UserControl
    /// {
    ///     private readonly LibraryViewModel viewModel;
    ///     
    ///     public MyUserControl()
    ///     {
    ///         this.InitializeComponent();
    ///         var books = new Book[]
    ///             {
    ///                 new Book(){ Isbn = "12345", Name = "Object Thinking" },
    ///                 new Book(){ Isbn = "67890", Name = "Code Complete" },
    ///                 new Book(){ Isbn = "24680", Name = "Enterprise Design Patterns" }
    ///             };
    ///         this.viewModel = new LibraryViewModel( books );
    ///         this.DataContext = viewModel;
    ///     }
    /// }
    /// ]]></code></example>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( SelectableItemCollectionDebugView<> ) )]
    public class SelectableItemCollection<T> : ReadOnlyObservableCollection<SelectableItem<T>>
    {
        /// <summary>
        /// Represents a mediator for coordinating element synchronization between the current selected items and a target collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item to mediate.</typeparam>
        private sealed class SelectedValuesMediator<TItem> : IDisposable
        {
            private readonly ObservableCollection<TItem> source;
            private readonly INotifyCollectionChanged target;
            private readonly ICollection<TItem> targetCollection;
            private bool disposed;

            ~SelectedValuesMediator()
            {
                this.Dispose( false );
            }

            internal SelectedValuesMediator( ObservableCollection<TItem> source, ICollection<TItem> targetCollection )
            {
                Contract.Requires( source != null );
                Contract.Requires( targetCollection != null );

                this.source = source;
                this.targetCollection = targetCollection;
                this.source.CollectionChanged += this.OnSourceChanged;
                this.target = targetCollection as INotifyCollectionChanged;

                if ( this.target != null )
                    this.target.CollectionChanged += this.OnTargetChanged;
            }

            [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing", Justification = "This is the standard implementation of the Dispose pattern." )]
            private void Dispose( bool disposing )
            {
                if ( this.disposed )
                    return;

                this.disposed = true;
                this.source.CollectionChanged -= this.OnSourceChanged;

                if ( this.target != null )
                    this.target.CollectionChanged -= this.OnTargetChanged;
            }

            private void OnSourceChanged( object sender, NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( sender != null );
                Contract.Requires( e != null );

                // disable events (to prevent cyclic recursion)
                if ( this.target != null )
                    this.target.CollectionChanged -= this.OnTargetChanged;

                switch ( e.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if ( e.NewItems != null )
                                this.targetCollection.AddRange( e.NewItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if ( e.OldItems != null )
                                this.targetCollection.RemoveRange( e.OldItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            this.targetCollection.Clear();
                            break;
                        }
                }

                // re-enable events
                if ( this.target != null )
                    this.target.CollectionChanged += this.OnTargetChanged;
            }

            private void OnTargetChanged( object sender, NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( sender != null );
                Contract.Requires( e != null );

                // disable events (to prevent cyclic recursion)
                this.source.CollectionChanged -= this.OnSourceChanged;

                switch ( e.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if ( e.NewItems != null )
                                this.source.AddRange( e.NewItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if ( e.OldItems != null )
                                this.source.RemoveRange( e.OldItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            this.source.Clear();
                            break;
                        }
                }

                // re-enable events
                this.source.CollectionChanged += this.OnSourceChanged;
            }

            public void Dispose()
            {
                this.Dispose( true );
                GC.SuppressFinalize( this );
            }
        }

        /// <summary>
        /// Represents a special <i>Select All</i> item.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of item value.</typeparam>
        private sealed class SelectAllItem<TValue> : SelectableItem<TValue>
        {
            private readonly SelectableItemCollection<TValue> owner;
            private bool suppressEvents;
            private bool processing;

            internal SelectAllItem( SelectableItemCollection<TValue> owner, TValue value )
                : base( false, value )
            {
                Contract.Requires( owner != null );
                this.owner = owner;
            }

            internal void SetSelectedWithoutEvents( bool? value )
            {
                this.suppressEvents = true;
                this.IsSelected = value;
                this.suppressEvents = false;
            }

            internal void UpdateSelectedWithoutEvents()
            {
                if ( this.processing )
                    return;

                var count = this.owner.Count - 1;

                if ( count == 0 )
                    return;

                // update state if there is more than just the 'All' item
                bool? state = null;
                var selectedCount = this.owner.SelectedItems.Count;

                // determine if all items are checked or unchecked
                if ( count == selectedCount )
                    state = true;
                else if ( selectedCount == 0 )
                    state = false;

                this.SetSelectedWithoutEvents( state );
            }

            [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Handled by base class" )]
            protected override void OnPropertyChanged( PropertyChangedEventArgs e )
            {
                Arg.NotNull( e, "e" );

                base.OnPropertyChanged( e );

                if ( this.suppressEvents ||
                    !string.Equals( e.PropertyName, "IsSelected", StringComparison.Ordinal ) ||
                    this.IsSelected == null )
                {
                    return;
                }

                var state = this.IsSelected;

                this.processing = true;

                foreach ( var item in this.owner.Where( i => !object.ReferenceEquals( this, i ) ) )
                    item.IsSelected = state;

                this.processing = false;
            }
        }

        /// <summary>
        /// Provides a wrapper collection around an sequence of elements.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of items to wrap.</typeparam>
        private sealed class ItemCollection<TItem> : ObservableCollection<SelectableItem<TItem>>
        {
            private readonly IEqualityComparer<TItem> valueComparer;
            private readonly IEqualityComparer<SelectableItem<TItem>> itemComparer;
            private SelectableItemCollection<TItem> owner;

            internal ItemCollection( IEqualityComparer<TItem> comparer )
            {
                Contract.Requires( comparer != null );

                // since we don't have a comparer that can be used here for the standard 0,1,-1 values use -1 as the non-equal value
                // we're only interested in equality so that should suffice for this internally implementation
                this.itemComparer = new DynamicComparer<SelectableItem<TItem>>( ( i1, i2 ) => comparer.Equals( i1.Value, i2.Value ) ? 0 : -1, i => comparer.GetHashCode( i.Value ) );
                this.valueComparer = comparer;
            }

            internal void Initialize( SelectableItemCollection<TItem> source, IEnumerable<TItem> sequence )
            {
                Contract.Requires( source != null );
                Contract.Requires( sequence != null );

                this.owner = source;

                var notifyCollection = sequence as INotifyCollectionChanged;

                foreach ( var item in sequence )
                {
                    var selectableItem = this.owner.CreateItem( item );
                    selectableItem.PropertyChanged += this.OnItemChanged;
                    this.Items.Add( selectableItem );
                }

                if ( notifyCollection != null )
                    notifyCollection.CollectionChanged += this.OnSourceCollectionChanged;
            }

            private void AddNewItems( IEnumerable<TItem> newItems )
            {
                var count = this.Count;

                foreach ( var newItem in newItems )
                {
                    var item = this.owner.CreateItem( newItem );
                    item.PropertyChanged += this.OnItemChanged;
                    this.Add( item );
                }

                // all items cannot be selected once a single item is not selected
                if ( this.Count != count && this.owner.selectAllItem != null && ( this.owner.selectAllItem.IsSelected ?? false ) )
                    this.owner.selectAllItem.SetSelectedWithoutEvents( null );
            }

            private void RemoveOldItems( IEnumerable<TItem> oldItems )
            {
                var count = this.Count;

                foreach ( var oldItem in oldItems )
                {
                    var removedItems = ( from i in this
                                         where this.valueComparer.Equals( i.Value, oldItem )
                                         select i ).ToArray();

                    foreach ( var removedItem in removedItems )
                    {
                        removedItem.PropertyChanged -= this.OnItemChanged;
                        this.Remove( removedItem );

                        if ( removedItem.IsSelected ?? false )
                        {
                            this.owner.SelectedValues.Remove( removedItem.Value );
                            this.owner.selectedItems.Remove( removedItem, this.itemComparer );
                        }
                    }
                }

                // removing an item might trigger a state change
                if ( this.Count != count && this.owner.selectAllItem != null )
                    this.owner.selectAllItem.UpdateSelectedWithoutEvents();
            }

            private void ReplaceItems( int startingIndex, IEnumerable<TItem> newValues )
            {
                var any = false;
                var index = startingIndex;

                foreach ( var newValue in newValues )
                {
                    any = true;
                    var oldItem = this[index];
                    var newItem = this.owner.CreateItem( newValue );

                    oldItem.PropertyChanged -= this.OnItemChanged;

                    if ( oldItem.IsSelected ?? false )
                    {
                        this.owner.SelectedValues.Remove( oldItem.Value );
                        this.owner.selectedItems.Remove( oldItem, this.itemComparer );
                    }

                    newItem.PropertyChanged += this.OnItemChanged;
                    this[index++] = newItem;
                }

                // replacing an item might trigger a state change
                if ( any && this.owner.selectAllItem != null )
                    this.owner.selectAllItem.UpdateSelectedWithoutEvents();
            }

            private void OnSourceCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
            {
                Contract.Requires( sender != null );
                Contract.Requires( e != null );

                switch ( e.Action )
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if ( e.NewItems != null )
                                this.AddNewItems( e.NewItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if ( e.OldItems != null )
                                this.RemoveOldItems( e.OldItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            if ( ( e.OldItems != null && e.OldItems.Count > 0 ) && ( e.NewItems != null && e.NewItems.Count > 0 ) )
                                this.ReplaceItems( e.NewStartingIndex, e.NewItems.OfType<TItem>() );

                            break;
                        }
                    case NotifyCollectionChangedAction.Reset:
                        {
                            this.Clear();
                            break;
                        }
                }
            }

            internal void OnItemChanged( object sender, PropertyChangedEventArgs e )
            {
                Contract.Requires( sender != null );
                Contract.Requires( e != null );

                var item = (SelectableItem<TItem>) sender;

                if ( item.IsSelected ?? false )
                {
                    // never add the 'All' item as a selection
                    if ( !object.ReferenceEquals( this.owner.selectAllItem, item ) )
                    {
                        // add selected value
                        if ( !this.owner.SelectedValues.Contains( item.Value, this.valueComparer ) )
                            this.owner.SelectedValues.Add( item.Value );

                        // add selected item
                        if ( !this.owner.selectedItems.Contains( item, this.itemComparer ) )
                            this.owner.selectedItems.Add( item );
                    }

                    // update the state of the 'All' item as necessary
                    if ( this.owner.selectAllItem != null )
                        this.owner.selectAllItem.UpdateSelectedWithoutEvents();
                }
                else
                {
                    // never add the 'All' item as a selection
                    if ( !object.ReferenceEquals( this.owner.selectAllItem, item ) )
                    {
                        // remove selected value and item
                        this.owner.SelectedValues.Remove( item.Value );
                        this.owner.selectedItems.Remove( item, this.itemComparer );
                    }

                    // update the state of the 'All' item as necessary
                    if ( this.owner.selectAllItem != null )
                        this.owner.selectAllItem.UpdateSelectedWithoutEvents();
                }
            }

            protected override void ClearItems()
            {
                foreach ( var item in this.owner )
                    item.PropertyChanged -= this.OnItemChanged;

                base.ClearItems();
                this.owner.SelectedValues.Clear();
                this.owner.selectedItems.Clear();

                if ( this.owner.selectAllItem == null )
                    return;

                this.owner.selectAllItem.SetSelectedWithoutEvents( false );
                this.owner.Items.Add( this.owner.selectAllItem );
            }

        }

        /// <summary>
        /// Represents a collection of selected values.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of selected values.</typeparam>
        private sealed class SelectedValuesCollection<TValue> : ObservableCollection<TValue>
        {
            private readonly IEqualityComparer<TValue> comparer;
            private readonly SelectableItemCollection<TValue> owner;

            internal SelectedValuesCollection( SelectableItemCollection<TValue> owner, IEqualityComparer<TValue> comparer )
            {
                Contract.Requires( owner != null );
                Contract.Requires( comparer != null );
                this.owner = owner;
                this.comparer = comparer;
            }

            private bool ItemExistsInSource( TValue value )
            {
                return this.FindAll( value ).Any();
            }

            private IEnumerable<SelectableItem<TValue>> FindAll( TValue value )
            {
                Contract.Ensures( Contract.Result<IEnumerable<SelectableItem<TValue>>>() != null );
                return this.owner.Where( item => this.comparer.Equals( item.Value, value ) );
            }

            protected override void ClearItems()
            {
                base.ClearItems();

                foreach ( var item in this.owner )
                    item.IsSelected = false;

                this.owner.OnPropertyChanged( new PropertyChangedEventArgs( "SelectedValues" ) );
            }

            protected override void InsertItem( int index, TValue item )
            {
                if ( !this.ItemExistsInSource( item ) )
                    throw new KeyNotFoundException( ExceptionMessage.ItemDoesNotExistInSource );

                base.InsertItem( index, item );

                foreach ( var selectableItem in this.FindAll( item ) )
                    selectableItem.IsSelected = true;

                this.owner.OnPropertyChanged( new PropertyChangedEventArgs( "SelectedValues" ) );
            }

            protected override void RemoveItem( int index )
            {
                var oldItem = this[index];
                base.RemoveItem( index );

                foreach ( var item in this.FindAll( oldItem ) )
                    item.IsSelected = false;

                this.owner.OnPropertyChanged( new PropertyChangedEventArgs( "SelectedValues" ) );
            }

            protected override void SetItem( int index, TValue item )
            {
                if ( !this.ItemExistsInSource( item ) )
                    throw new KeyNotFoundException( ExceptionMessage.ItemDoesNotExistInSource );

                var oldItem = this[index];

                foreach ( var selectableItem in this.FindAll( oldItem ) )
                    selectableItem.IsSelected = false;

                base.SetItem( index, item );

                foreach ( var selectableItem in this.FindAll( item ) )
                    selectableItem.IsSelected = true;

                this.owner.OnPropertyChanged( new PropertyChangedEventArgs( "SelectedValues" ) );
            }
        }

        private readonly IEqualityComparer<T> valueComparer;
        private readonly SelectAllItem<T> selectAllItem;
        private readonly SelectedValuesCollection<T> selectedValues;
        private readonly ObservableCollection<SelectableItem<T>> selectedItems;
        private readonly ReadOnlyObservableCollection<SelectableItem<T>> readOnlySelectedItems;
        private readonly Func<T, IEqualityComparer<T>, SelectableItem<T>> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        protected SelectableItemCollection()
            : this( ( v, c ) => new SelectableItem<T>( v, c ), new T[0], (object) null, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="Func{T1,T2,TResult}">function</see> representing the factory method to create new items.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        protected SelectableItemCollection( Func<T, IEqualityComparer<T>, SelectableItem<T>> factory )
            : this( factory, Enumerable.Empty<T>(), (object) null, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values in the specified sequence.</param>
        protected SelectableItemCollection( IEqualityComparer<T> comparer )
            : this( ( v, c ) => new SelectableItem<T>( v, c ), Enumerable.Empty<T>(), (object) null, comparer )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="Func{T1,T2,TResult}">function</see> representing the factory method to create new items.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values in the specified sequence.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for the factory method." )]
        protected SelectableItemCollection( Func<T, IEqualityComparer<T>, SelectableItem<T>> factory, IEqualityComparer<T> comparer )
            : this( factory, Enumerable.Empty<T>(), (object) null, comparer )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">An <see cref="IEnumerable{T}">sequence</see> to make selectable.</param>
        public SelectableItemCollection( IEnumerable<T> sequence )
            : this( ( v, c ) => new SelectableItem<T>( v, c ), sequence, (object) null, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">An <see cref="IEnumerable{T}">sequence</see> to make selectable.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values in the specified sequence.</param>
        public SelectableItemCollection( IEnumerable<T> sequence, IEqualityComparer<T> comparer )
            : this( ( v, c ) => new SelectableItem<T>( v, c ), sequence, (object) null, comparer )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">An <see cref="IEnumerable{T}">sequence</see> to make selectable.</param>
        /// <param name="selectAllValue">The object of type <typeparamref name="T"/> that represents the value of the special <i>Select All</i> item.</param>
        public SelectableItemCollection( IEnumerable<T> sequence, T selectAllValue )
            : this( ( v, c ) => new SelectableItem<T>( v, c ), sequence, (object) selectAllValue, EqualityComparer<T>.Default )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemCollection{T}"/> class.
        /// </summary>
        /// <param name="sequence">An <see cref="IEnumerable{T}">sequence</see> to make selectable.</param>
        /// <param name="selectAllValue">The object of type <typeparamref name="T"/> that represents the value of the special <i>Select All</i> item.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values in the specified sequence.</param>
        public SelectableItemCollection( IEnumerable<T> sequence, T selectAllValue, IEqualityComparer<T> comparer )
            : this( ( v, c ) => new SelectableItem<T>( v, c ), sequence, (object) selectAllValue, comparer )
        {
        }

        private SelectableItemCollection(
            Func<T, IEqualityComparer<T>, SelectableItem<T>> factory,
            IEnumerable<T> sequence,
            object selectAllValue,
            IEqualityComparer<T> comparer )
            : base( new ItemCollection<T>( comparer ) )
        {
            Arg.NotNull( factory, "factory" );
            Arg.NotNull( sequence, "sequence" );
            Arg.NotNull( comparer, "comparer" );

            this.factory = factory;
            this.valueComparer = comparer;
            ( (ItemCollection<T>) this.Items ).Initialize( this, sequence );
            this.selectedValues = new SelectedValuesCollection<T>( this, comparer );
            this.selectedItems = new ObservableCollection<SelectableItem<T>>();
            this.readOnlySelectedItems = new ReadOnlyObservableCollection<SelectableItem<T>>( this.selectedItems );

            if ( selectAllValue == null )
                return;

            this.selectAllItem = new SelectAllItem<T>( this, (T) selectAllValue );
            this.Items.Insert( 0, this.selectAllItem );
        }

        /// <summary>
        /// Gets the value comparer for the collection.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        protected virtual IEqualityComparer<T> ValueComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null ); 
                return this.valueComparer;
            }
        }

        /// <summary>
        /// Gets a collection of selected items.
        /// </summary>
        /// <value>An <see cref="ReadOnlyObservableCollection{T}"/> object.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public virtual ReadOnlyObservableCollection<SelectableItem<T>> SelectedItems
        {
            get
            {
                Contract.Ensures( Contract.Result<ReadOnlyObservableCollection<SelectableItem<T>>>() != null ); 
                return this.readOnlySelectedItems;
            }
        }

        /// <summary>
        /// Gets a collection of selected values.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}"/> object.</value>
        public virtual ObservableCollection<T> SelectedValues
        {
            get
            {
                Contract.Ensures( Contract.Result<ObservableCollection<T>>() != null ); 
                return this.selectedValues;
            }
        }

        /// <summary>
        /// Creates a mediator that synchronizes the elements in the <see cref="SelectedValues"/> collection and the specified target collection.
        /// </summary>
        /// <param name="target">The target <see cref="ICollection{T}"/> to mediate.</param>
        /// <returns>An <see cref="IDisposable"/> object.</returns>
        /// <remarks>If the collection specified by <paramref name="target"/> does not implement <see cref="INotifyCollectionChanged"/>, then
        /// the sychronization is one-way.  Invoking <see cref="IDisposable.Dispose"/> on the returned object terminates mediation.</remarks>
        /// <example>This example demonstrates to create a mediator that automatically synchronizes selected values to an external collection.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.Collections.Generic;
        /// using System.ComponentModel;
        /// using System.Linq;
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// using System.Windows.Data;
        /// 
        /// public class MyModel
        /// {
        ///     private readonly Collection<string> items = new Collection<string>();
        ///     private readonly Collection<string> selectedItems = new Collection<string>();
        ///     
        ///     public IList<string> Items
        ///     {
        ///         get
        ///         {
        ///             return this.items;
        ///         }
        ///     }
        ///     
        ///     public IList<string> SelectedItems
        ///     {
        ///         get
        ///         {
        ///             return this.selectedItems;
        ///         }
        ///     }
        /// }
        /// 
        /// public partial class MyUserControl : UserControl
        /// {
        ///     private readonly MyModel model = new MyModel();
        ///     
        ///     public MyUserControl()
        ///     {
        ///         this.InitializeComponent();
        ///         
        ///         this.model.Items.Add( "One" );
        ///         this.model.Items.Add( "Two" );
        ///         this.model.Items.Add( "Three" );
        ///         
        ///         // create collection of selectable items
        ///         var collection = new SelectableItemCollection<string>( this.model.Items );
        ///         
        ///         // create mediator that pushes changes back to the model
        ///         // note: since the model didn't use ObservableCollection<string>
        ///         // changes are one-way. direct model changes are not reflected.
        ///         collection.CreateSelectedValuesMediator( this.model.SelectedItems );
        /// 
        ///         this.DataContext = collection;
        ///     }
        /// }
        /// ]]></code></example>
        [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method creates and returns a disposable object." )]
        public virtual IDisposable CreateSelectedValuesMediator( ICollection<T> target )
        {
            Arg.NotNull( target, "target" );
            Contract.Ensures( Contract.Result<IDisposable>() != null );
            return new SelectedValuesMediator<T>( this.SelectedValues, target );
        }

        /// <summary>
        /// Creates a new selectable item.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to create the item for.</param>
        /// <returns>A <see cref="SelectableItem{T}"/> object.</returns>
        /// <remarks>This method is typically only used by the collection to internally create new items.</remarks>
        protected SelectableItem<T> CreateItem( T value )
        {
            Contract.Ensures( Contract.Result<SelectableItem<T>>() != null ); 
            return this.factory( value, this.ValueComparer );
        }
    }
}
