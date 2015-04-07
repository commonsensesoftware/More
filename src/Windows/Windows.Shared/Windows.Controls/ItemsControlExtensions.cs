namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Provides extension methods for the <see cref="ItemsControl"/> class.
    /// </summary>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    public static class ItemsControlExtensions
    {
        private static TPanel FindItemsPanel<TPanel>( DependencyObject visual ) where TPanel : Panel
        {
            Contract.Requires( visual != null );

            for ( var i = 0; i < VisualTreeHelper.GetChildrenCount( visual ); i++ )
            {
                var child = VisualTreeHelper.GetChild( visual, i );

                if ( child == null )
                    continue;

                TPanel panel = child as TPanel;

                if ( panel != null && VisualTreeHelper.GetParent( child ) is ItemsPresenter )
                    return panel;

                panel = FindItemsPanel<TPanel>( child );

                if ( panel != null )
                    return panel;
            }

            return default( TPanel );
        }

        /// <summary>
        /// Returns a <see cref="Panel">panel</see> defined in the <see cref="P:ItemsControl.ItemPanelTemplate">item panel template</see> for an <see cref="ItemsControl">items control</see>.
        /// </summary>
        /// <typeparam name="TPanel">The <see cref="Type">type</see> of panel to retrieve.</typeparam>
        /// <param name="control">The <see cref="ItemsControl"/> to get the <see cref="Panel">panel</see> from.</param>
        /// <returns>A <see cref="Panel">panel</see> object of type <typeparamref name="TPanel"/>.</returns>
        /// <remarks>This method may return null if the control has not be loaded into the visual tree.  This method should only be leveraged after the <see cref="E:UIElement.Loaded"/> event.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only valid for ItemsControl controls." )]
        public static TPanel GetItemsPanel<TPanel>( this ItemsControl control ) where TPanel : Panel
        {
            Contract.Requires<ArgumentNullException>( control != null, "control" );
            return FindItemsPanel<TPanel>( control );
        }
    }
}
