namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IQueue<> ) )]
    abstract class IQueueContract<T> : IQueue<T>
    {
        T IQueue<T>.Peek()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        T IQueue<T>.Dequeue()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        void IQueue<T>.Enqueue( T item ) => Contract.Ensures( ( (ICollection) this ).Count > 0 );

        void IQueue<T>.Clear() => Contract.Ensures( ( (ICollection) this ).Count == 0 );

        bool IQueue<T>.Contains( T item ) => default( bool );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "This is only a code contract definition." )]
        void IQueue<T>.CopyTo( T[] array, int arrayIndex )
        {
            Contract.Requires<ArgumentNullException>( array != null, nameof( array ) );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex >= 0, nameof( arrayIndex ) );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex <= ( array.Length + ( (ICollection) this ).Count ), nameof( arrayIndex ) );
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => null;

        IEnumerator IEnumerable.GetEnumerator() => null;

        void ICollection.CopyTo( Array array, int index ) { }

        int ICollection.Count => default( int );

        bool ICollection.IsSynchronized => default( bool );

        object ICollection.SyncRoot => null;
    }
}