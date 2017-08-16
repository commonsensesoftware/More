namespace More.ComponentModel
{
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using static HierarchicalItemSelectionModes;

    /// <summary>
    /// Represents a hierarchy of items.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> for item in the collection.</typeparam>
    /// <example>This example demonstrates how to define a <see cref="T:System.Windows.Controls.UserControl"/> with a
    /// <see cref="T:System.Windows.Controls.TreeView"/> control.
    /// <code lang="Xaml">
    /// <![CDATA[
    /// <UserControl x:Class="MyProject.MyUserControl"
    ///              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///              xmlns:Templates="clr-namespace:System.Windows;assembly=System.Windows.Controls"
    ///              xmlns:Controls="clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls">
    /// <Grid>
    ///     <Controls:TreeView ItemsSource="{Binding Items}">
    ///         <Controls:TreeView.ItemTemplate>
    ///             <Templates:HierarchicalDataTemplate ItemsSource="{Binding}">
    ///                 <CheckBox Content="{Binding Value}" IsChecked="{Binding IsSelected, Mode=TwoWay}" IsThreeState="False" />
    ///             </Templates:HierarchicalDataTemplate>
    ///         </Controls:TreeView.ItemTemplate>
    ///     </Controls:TreeView>
    /// </Grid>
    /// </UserControl>
    /// ]]></code></example>
    /// <example>This example demonstrates how to create a <see cref="HierarchicalItemCollection{T}"/> that can be data bound to the
    /// <see cref="T:System.Windows.Controls.UserControl"/> in the previous example.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.ComponentModel;
    /// using System.Windows.Controls;
    /// 
    /// public partial class MyUserControl : UserControl
    /// {
    ///     public HierarchicalItemCollection<string> Items
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     public MyUserControl()
    ///     {
    ///         this.InitializeComponent();
    ///         var rootNode = new Node<string>( "All Features" )
    ///         {
    ///             new Node<string>( "Feature Group 1" )
    ///             {
    ///                 new Node<string>( "Feature 1.1" ),
    ///                 new Node<string>( "Feature 1.2" ),
    ///                 new Node<string>( "Feature 1.3" )
    ///             },
    ///             new Node<string>( "Feature Group 2" )
    ///             {
    ///                 new Node<string>( "Feature 2.1" ),
    ///                 new Node<string>( "Feature 2.2" ),
    ///                 new Node<string>( "Feature 2.3" )
    ///             },
    ///             new Node<string>( "Feature Group 3" )
    ///             {
    ///                 new Node<string>( "Feature 3.1" ),
    ///                 new Node<string>( "Feature 3.2" ),
    ///                 new Node<string>( "Feature 3.3" )
    ///             }
    ///         };
    ///         this.Items = new HierarchicalItemCollection<string>( rootNode, StringComparer.OrdinalIgnoreCase );
    ///         collection.SelectionMode = HierarchicalItemSelectionModes.All | HierarchicalItemSelectionModes.Synchronize;
    ///         this.DataContext = this;
    ///     }
    /// }
    /// ]]></code></example>
    /// <example>This example is identical to the previous example except that it sets the <see cref="P:HierarchicalItemCollection{T}.SelectionMode"/> to
    /// <see cref="T:HierarchicalItemSelectionModes.Leaf"/>, which only adds leaf items to the <see cref="P:HierarchicalItemCollection{T}.SelectedItems"/> collection.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// using System.ComponentModel;
    /// using System.Windows.Controls;
    /// 
    /// public partial class MyUserControl : UserControl
    /// {
    ///     public HierarchicalItemCollection<string> Items
    ///     {
    ///         get;
    ///         private set;
    ///     }
    ///     
    ///     public MyUserControl()
    ///     {
    ///         this.InitializeComponent();
    ///         var rootNode = new Node<string>( "All Features" )
    ///         {
    ///             new Node<string>( "Feature Group 1" )
    ///             {
    ///                 new Node<string>( "Feature 1.1" ),
    ///                 new Node<string>( "Feature 1.2" ),
    ///                 new Node<string>( "Feature 1.3" )
    ///             },
    ///             new Node<string>( "Feature Group 2" )
    ///             {
    ///                 new Node<string>( "Feature 2.1" ),
    ///                 new Node<string>( "Feature 2.2" ),
    ///                 new Node<string>( "Feature 2.3" )
    ///             },
    ///             new Node<string>( "Feature Group 3" )
    ///             {
    ///                 new Node<string>( "Feature 3.1" ),
    ///                 new Node<string>( "Feature 3.2" ),
    ///                 new Node<string>( "Feature 3.3" )
    ///             }
    ///         };
    ///         var collection = new HierarchicalItemCollection<string>( rootNode, StringComparer.OrdinalIgnoreCase );
    ///         collection.SelectionMode = HierarchicalItemSelectionModes.Leaf | HierarchicalItemSelectionModes.Synchronize;
    ///         this.Items = collection;
    ///         this.DataContext = this;
    ///     }
    /// }
    /// ]]></code></example>
    public class HierarchicalItemCollection<T> : ObservableCollection<HierarchicalItem<T>>
    {
        static readonly Node<T>[] EmptyNodes = new Node<T>[0];
        readonly IEqualityComparer<T> valueComparer;
        readonly IEqualityComparer<HierarchicalItem<T>> itemComparer;
        readonly SelectedItemCollection<T> selectedItems;
        readonly Func<Node<T>, IEqualityComparer<T>, HierarchicalItem<T>> factory;
        HierarchicalItemSelectionModes selectionModes = HierarchicalItemSelectionModes.All;

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        protected HierarchicalItemCollection()
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), EmptyNodes, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        protected HierarchicalItemCollection( IEqualityComparer<T> comparer )
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), EmptyNodes, comparer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="Func{T1,T2,TResult}">function</see> representing the factory method used to create new items.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        protected HierarchicalItemCollection( Func<Node<T>, IEqualityComparer<T>, HierarchicalItem<T>> factory )
            : this( factory, EmptyNodes, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="Func{T1,T2,TResult}">function</see> representing the factory method used to create new items.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        protected HierarchicalItemCollection( Func<Node<T>, IEqualityComparer<T>, HierarchicalItem<T>> factory, IEqualityComparer<T> comparer )
            : this( factory, EmptyNodes, comparer ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="rootNode">The <see cref="Node{T}"/> representing the root node in the collection.</param>
        public HierarchicalItemCollection( Node<T> rootNode )
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), rootNode == null ? EmptyNodes : new Node<T>[] { rootNode }, EqualityComparer<T>.Default )
        {
            Arg.NotNull( rootNode, nameof( rootNode ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="rootNode">The <see cref="Node{T}"/> representing the root node in the collection.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        public HierarchicalItemCollection( Node<T> rootNode, IEqualityComparer<T> comparer )
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), rootNode == null ? EmptyNodes : new Node<T>[] { rootNode }, comparer )
        {
            Arg.NotNull( rootNode, nameof( rootNode ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}">sequence</see> of root nodes for the collection.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public HierarchicalItemCollection( IEnumerable<Node<T>> nodes )
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), nodes, EqualityComparer<T>.Default ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalItemCollection{T}"/> class.
        /// </summary>
        /// <param name="nodes">The <see cref="IEnumerable{T}">sequence</see> of root nodes for the collection.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare items.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public HierarchicalItemCollection( IEnumerable<Node<T>> nodes, IEqualityComparer<T> comparer )
            : this( ( n, c ) => new HierarchicalItem<T>( n.Value, false, c ), nodes, comparer ) { }

        HierarchicalItemCollection( Func<Node<T>, IEqualityComparer<T>, HierarchicalItem<T>> factory, IEnumerable<Node<T>> nodes, IEqualityComparer<T> comparer )
        {
            Arg.NotNull( nodes, nameof( nodes ) );
            Arg.NotNull( comparer, nameof( comparer ) );

            this.factory = factory;
            valueComparer = comparer;
            itemComparer = new DynamicComparer<HierarchicalItem<T>>( ( i1, i2 ) => comparer.Equals( i1.Value, i2.Value ) ? 0 : -1, i => comparer.GetHashCode( i.Value ) );
            selectedItems = new SelectedItemCollection<T>( this );
            Items.AddRange( nodes.Select( n => CreateItem( n ) ) );

            if ( nodes is INotifyCollectionChanged notify )
            {
                notify.CollectionChanged += OnSourceCollectionChanged;
            }
        }

        bool SelectLeavesOnly => ( SelectionMode & Leaf ) == Leaf;

        bool SynchronizeSelections => ( SelectionMode & Synchronize ) == Synchronize;

        /// <summary>
        /// Gets the comparer used to evaluate item value equality.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        protected IEqualityComparer<T> ValueComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<T>>() != null );
                return valueComparer;
            }
        }

        /// <summary>
        /// Gets the comparer used to evaluate item equality.
        /// </summary>
        /// <value>An <see cref="IEqualityComparer{T}">comparer</see> object.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        protected virtual IEqualityComparer<HierarchicalItem<T>> ItemComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<HierarchicalItem<T>>>() != null );
                return itemComparer;
            }
        }

        /// <summary>
        /// Gets a collection of selected items.
        /// </summary>
        /// <value>An <see cref="ObservableCollection{T}"/> object.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public virtual ObservableCollection<HierarchicalItem<T>> SelectedItems
        {
            get
            {
                Contract.Ensures( Contract.Result<ObservableCollection<HierarchicalItem<T>>>() != null );
                return selectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the selection mode of the collection.
        /// </summary>
        /// <value>One or more of the <see cref="HierarchicalItemSelectionModes"/> values.</value>
        public HierarchicalItemSelectionModes SelectionMode
        {
            get => selectionModes;
            set
            {
                if ( selectionModes == value )
                {
                    return;
                }

                selectionModes = value;
                OnPropertyChanged( new PropertyChangedEventArgs( nameof( SelectionMode ) ) );
                SynchronizeItemsToSelectionMode();
            }
        }

        void SelectWithoutEvents( HierarchicalItem<T> item, bool? selected )
        {
            Contract.Requires( item != null );

            ( (INotifyPropertyChanged) item ).PropertyChanged -= OnItemPropertyChanged;

            // change state
            item.IsSelected = selected;

            if ( SelectLeavesOnly )
            {
                // add or remove from selected items
                if ( item.IsLeaf && ( selected ?? false ) && !SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Add( item );
                }
                else if ( !item.IsLeaf || ( !( selected ?? false ) && SelectedItems.Contains( item, ItemComparer ) ) )
                {
                    SelectedItems.Remove( item, ItemComparer );
                }
            }
            else
            {
                // add or remove from selected items
                if ( ( selected ?? false ) && !SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Add( item );
                }
                else if ( !( selected ?? false ) && SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Remove( item, ItemComparer );
                }
            }

            ( (INotifyPropertyChanged) item ).PropertyChanged += OnItemPropertyChanged;
        }

        void SynchronizeItemsToSelectionMode()
        {
            if ( SelectLeavesOnly )
            {
                var selections = SelectedItems.Where( i => !i.IsLeaf ).ToList();

                // remove all selections that are not leaves
                foreach ( var selection in selections )
                {
                    SelectedItems.Remove( selection, ItemComparer );
                }
            }
            else
            {
                // add selected items that are not currently in the collection
                foreach ( var item in this.Flatten() )
                {
                    if ( ( item.IsSelected ?? false ) && !SelectedItems.Contains( item, ItemComparer ) )
                    {
                        SelectedItems.Add( item );
                    }
                }
            }
        }

        static bool? GetSelectedState( HierarchicalItem<T> item, HierarchicalItem<T> parent )
        {
            Contract.Requires( item != null );
            Contract.Requires( parent != null );

            if ( !item.IsSelected.HasValue && parent.IsSelected.HasValue )
            {
                return null;
            }
            else if ( parent.All( i => i.IsSelected.HasValue && i.IsSelected.Value ) )
            {
                return true;
            }
            else if ( parent.All( i => i.IsSelected.HasValue && !i.IsSelected.Value ) )
            {
                return false;
            }

            return null;
        }

        static IEnumerable<HierarchicalItem<T>> EnumerateParents( HierarchicalItem<T> root )
        {
            Contract.Requires( root != null );

            var parent = root.Parent;

            while ( parent != null )
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        void OnItemIsSelectedChanged( HierarchicalItem<T> item )
        {
            Contract.Requires( item != null );

            var selected = item.IsSelected;

            if ( selected ?? false )
            {
                // do not select non-leaf items when appropriate and do not add the item more than once
                if ( ( !SelectLeavesOnly || item.IsLeaf ) && !SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Add( item );
                }
            }
            else
            {
                SelectedItems.Remove( item, ItemComparer );
            }

            if ( !SynchronizeSelections )
            {
                return;
            }

            // walk down the hierarchy and update children
            if ( selected != null )
            {
                item.Flatten().ForEach( i => SelectWithoutEvents( i, selected ) );
            }

            // walk up the hierarchy and update parents
            EnumerateParents( item ).ForEach( p => SelectWithoutEvents( p, GetSelectedState( item, p ) ) );
        }

        void OnItemIsLeafChanged( HierarchicalItem<T> item )
        {
            Contract.Requires( item != null );

            if ( SelectLeavesOnly )
            {
                // the selection of an item should change when its status as a leaf changes
                if ( item.IsLeaf && ( item.IsSelected ?? false ) && !SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Add( item );
                }
                else if ( !item.IsLeaf && SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Remove( item, ItemComparer );
                }
            }
            else
            {
                // this is unlikely, but ensure the item is selected
                if ( ( item.IsSelected ?? false ) && !SelectedItems.Contains( item, ItemComparer ) )
                {
                    SelectedItems.Add( item );
                }
            }
        }

        void AddItemsForNodes( HierarchicalItem<T> parent, IEnumerable<Node<T>> newItems )
        {
            Contract.Requires( parent != null );
            Contract.Requires( newItems != null );

            if ( SynchronizeSelections )
            {
                // this branch re-evaluates parent.IsSelected for every item added
                foreach ( var newItem in newItems )
                {
                    var item = CreateItem( newItem );
                    parent.Add( item );
                    parent.IsSelected = GetSelectedState( item, parent );
                }

                return;
            }

            // this branch adds the items without synchronizing the parent selection
            foreach ( var newItem in newItems )
            {
                parent.Add( CreateItem( newItem ) );
            }
        }

        void RemoveItemsForNodes( HierarchicalItem<T> parent, IEnumerable<Node<T>> oldItems )
        {
            Contract.Requires( parent != null );
            Contract.Requires( oldItems != null );

            // find all removed items from nodes
            var removedItems = ( from oldItem in oldItems
                                 from item in parent
                                 where ValueComparer.Equals( item.Value, oldItem.Value )
                                 select item ).ToList();

            if ( SynchronizeSelections )
            {
                // this branch re-evaluates parent.IsSelected for every item removed
                foreach ( var removedItem in removedItems )
                {
                    parent.Remove( removedItem );
                    parent.IsSelected = GetSelectedState( parent, parent );

                    if ( removedItem.IsSelected ?? false )
                    {
                        SelectedItems.Remove( removedItem, ItemComparer );
                    }
                }

                return;
            }

            // this branch removes the items without synchronizing the parent selection
            foreach ( var removedItem in removedItems )
            {
                parent.Remove( removedItem );

                if ( removedItem.IsSelected ?? false )
                {
                    SelectedItems.Remove( removedItem, ItemComparer );
                }
            }
        }

        void SetItemsForNodes( HierarchicalItem<T> parent, int index, IEnumerable<Node<T>> newItems )
        {
            Contract.Requires( parent != null );
            Contract.Requires( newItems != null );

            if ( SynchronizeSelections )
            {
                // this branch re-evaluates parent.IsSelected for every item replaced
                foreach ( var newItem in newItems )
                {
                    var oldItem = parent[index];
                    var replacement = CreateItem( newItem );

                    if ( oldItem.IsSelected ?? false )
                    {
                        SelectedItems.Remove( oldItem, ItemComparer );
                    }

                    parent[index++] = replacement;
                    parent.IsSelected = GetSelectedState( parent, parent );
                }

                return;
            }

            // this branch replaces the items without synchronizing the parent selection
            foreach ( var newItem in newItems )
            {
                var oldItem = parent[index];
                var replacement = CreateItem( newItem );

                if ( oldItem.IsSelected ?? false )
                {
                    SelectedItems.Remove( oldItem, ItemComparer );
                }

                parent[index++] = replacement;
            }
        }

        void OnSourceNodeChanged( HierarchicalItem<T> parent, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( parent != null );
            Contract.Requires( e != null );

            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if ( e.NewItems != null )
                        {
                            AddItemsForNodes( parent, e.NewItems.OfType<Node<T>>() );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if ( e.OldItems != null )
                        {
                            RemoveItemsForNodes( parent, e.OldItems.OfType<Node<T>>() );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        if ( e.OldItems != null && e.NewItems != null )
                        {
                            SetItemsForNodes( parent, e.NewStartingIndex, e.NewItems.OfType<Node<T>>() );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        parent.Clear();

                        if ( SynchronizeSelections )
                        {
                            parent.IsSelected = parent.IsSelected ?? false;
                        }

                        break;
                    }
            }
        }

        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is due to a large switch statement." )]
        void OnSourceCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if ( e.NewItems != null )
                        {
                            e.NewItems.OfType<Node<T>>().ForEach( n => Add( CreateItem( n ) ) );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if ( e.OldItems == null )
                        {
                            break;
                        }

                        var items = from node in e.OldItems.OfType<Node<T>>()
                                    from item in Items
                                    where Equals( node.Value, item.Value )
                                    select item;

                        items.ToList().ForEach( i => Remove( i ) );
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var items = from node in e.OldItems.OfType<Node<T>>()
                                    from item in Items
                                    where Equals( node.Value, item.Value )
                                    select item;

                        items.ToList().ForEach( i => Remove( i ) );

                        if ( e.NewItems != null )
                        {
                            e.NewItems.OfType<Node<T>>().ForEach( n => Add( CreateItem( n ) ) );
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        Clear();
                        break;
                    }
            }
        }

        void OnItemPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            var item = (HierarchicalItem<T>) sender;

            switch ( e.PropertyName )
            {
                case "IsLeaf":
                    {
                        OnItemIsLeafChanged( item );
                        break;
                    }
                case "IsSelected":
                    {
                        OnItemIsSelectedChanged( item );
                        break;
                    }
            }
        }

        /// <summary>
        /// Creates a new hierarchical item.
        /// </summary>
        /// <param name="node">The <see cref="Node{T}"/> to create the item for.</param>
        /// <returns>A <see cref="HierarchicalItem{T}"/> object.</returns>
        /// <remarks>This method is typically only used by the collection to internally create new items.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected HierarchicalItem<T> CreateItem( Node<T> node )
        {
            Arg.NotNull( node, nameof( node ) );
            Contract.Ensures( Contract.Result<HierarchicalItem<T>>() != null );

            var item = factory( node, ValueComparer );

            item.AddRange( node.Select( n => CreateItem( n ) ) );
            node.CollectionChanged += ( s, e ) => OnSourceNodeChanged( item, e );
            ( (INotifyPropertyChanged) item ).PropertyChanged += OnItemPropertyChanged;

            return item;
        }

        /// <summary>
        /// Creates a new hierarchical item from a given value.
        /// </summary>
        /// <param name="value">The value of type <typeparamref name="T"/> to create the item for.</param>
        /// <returns>A <see cref="HierarchicalItem{T}"/> object.</returns>
        /// <remarks>This method is typically only used by the collection to internally create new items.</remarks>
        protected HierarchicalItem<T> CreateItem( T value )
        {
            Contract.Ensures( Contract.Result<HierarchicalItem<T>>() != null );
            return CreateItem( new Node<T>( value ) );
        }

        /// <summary>
        /// Represents a collection of selected items in the parent collection.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type">type</see> of item in the collection.</typeparam>
        sealed class SelectedItemCollection<TItem> : ObservableCollection<HierarchicalItem<TItem>>
        {
            readonly HierarchicalItemCollection<TItem> owner;

            internal SelectedItemCollection( HierarchicalItemCollection<TItem> owner )
            {
                Contract.Requires( owner != null );
                this.owner = owner;
            }

            protected override void ClearItems()
            {
                foreach ( var item in this )
                {
                    owner.SelectWithoutEvents( item, false );
                }

                base.ClearItems();
            }

            [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Handled by base type. Null items are not allowed" )]
            protected override void InsertItem( int index, HierarchicalItem<TItem> item )
            {
                base.InsertItem( index, item );
                Contract.Assume( item != null );
                owner.SelectWithoutEvents( item, true );
            }

            protected override void RemoveItem( int index )
            {
                Contract.Assume( index >= 0 && index < Count );
                var item = this[index];

                base.RemoveItem( index );

                Contract.Assume( item != null );
                if ( item.IsSelected ?? false )
                {
                    owner.SelectWithoutEvents( item, false );
                }
            }

            [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Handled by base type. Null items are not allowed" )]
            protected override void SetItem( int index, HierarchicalItem<TItem> item )
            {
                Contract.Assume( index >= 0 && index < Count );
                var oldItem = this[index];
                base.SetItem( index, item );

                Contract.Assume( oldItem != null );
                if ( oldItem.IsSelected ?? false )
                {
                    owner.SelectWithoutEvents( oldItem, false );
                }

                Contract.Assume( item != null );
                owner.SelectWithoutEvents( item, true );
            }

        }
    }
}