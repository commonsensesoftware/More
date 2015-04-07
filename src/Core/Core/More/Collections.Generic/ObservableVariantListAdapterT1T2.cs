namespace More.Collections.Generic
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.Specialized;
    using global::System.ComponentModel;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>( list != null, "list" );

            var notifyProperty = list as INotifyPropertyChanged;
            var notifyCollection = list as INotifyCollectionChanged;

            if ( notifyProperty != null )
                notifyProperty.PropertyChanged += this.OnSourcePropertyChanged;

            if ( notifyCollection != null )
                notifyCollection.CollectionChanged += this.OnSourceCollectionChanged;
        }

        private bool SuppressEvents
        {
            get
            {
                return this.suppressions > 0;
            }
        }

        private void OnSourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null, "sender" );
            Contract.Requires( e != null, "e" );

            if ( this.SuppressEvents )
                return;

            switch ( e.PropertyName )
            {
                case "Count":
                case "Item[]":
                    {
                        this.OnPropertyChanged( e );
                        break;
                    }
            }
        }

        private void OnSourceCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( sender != null, "sender" );
            Contract.Requires( e != null, "e" );

            if ( !this.SuppressEvents )
                this.OnCollectionChanged( e );
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
        /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langkeyword="null">null</see>.</exception>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
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
            this.suppressions++;
            try
            {
                base.ClearItems();
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
            }
            finally
            {
                this.suppressions--;
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
            this.suppressions++;
            try
            {
                base.InsertItem( index, item );
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
            }
            finally
            {
                this.suppressions--;
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
            this.suppressions++;
            try
            {
                var item = this.Items[index];
                base.RemoveItem( index );
                this.OnPropertyChanged( "Count" );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
            }
            finally
            {
                this.suppressions--;
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
            this.suppressions++;
            try
            {
                var oldItem = this.Items[index];
                base.SetItem( index, item );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, oldItem, item, index ) );
            }
            finally
            {
                this.suppressions--;
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
            this.suppressions++;
            try
            {
                var item = this.Items[oldIndex];
                base.RemoveItem( oldIndex );
                base.InsertItem( newIndex, item );
                this.OnPropertyChanged( "Item[]" );
                this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, item, newIndex, oldIndex ) );
            }
            finally
            {
                this.suppressions--;
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
