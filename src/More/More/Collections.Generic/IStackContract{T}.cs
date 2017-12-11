namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof( IStack<> ) )]
    abstract class IStackContract<T> : IStack<T>
    {
        T IStack<T>.Peek()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        T IStack<T>.Pop()
        {
            Contract.Requires<InvalidOperationException>( ( (ICollection) this ).Count > 0 );
            return default( T );
        }

        void IStack<T>.Push( T item ) => Contract.Ensures( ( (ICollection) this ).Count > 0 );

        void IStack<T>.Clear() => Contract.Ensures( ( (ICollection) this ).Count == 0 );

        bool IStack<T>.Contains( T item ) => default( bool );

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "This is only a code contract definition." )]
        void IStack<T>.CopyTo( T[] array, int arrayIndex )
        {
            Contract.Requires<ArgumentNullException>( array != null, nameof( array ) );
            Contract.Requires<ArgumentException>( array.Rank == 1, nameof( array ) + ".Rank" );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex <= ( array.Length + ( (ICollection) this ).Count ), nameof( arrayIndex ) );
            Contract.Requires<ArgumentOutOfRangeException>( arrayIndex <= ( array.Length + ( (ICollection) this ).Count ), nameof( arrayIndex ) );
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => null;

        IEnumerator IEnumerable.GetEnumerator() => null;

        int IReadOnlyCollection<T>.Count => default( int );

        void ICollection.CopyTo( Array array, int index ) { }

        int ICollection.Count => default( int );

        bool ICollection.IsSynchronized => default( bool );

        object ICollection.SyncRoot => null;
    }
}