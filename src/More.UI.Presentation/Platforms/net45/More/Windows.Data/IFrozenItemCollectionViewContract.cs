namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    [ContractClassFor( typeof( IFrozenItemCollectionView ) )]
    abstract class IFrozenItemCollectionViewContract : IFrozenItemCollectionView
    {
        FrozenItemPosition IFrozenItemCollectionView.FrozenItemPosition { get; set; }

        IEnumerable IFrozenItemCollectionView.FrozenItems
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable>() != null );
                return null;
            }
        }

        IEnumerable IFrozenItemCollectionView.UnfrozenItems
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable>() != null );
                return null;
            }
        }

        bool ICollectionView.CanFilter => default( bool );

        bool ICollectionView.CanGroup => default( bool );

        bool ICollectionView.CanSort => default( bool );

        bool ICollectionView.Contains( object item ) => default( bool );

        CultureInfo ICollectionView.Culture { get; set; }

        event EventHandler ICollectionView.CurrentChanged
        {
            add { }
            remove { }
        }

        event CurrentChangingEventHandler ICollectionView.CurrentChanging
        {
            add { }
            remove { }
        }

        object ICollectionView.CurrentItem => null;

        int ICollectionView.CurrentPosition => default( int );

        IDisposable ICollectionView.DeferRefresh() => null;

        Predicate<object> ICollectionView.Filter { get; set; }

        ObservableCollection<GroupDescription> ICollectionView.GroupDescriptions => null;

        ReadOnlyObservableCollection<object> ICollectionView.Groups => null;

        bool ICollectionView.IsCurrentAfterLast => default( bool );

        bool ICollectionView.IsCurrentBeforeFirst => default( bool );

        bool ICollectionView.IsEmpty => default( bool );

        bool ICollectionView.MoveCurrentTo( object item ) => default( bool );

        bool ICollectionView.MoveCurrentToFirst() => default( bool );

        bool ICollectionView.MoveCurrentToLast() => default( bool );

        bool ICollectionView.MoveCurrentToNext() => default( bool );

        bool ICollectionView.MoveCurrentToPosition( int position ) => default( bool );

        bool ICollectionView.MoveCurrentToPrevious() => default( bool );

        void ICollectionView.Refresh() { }

        SortDescriptionCollection ICollectionView.SortDescriptions => null;

        IEnumerable ICollectionView.SourceCollection => null;

        IEnumerator IEnumerable.GetEnumerator() => null;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { }
            remove { }
        }
    }
}