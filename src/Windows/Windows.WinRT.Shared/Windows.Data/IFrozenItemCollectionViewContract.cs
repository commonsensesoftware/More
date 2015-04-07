namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices.WindowsRuntime;
    using global::Windows.Foundation;
    using global::Windows.Foundation.Collections;
    using global::Windows.UI.Xaml.Data;

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

        IObservableVector<object> ICollectionView.CollectionGroups
        {
            get
            {
                return null;
            }
        }

        event EventHandler<object> ICollectionView.CurrentChanged
        {
            add
            {
                return default( EventRegistrationToken );
            }
            remove
            {
            }
        }

        event CurrentChangingEventHandler ICollectionView.CurrentChanging
        {
            add
            {
                return default( EventRegistrationToken );
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

        bool ICollectionView.HasMoreItems
        {
            get
            {
                return default( bool );
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

        IAsyncOperation<LoadMoreItemsResult> ICollectionView.LoadMoreItemsAsync( uint count )
        {
            return null;
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

        bool ICollectionView.MoveCurrentToPosition( int index )
        {
            return default( bool );
        }

        bool ICollectionView.MoveCurrentToPrevious()
        {
            return default( bool );
        }

        event VectorChangedEventHandler<object> IObservableVector<object>.VectorChanged
        {
            add
            {
                return default( EventRegistrationToken );
            }
            remove
            {
            }
        }

        int IList<object>.IndexOf( object item )
        {
            return default( int );
        }

        void IList<object>.Insert( int index, object item )
        {
            throw new NotImplementedException();
        }

        void IList<object>.RemoveAt( int index )
        {
        }

        object IList<object>.this[int index]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        void ICollection<object>.Add( object item )
        {
        }

        void ICollection<object>.Clear()
        {
        }

        bool ICollection<object>.Contains( object item )
        {
            return default( bool );
        }

        void ICollection<object>.CopyTo( object[] array, int arrayIndex )
        {
        }

        int ICollection<object>.Count
        {
            get
            {
                return default( int );
            }
        }

        bool ICollection<object>.IsReadOnly
        {
            get
            {
                return default( bool );
            }
        }

        bool ICollection<object>.Remove( object item )
        {
            return default( bool );
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
}
