namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an adapter class that makes an observable source <see cref="IList{T}">list</see> covariant and contravariant.
    /// </summary>
    /// <typeparam name="TFrom">The <see cref="Type">type</see> of item to make covariant.</typeparam>
    /// <typeparam name="TTo">The <see cref="Type">type</see> of contravariant item.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( CollectionDebugView<> ) )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is a specialized type that adapts to another list." )]
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is a specialized type that adapts to another list." )]
    public class ObservableVariantListAdapter<TFrom, TTo> : VariantListAdapter<TFrom, TTo>, INotifyPropertyChanged, INotifyCollectionChanged where TFrom : TTo
    {
        private volatile int suppressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableVariantListAdapter{TFrom,TTo}"/> class.
        /// </summary>
        /// <param name="list">The <see cref="IList{T}">list</see> to enable type variance for.</param>
        public ObservableVariantListAdapter( IList<TFrom> list )
            : base( list )
        {
            Arg.NotNull( list, nameof( list ) );

            var notifyProperty = list as INotifyPropertyChanged;
            var notifyCollection = list as INotifyCollectionChanged;

            if ( notifyProperty != null )
                notifyProperty.PropertyChanged += OnSourcePropertyChanged;

            if ( notifyCollection != null )
                notifyCollection.CollectionChanged += OnSourceCollectionChanged;
        }

        private bool SuppressEvents
        {
            get
            {
                return suppressions > 0;
            }
        }

        private void OnSourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            if ( SuppressEvents )
                return;

            switch ( e.PropertyName )
            {
                case "Count":
                case "Item[]":
                    {
                        OnPropertyChanged( e );
                        break;
                    }
            }
        }

        private void OnSourceCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            if ( !SuppressEvents )
                OnCollectionChanged( e );
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
        /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langkeyword="null">null</see>.</exception>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            CollectionChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Removes all of the items from the list and raises the appropriate events.
        /// </summary>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs:Action"/> as <see cref="T:NotifyCollectionChangedAction.Reset"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void ClearItems()
        {
            suppressions++;
            try
            {
                base.ClearItems();
                OnPropertyChanged( "Count" );
                OnPropertyChanged( "Item[]" );
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
            }
            finally
            {
                suppressions--;
            }
        }

        /// <summary>
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The <typeparamref name="TFrom">item</typeparamref> to insert.</param>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs:Action"/> as <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void InsertItem( int index, TFrom item )
        {
            suppressions++;
            try
            {
                base.InsertItem( index, item );
                OnPropertyChanged( "Count" );
                OnPropertyChanged( "Item[]" );
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
            }
            finally
            {
                suppressions--;
            }
        }

        /// <summary>
        /// Removes an item from the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Count"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs:Action"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void RemoveItem( int index )
        {
            suppressions++;
            try
            {
                var item = Items[index];
                base.RemoveItem( index );
                OnPropertyChanged( "Count" );
                OnPropertyChanged( "Item[]" );
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
            }
            finally
            {
                suppressions--;
            }
        }

        /// <summary>
        /// Replaces an item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The replacement <typeparamref name="TFrom">item</typeparamref> to set.</param>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs:Action"/> as <see cref="T:NotifyCollectionChangedAction.Replace"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void SetItem( int index, TFrom item )
        {
            suppressions++;
            try
            {
                var oldItem = Items[index];
                base.SetItem( index, item );
                OnPropertyChanged( "Item[]" );
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, item, oldItem, index ) );
            }
            finally
            {
                suppressions--;
            }
        }

        /// <summary>
        /// Moves an item from the specified old index to the new index.
        /// </summary>
        /// <param name="oldIndex">The zero-based index of the source item.</param>
        /// <param name="newIndex">The zero-based index of the destination in the list.</param>
        /// <remarks>This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName">property name</see> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs:Action"/> as <see cref="T:NotifyCollectionChangedAction.Move"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void MoveItem( int oldIndex, int newIndex )
        {
            suppressions++;
            try
            {
                var item = Items[oldIndex];
                base.RemoveItem( oldIndex );
                base.InsertItem( newIndex, item );
                OnPropertyChanged( "Item[]" );
                OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, item, newIndex, oldIndex ) );
            }
            finally
            {
                suppressions--;
            }
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
    }
}
