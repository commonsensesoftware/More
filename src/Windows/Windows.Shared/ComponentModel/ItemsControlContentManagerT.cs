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
            Contract.Requires<ArgumentNullException>( itemsControl != null, "itemsControl" );
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
                return this.itemsControl;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Gets a sequence of views in the current content.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        public virtual IEnumerable<object> Content
        {
            get
            {
                return this.ItemsControl.Items.Cast<object>();
            }
        }

        /// <summary>
        /// Sets the current content with the specified content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to set.</param>
        public virtual void SetContent( object content )
        {
            // exit if there's nothing to do
            if ( this.ItemsControl.Items.Count == 1 && object.Equals( this.ItemsControl.Items[0], content ) )
                return;

            this.ClearContent();
            this.AddToContent( content );
        }

        /// <summary>
        /// Adds the specified content to the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to add.</param>
        public virtual void AddToContent( object content )
        {
#if NETFX_CORE
            this.ItemsControl.Items.Add( content );
            var index = this.ItemsControl.Items.IndexOf( content );
#else
            var index = this.ItemsControl.Items.Add( content );
#endif
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, content, index ) );
        }

        /// <summary>
        /// Removes the specified content from the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to remove.</param>
        public virtual void RemoveFromContent( object content )
        {
            var index = this.ItemsControl.Items.IndexOf( content );

            if ( index < 0 )
                return;

            this.ItemsControl.Items.RemoveAt( index );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, content, index ) );
        }

        /// <summary>
        /// Clears the current content.
        /// </summary>
        public virtual void ClearContent()
        {
            this.ItemsControl.Items.Clear();
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }
        /// <summary>
        /// Occurs when the content collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
