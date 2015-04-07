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
    internal abstract class IFrozenItemCollectionViewContract : IFrozenItemCollectionView
    {
        FrozenItemPosition IFrozenItemCollectionView.FrozenItemPosition
        {
            get;
            set;
        }

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

        bool ICollectionView.CanFilter
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.CanGroup
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.CanSort
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.Contains( object item )
        {
            return default( bool );
        }

        CultureInfo ICollectionView.Culture
        {
            get;
            set;
        }

        event EventHandler ICollectionView.CurrentChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        event CurrentChangingEventHandler ICollectionView.CurrentChanging
        {
            add
            {
            }
            remove
            {
            }
        }

        object ICollectionView.CurrentItem
        {
            get
            {
                return null;
            }
        }

        int ICollectionView.CurrentPosition
        {
            get
            {
                return default( int );
            }
        }

        IDisposable ICollectionView.DeferRefresh()
        {
            return null;
        }

        Predicate<object> ICollectionView.Filter
        {
            get;
            set;
        }

        ObservableCollection<GroupDescription> ICollectionView.GroupDescriptions
        {
            get
            {
                return null;
            }
        }

        ReadOnlyObservableCollection<object> ICollectionView.Groups
        {
            get
            {
                return null;
            }
        }

        bool ICollectionView.IsCurrentAfterLast
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.IsCurrentBeforeFirst
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.IsEmpty
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollectionView.MoveCurrentTo( object item )
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToFirst()
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToLast()
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToNext()
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToPosition( int position )
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToPrevious()
        {
            return default( bool );
        }

        void ICollectionView.Refresh()
        {
        }

        SortDescriptionCollection ICollectionView.SortDescriptions
        {
            get
            {
                return null;
            }
        }

        IEnumerable ICollectionView.SourceCollection
        {
            get
            {
                return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
            }
            remove
            {
            }
        }
    }
}
