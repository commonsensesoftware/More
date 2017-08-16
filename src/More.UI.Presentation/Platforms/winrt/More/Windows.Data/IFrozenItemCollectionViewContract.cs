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

        IObservableVector<object> ICollectionView.CollectionGroups => null;

        event EventHandler<object> ICollectionView.CurrentChanged
        {
            add => default( EventRegistrationToken );
            remove { }
        }

        event CurrentChangingEventHandler ICollectionView.CurrentChanging
        {
            add => default( EventRegistrationToken );
            remove { }
        }

        object ICollectionView.CurrentItem => null;

        int ICollectionView.CurrentPosition => default( int );

        bool ICollectionView.HasMoreItems => default( bool );

        bool ICollectionView.IsCurrentAfterLast => default( bool );

        bool ICollectionView.IsCurrentBeforeFirst => default( bool );

        IAsyncOperation<LoadMoreItemsResult> ICollectionView.LoadMoreItemsAsync( uint count ) => null;

        bool ICollectionView.MoveCurrentTo( object item ) => default( bool );

        bool ICollectionView.MoveCurrentToFirst() => default( bool );

        bool ICollectionView.MoveCurrentToLast() => default( bool );

        bool ICollectionView.MoveCurrentToNext() => default( bool );

        bool ICollectionView.MoveCurrentToPosition( int index ) => default( bool );

        bool ICollectionView.MoveCurrentToPrevious() => default( bool );

        event VectorChangedEventHandler<object> IObservableVector<object>.VectorChanged
        {
            add => default( EventRegistrationToken );
            remove { }
        }

        int IList<object>.IndexOf( object item ) => default( int );

        void IList<object>.Insert( int index, object item ) { }

        void IList<object>.RemoveAt( int index ) { }

        object IList<object>.this[int index]
        {
            get => null;
            set { }
        }

        void ICollection<object>.Add( object item ) { }

        void ICollection<object>.Clear() { }

        bool ICollection<object>.Contains( object item ) => default( bool );

        void ICollection<object>.CopyTo( object[] array, int arrayIndex ) { }

        int ICollection<object>.Count => default( int );

        bool ICollection<object>.IsReadOnly => default( bool );

        bool ICollection<object>.Remove( object item ) => default( bool );

        IEnumerator<object> IEnumerable<object>.GetEnumerator() => null;

        IEnumerator IEnumerable.GetEnumerator() => null;
    }
}