namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Represents a content manager using an <see cref="T:ItemsControl"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="T:ItemsControl"/> used by the content manager.</typeparam>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public class ItemsControlContentManager<T> : IContentManager where T : ItemsControl, new()
    {
        private readonly T itemsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlContentManager{T}"/> class.
        /// </summary>
        public ItemsControlContentManager()
            : this( new T() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControlContentManager{T}"/> class.
        /// </summary>
        /// <param name="itemsControl">The underlying items control of type <typeparamref name="T"/> for the content manager.</param>
        public ItemsControlContentManager( T itemsControl )
        {
            Arg.NotNull( itemsControl, nameof( itemsControl ) );
            this.itemsControl = itemsControl;
        }

        /// <summary>
        /// Gets the items control used by the content manager.
        /// </summary>
        /// <value>A items control object of type <typeparamref name="T"/>.</value>
        protected virtual T ItemsControl
        {
            get
            {
                Contract.Ensures( Contract.Result<T>() != null );
                return itemsControl;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );

            CollectionChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Gets a sequence of views in the current content.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        public virtual IEnumerable<object> Content
        {
            get
            {
                return ItemsControl.Items.Cast<object>();
            }
        }

        /// <summary>
        /// Sets the current content with the specified content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to set.</param>
        public virtual void SetContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );

            // exit if there's nothing to do
            if ( ItemsControl.Items.Count == 1 && object.Equals( ItemsControl.Items[0], content ) )
                return;

            ClearContent();
            AddToContent( content );
        }

        /// <summary>
        /// Adds the specified content to the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to add.</param>
        public virtual void AddToContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );
#if NETFX_CORE
            ItemsControl.Items.Add( content );
            var index = ItemsControl.Items.IndexOf( content );
#else
            var index = ItemsControl.Items.Add( content );
#endif
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, content, index ) );
        }

        /// <summary>
        /// Removes the specified content from the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to remove.</param>
        public virtual void RemoveFromContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );

            var index = ItemsControl.Items.IndexOf( content );

            if ( index < 0 )
                return;

            ItemsControl.Items.RemoveAt( index );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, content, index ) );
        }

        /// <summary>
        /// Clears the current content.
        /// </summary>
        public virtual void ClearContent()
        {
            ItemsControl.Items.Clear();
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }
        /// <summary>
        /// Occurs when the content collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
