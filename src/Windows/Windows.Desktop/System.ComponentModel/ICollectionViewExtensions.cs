namespace System.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Data;

    /// <summary>
    /// Provides extension methods for the <see cref="ICollectionView"/> interface.
    /// </summary>
    public static class ICollectionViewExtensions
    {
        private static ICollectionView Sort( ICollectionView view, string propertyName, ListSortDirection direction, bool append )
        {
            Contract.Requires( view != null, "view" );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            using ( var defer = view.DeferRefresh() )
            {
                if ( !append )
                    view.SortDescriptions.Clear();

                view.SortDescriptions.Add( new SortDescription( propertyName, direction ) );
            }

            return view;
        }

        /// <summary>
        /// Sets the sort direction of the collection view to ascending for the specified property name.
        /// </summary>
        /// <param name="view">The extended <see cref="ICollectionView"/> object.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <returns>An <see cref="ICollectionView"/> object.</returns>
        /// <remarks>Any existing sort descriptions are clear before the new sort description is added.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static ICollectionView OrderBy( this ICollectionView view, string propertyName )
        {
            Contract.Requires<ArgumentNullException>( view != null, "view" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            return Sort( view, propertyName, ListSortDirection.Ascending, false );
        }

        /// <summary>
        /// Sets the sort direction of the collection view to descending for the specified property name.
        /// </summary>
        /// <param name="view">The extended <see cref="ICollectionView"/> object.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <returns>An <see cref="ICollectionView"/> object.</returns>
        /// <remarks>Any existing sort descriptions are clear before the new sort description is added.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static ICollectionView OrderByDescending( this ICollectionView view, string propertyName )
        {
            Contract.Requires<ArgumentNullException>( view != null, "view" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            return Sort( view, propertyName, ListSortDirection.Descending, false );
        }

        /// <summary>
        /// Adds an ascending sort description to the collection view for the specified property name.
        /// </summary>
        /// <param name="view">The extended <see cref="ICollectionView"/> object.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <returns>An <see cref="ICollectionView"/> object.</returns>
        /// <remarks>The new sort description is appended to any existing sort descriptions.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static ICollectionView ThenBy( this ICollectionView view, string propertyName )
        {
            Contract.Requires<ArgumentNullException>( view != null, "view" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            return Sort( view, propertyName, ListSortDirection.Ascending, true );
        }

        /// <summary>
        /// Adds a descending sort description to the collection view for the specified property name.
        /// </summary>
        /// <param name="view">The extended <see cref="ICollectionView"/> object.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <returns>An <see cref="ICollectionView"/> object.</returns>
        /// <remarks>The new sort description is appended to any existing sort descriptions.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static ICollectionView ThenByDescending( this ICollectionView view, string propertyName )
        {
            Contract.Requires<ArgumentNullException>( view != null, "view" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            return Sort( view, propertyName, ListSortDirection.Descending, true );
        }

        /// <summary>
        /// Adds a group description to the collection view for the specified property name.
        /// </summary>
        /// <param name="view">The extended <see cref="ICollectionView"/> object.</param>
        /// <param name="propertyName">The name of the property to group by.</param>
        /// <returns>An <see cref="ICollectionView"/> object.</returns>
        /// <remarks>The new group description is appended to any existing group descriptions.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static ICollectionView GroupBy( this ICollectionView view, string propertyName )
        {
            Contract.Requires<ArgumentNullException>( view != null, "view" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( Contract.Result<ICollectionView>() != null );
            view.GroupDescriptions.Add( new PropertyGroupDescription( propertyName ) );
            return view;
        }

    }
}
