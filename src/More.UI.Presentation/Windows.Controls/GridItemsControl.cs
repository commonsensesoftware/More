namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if UAP10_0
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using static global::Windows.UI.Xaml.GridUnitType;
    using GridLength = global::Windows.UI.Xaml.GridLength;
#else
    using System.Windows;
    using System.Windows.Controls;
    using static System.Windows.GridUnitType;
#endif

    /// <summary>
    /// Represents an <see cref="ItemsControl"/> that uses a <see cref="Grid"/> for its <see cref="ItemsControl.ItemsPanel"/> and dynamically creates
    /// a row for each item in the control.
    /// </summary>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    public class GridItemsControl : ItemsControl
    {
        /// <summary>
        /// Gets the last child fill dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register( nameof( LastChildFill ), typeof( bool ), typeof( GridItemsControl ), new PropertyMetadata( false, OnLastChildFillPropertyChanged ) );

        Grid grid;

        /// <summary>
        /// Gets the height of a row that that uses star (*) or fill height behavior.
        /// </summary>
        /// <value>A <see cref="GridLength"/> structure.</value>
        protected static readonly GridLength StarRowHeight = new GridLength( 1d, Star );

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
            DefaultStyleKey = typeof( GridItemsControl );
#pragma warning disable CA2214 // Do not call overridable methods in constructors
            Loaded += ( s, e ) => OnLoaded();
#pragma warning restore CA2214 // Do not call overridable methods in constructors
        }

        /// <summary>
        /// Gets the grid that is used as the <see cref="ItemsControl.ItemsPanel">items panel</see>.
        /// </summary>
        /// <value>A <see cref="Grid"/> object.</value>
        protected virtual Grid Grid => grid;

        /// <summary>
        /// Gets or sets a value indicating whether the last child element added resizes to fill the remaining space.
        /// </summary>
        /// <value>True if the last child element resizes to filll the remaining space; otherwise, false.  The default value is false.</value>
        public bool LastChildFill
        {
            get => (bool) GetValue( LastChildFillProperty );
            set => SetValue( LastChildFillProperty, value );
        }

        private static void OnLastChildFillPropertyChanged( object sender, DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            ( (GridItemsControl) sender ).OnLastChildFillChanged();
        }

        void ResetRows()
        {
            if ( !Grid.RowDefinitions.Any() )
            {
                return;
            }

            for ( var i = 0; i < Grid.RowDefinitions.Count; i++ )
            {
                Grid.RowDefinitions[i].Height = GetRowHeight( i );
            }

            for ( var i = 0; i < Items.Count; i++ )
            {
#if UAP10_0
                var element = (FrameworkElement) ContainerFromIndex( i );
#else
                var element = (FrameworkElement) ItemContainerGenerator.ContainerFromIndex( i );
#endif
                if ( element != null )
                {
                    Grid.SetRow( element, i );
                }
            }
        }

        /// <summary>
        /// Returns the height for the specified row.
        /// </summary>
        /// <param name="row">The zero-based row to return the height for.</param>
        /// <returns>A <see cref="GridLength"/> structure.</returns>
        protected virtual GridLength GetRowHeight( int row )
        {
            Arg.GreaterThanOrEqualTo( row, 0, nameof( row ) );

            if ( LastChildFill )
            {
                return ( Grid == null || row == Grid.RowDefinitions.Count - 1 ) ? StarRowHeight : AutoRowHeight;
            }

            return row == 0 ? StarRowHeight : AutoRowHeight;
        }

        /// <summary>
        /// Occurs when the control has been loaded.
        /// </summary>
        protected virtual void OnLoaded() => grid = this.GetItemsPanel<Grid>();

        /// <summary>
        /// Occurs when the <see cref="LastChildFill"/> property has changed.
        /// </summary>
        protected virtual void OnLastChildFillChanged()
        {
            if ( Grid == null || !Grid.RowDefinitions.Any() )
            {
                return;
            }

            Grid.RowDefinitions.First().Height = GetRowHeight( 0 );
            Grid.RowDefinitions.Last().Height = GetRowHeight( Grid.RowDefinitions.Count - 1 );
        }

        /// <summary>
        /// Overrides the default behavior when a container is prepared for an item.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/> representing the containing element.</param>
        /// <param name="item">The <see cref="object">item</see> to prepare.</param>
        protected override void PrepareContainerForItemOverride( DependencyObject element, object item )
        {
            base.PrepareContainerForItemOverride( element, item );

            if ( Grid == null )
            {
                return;
            }

            Grid.RowDefinitions.Add( new RowDefinition() );
            ResetRows();
        }

        /// <summary>
        /// Overrides the default behavior when an item is cleared.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/> representing the containing element.</param>
        /// <param name="item">The <see cref="object">item</see> to clear the container for.</param>
        protected override void ClearContainerForItemOverride( DependencyObject element, object item )
        {
            base.ClearContainerForItemOverride( element, item );

            if ( Grid == null || !Grid.RowDefinitions.Any() )
            {
                return;
            }

            Grid.RowDefinitions.RemoveAt( Grid.RowDefinitions.Count - 1 );
            ResetRows();
        }
    }
}