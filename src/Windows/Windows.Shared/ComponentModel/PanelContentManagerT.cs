namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Represents a content manager using a <see cref="T:Panel"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="T:Panel"/> used by the content manager.</typeparam>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public class PanelContentManager<T> : IContentManager where T : Panel, new()
    {
        private readonly T panel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelContentManager{T}"/> class.
        /// </summary>
        public PanelContentManager()
            : this( new T() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelContentManager{T}"/> class.
        /// </summary>
        /// <param name="panel">The underlying panel of type <typeparamref name="T"/> for the content manager.</param>
        public PanelContentManager( T panel )
        {
            Contract.Requires<ArgumentNullException>( panel != null, "panel" );
            this.panel = panel;
        }

        /// <summary>
        /// Gets the panel used by the content manager.
        /// </summary>
        /// <value>A panel object of type <typeparamref name="T"/>.</value>
        protected virtual T Panel
        {
            get
            {
                Contract.Ensures( Contract.Result<T>() != null );
                return this.panel;
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
                return this.Panel.Children.Cast<object>();
            }
        }

        /// <summary>
        /// Sets the current content with the specified content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to set.</param>
        public virtual void SetContent( object content )
        {
            // exit if there's nothing to do
            if ( this.Panel.Children.Count == 1 && object.Equals( this.Panel.Children[0], content ) )
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
            var element = (UIElement) content;
            this.Panel.Children.Add( element );
            var index = this.Panel.Children.IndexOf( element );
#else
            var index = this.Panel.Children.Add( (UIElement) content );
#endif
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, content, index ) );
        }

        /// <summary>
        /// Removes the specified content from the current content.
        /// </summary>
        /// <param name="content">The <see cref="Object">object</see> representing the content to remove.</param>
        public virtual void RemoveFromContent( object content )
        {
            var index = this.Panel.Children.IndexOf( (UIElement) content );

            if ( index < 0 )
                return;

            this.Panel.Children.RemoveAt( index );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, content, index ) );
        }

        /// <summary>
        /// Clears the current content.
        /// </summary>
        public virtual void ClearContent()
        {
            this.Panel.Children.Clear();
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        /// <summary>
        /// Occurs when the content collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
