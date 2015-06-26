namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a observable implementation of the <see cref="KeyedCollection{TKey,TItem}"/>
    /// class<seealso cref="INotifyCollectionChanged"/><seealso cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <typeparam name="TKey">The <see cref="Type">type</see> of the keys.</typeparam>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of values associated with the keys.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( CollectionDebugView<> ) )]
    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly Func<TItem, TKey> keyMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey,TItem}"/> class.
        /// </summary>
        /// <param name="getKeyMethod">The <see cref="Func{TItem,TKey}"/> used to retrieve an item's key.</param>
        public ObservableKeyedCollection( Func<TItem, TKey> getKeyMethod )
            : this( getKeyMethod, EqualityComparer<TKey>.Default )
        {
            Arg.NotNull( getKeyMethod, "getKeyMethod" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableKeyedCollection{TKey,TItem}"/> class.
        /// </summary>
        /// <param name="getKeyMethod">The <see cref="Func{TItem,TKey}"/> used to retrieve an item's key.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used for comparing item equality.</param>
        public ObservableKeyedCollection( Func<TItem, TKey> getKeyMethod, IEqualityComparer<TKey> comparer )
            : base( comparer )
        {
            Arg.NotNull( getKeyMethod, "getKeyMethod" );
            Arg.NotNull( comparer, "comparer" );
            this.keyMethod = getKeyMethod;
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
        /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langkeyword="null">null</see>.</exception>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Removes all items from the <see cref="ObservableKeyedCollection{TKey,TItem}"/>.
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
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Reset"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        /// <summary>
        /// Inserts an item into the <see cref="ObservableKeyedCollection{TKey,TItem}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
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
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Add"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void InsertItem( int index, TItem item )
        {
            base.InsertItem( index, item );
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
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
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Remove"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void RemoveItem( int index )
        {
            Contract.Assume( index >= 0 && index < this.Count );
            var item = this[index];
            base.RemoveItem( index );
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        /// <remarks>
        /// This method will raise the following events in order:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Event</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="PropertyChanged"/></term>
        ///         <description>The <see cref="P:PropertyChangedEventArgs.PropertyName"/> as "Item[]"</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CollectionChanged"/></term>
        ///         <description>The <see cref="P:NotifyCollectionChangedEventArgs.Acton"/> as <see cref="T:NotifyCollectionChangedAction.Replace"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        protected override void SetItem( int index, TItem item )
        {
            Contract.Assume( index >= 0 && index < this.Count );
            var oldItem = this[index];
            base.SetItem( index, item );
            this.OnPropertyChanged( "Item[]" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, item, oldItem, index ) );
        }

        /// <summary>
        /// Overrides the default behavior when the key for an item is retrieved.
        /// </summary>
        /// <param name="item">The <typeparamref name="TItem"/> to get the key for.</param>
        /// <returns>A <typeparamref name="TKey"/> object.</returns>
        protected override TKey GetKeyForItem( TItem item )
        {
            Contract.Assume( this.keyMethod != null );
            return this.keyMethod( item );
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
    }
}
