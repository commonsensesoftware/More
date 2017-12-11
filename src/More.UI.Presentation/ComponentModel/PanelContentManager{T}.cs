namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if UAP10_0
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif
    using static System.Collections.Specialized.NotifyCollectionChangedAction;

    /// <summary>
    /// Represents a content manager using a <see cref="T:Panel"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="T:Panel"/> used by the content manager.</typeparam>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    public class PanelContentManager<T> : IContentManager where T : Panel, new()
    {
        readonly T panel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelContentManager{T}"/> class.
        /// </summary>
        public PanelContentManager() : this( new T() ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelContentManager{T}"/> class.
        /// </summary>
        /// <param name="panel">The underlying panel of type <typeparamref name="T"/> for the content manager.</param>
        public PanelContentManager( T panel )
        {
            Arg.NotNull( panel, nameof( panel ) );
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
                return panel;
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
        public virtual IEnumerable<object> Content => Panel.Children.Cast<object>();

        /// <summary>
        /// Sets the current content with the specified content.
        /// </summary>
        /// <param name="content">The <see cref="object">object</see> representing the content to set.</param>
        public virtual void SetContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );

            if ( Panel.Children.Count == 1 && Equals( Panel.Children[0], content ) )
            {
                return;
            }

            ClearContent();
            AddToContent( content );
        }

        /// <summary>
        /// Adds the specified content to the current content.
        /// </summary>
        /// <param name="content">The <see cref="object">object</see> representing the content to add.</param>
        public virtual void AddToContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );
#if UAP10_0
            var element = (UIElement) content;
            Panel.Children.Add( element );
            var index = Panel.Children.IndexOf( element );
#else
            var index = Panel.Children.Add( (UIElement) content );
#endif
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( Add, content, index ) );
        }

        /// <summary>
        /// Removes the specified content from the current content.
        /// </summary>
        /// <param name="content">The <see cref="object">object</see> representing the content to remove.</param>
        public virtual void RemoveFromContent( object content )
        {
            Arg.NotNull( content, nameof( content ) );

            var index = Panel.Children.IndexOf( (UIElement) content );

            if ( index < 0 )
            {
                return;
            }

            Panel.Children.RemoveAt( index );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( Remove, content, index ) );
        }

        /// <summary>
        /// Clears the current content.
        /// </summary>
        public virtual void ClearContent()
        {
            Panel.Children.Clear();
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( Reset ) );
        }

        /// <summary>
        /// Occurs when the content collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}