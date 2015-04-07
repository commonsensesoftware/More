namespace More.Windows.Controls
{
    using System;
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
    /// Represents an <see cref="ItemsControl"/> that uses a <see cref="Grid"/> for its <see cref="P:ItemsControl.ItemPanel"/> and dynamically creates
    /// a row for each item in the control.
    /// </summary>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public class GridItemsControl : ItemsControl
    {
        /// <summary>
        /// Gets the last child fill dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register( "LastChildFill", typeof( bool ), typeof( GridItemsControl ), new PropertyMetadata( false, OnLastChildFillPropertyChanged ) );

        private Grid grid;

        /// <summary>
        /// Gets the height of a row that that uses star (*) or fill height behavior.
        /// </summary>
        /// <value>A <see cref="GridLength"/> structure.</value>
        protected static readonly GridLength StarRowHeight = new GridLength( 1.0, GridUnitType.Star );

        /// <summary>
        /// Gets the height of a row that that automatic height behavior.
        /// </summary>
        /// <value>A <see cref="GridLength"/> structure.</value>
        protected static readonly GridLength AutoRowHeight = GridLength.Auto;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridItemsControl"/> class.
        /// </summary>
        public GridItemsControl()
        {
            this.DefaultStyleKey = typeof( GridItemsControl );
            this.Loaded += ( s, e ) => this.OnLoaded();
        }

        /// <summary>
        /// Gets the grid that is used as the <see cref="P:ItemsPanel">items panel</see>.
        /// </summary>
        /// <value>A <see cref="Grid"/> object.</value>
        protected virtual Grid Grid
        {
            get
            {
                return this.grid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last child element added resizes to fill the remaining space. 
        /// </summary>
        /// <value>True if the last child element resizes to filll the remaining space; otherwise, false.  The default value is false.</value>
        public bool LastChildFill
        {
            get
            {
                return (bool) this.GetValue( LastChildFillProperty );
            }
            set
            {
                this.SetValue( LastChildFillProperty, value );
            }
        }

        private static void OnLastChildFillPropertyChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            ( (GridItemsControl) sender ).OnLastChildFillChanged();
        }

        private void ResetRows()
        {
            if ( !this.Grid.RowDefinitions.Any() )
                return;

            // set row heights
            for ( var i = 0; i < this.Grid.RowDefinitions.Count; i++ )
                this.Grid.RowDefinitions[i].Height = this.GetRowHeight( i );

            // set row indexes
            for ( var i = 0; i < this.Items.Count; i++ )
            {
#if NETFX_CORE
                var element = (FrameworkElement) this.ContainerFromIndex( i );
#else
                var element = (FrameworkElement) this.ItemContainerGenerator.ContainerFromIndex( i );
#endif
                if ( element != null )
                    Grid.SetRow( element, i );
            }
        }

        /// <summary>
        /// Returns the height for the specified row.
        /// </summary>
        /// <param name="row">The zero-based row to return the height for.</param>
        /// <returns>A <see cref="GridLength"/> structure.</returns>
        protected virtual GridLength GetRowHeight( int row )
        {
            Contract.Requires<ArgumentOutOfRangeException>( row >= 0, "row" );

            if ( this.LastChildFill )
                return ( this.Grid == null || row == this.Grid.RowDefinitions.Count - 1 ) ? StarRowHeight : AutoRowHeight;

            return row == 0 ? StarRowHeight : AutoRowHeight;
        }

        /// <summary>
        /// Occurs when the control has been loaded.
        /// </summary>
        protected virtual void OnLoaded()
        {
            this.grid = this.GetItemsPanel<Grid>();
        }

        /// <summary>
        /// Occurs when the <see cref="P:LastChildFill"/> property has changed.
        /// </summary>
        protected virtual void OnLastChildFillChanged()
        {
            if ( this.Grid == null || !this.Grid.RowDefinitions.Any() )
                return;

            // update the heights for the first and last row
            this.Grid.RowDefinitions.First().Height = this.GetRowHeight( 0 );
            this.Grid.RowDefinitions.Last().Height = this.GetRowHeight( this.Grid.RowDefinitions.Count - 1 );
        }

        /// <summary>
        /// Overrides the default behavior when a container is prepared for an item.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/> representing the containing element.</param>
        /// <param name="item">The <see cref="Object">item</see> to prepare.</param>
        protected override void PrepareContainerForItemOverride( DependencyObject element, object item )
        {
            base.PrepareContainerForItemOverride( element, item );

            if ( this.Grid == null )
                return;

            this.Grid.RowDefinitions.Add( new RowDefinition() );
            this.ResetRows();
        }

        /// <summary>
        /// Overrides the default behavior when an item is cleared.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/> representing the containing element.</param>
        /// <param name="item">The <see cref="Object">item</see> to clear the container for.</param>
        protected override void ClearContainerForItemOverride( DependencyObject element, object item )
        {
            base.ClearContainerForItemOverride( element, item );

            if ( this.Grid == null || !this.Grid.RowDefinitions.Any() )
                return;

            this.Grid.RowDefinitions.RemoveAt( this.Grid.RowDefinitions.Count - 1 );
            this.ResetRows();
        }
    }
}
