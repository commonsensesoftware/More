namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a dynamic object comparer using a delegate.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to compare.</typeparam>
    /// <example>This example demonstrates a comparer that uses a lambda expression to dynamically compare Person objects.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// 
    /// var people = new List<Person>();
    /// people.Add( new Person(){ FirstName = "Bob", LastName = "Smith" };
    /// people.Add( new Person(){ FirstName = "John", LastName = "Doe" };
    /// people.Add( new Person(){ FirstName = "Bill", LastName = "Mei" };
    /// 
    /// var ascending = new DynamicComparer<Person>( ( p1, p2 ) => StringComparer.Ordinal.Compare( p1.LastName, p2.LastName ) );
    /// 
    /// people.Sort( ascending );
    /// ]]></code>
    /// </example>
    public sealed class DynamicComparer<T> : IComparer<T>, IComparer, IEqualityComparer<T>, IEqualityComparer
    {
        private readonly Func<T, T, int> comparison;
        private readonly Func<T, int> getHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicComparer{T}"/> class.
        /// </summary>
        /// <param name="comparison">The <see cref="Func{T1,T2,TResult}">function</see> used to compare two objects.</param>
        public DynamicComparer( Func<T, T, int> comparison )
            : this( comparison, o => o == null ? 0 : o.GetHashCode() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicComparer{T}"/> class.
        /// </summary>
        /// <param name="comparison">The <see cref="Func{T1,T2,TResult}">function</see> used to compare two objects.</param>
        /// <param name="getHashCode">The <see cref="Func{T1,TResult}">function</see> used to get the hash code for an object.</param>
        public DynamicComparer( Func<T, T, int> comparison, Func<T, int> getHashCode )
        {
            Arg.NotNull( comparison, "comparison" );
            Arg.NotNull( getHashCode, "getHashCode" );
            this.comparison = comparison;
            this.getHashCode = getHashCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicComparer{T}"/> class.
        /// </summary>
        /// <param name="getHashCode">The <see cref="Func{T1,TResult}">function</see> used to get the hash code for an object.</param>
        public DynamicComparer( Func<T, int> getHashCode )
        {
            Arg.NotNull( getHashCode, "getHashCode" );
            this.getHashCode = getHashCode;
            this.comparison = ( i1, i2 ) => this.GetHashCode( i1 ).CompareTo( this.GetHashCode( i2 ) );
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="x">The object of type <typeparamref name="T"/> that is the basis of the comparison.</param>
        /// <param name="y">The object of type <typeparamref name="T"/> to compare against.</param>
        /// <returns>Zero if the two objects are equal, one if <paramref name="x"/> is greater than <paramref name="y"/>, or
        /// negative one if <paramref name="x"/> is less than <paramref name="y"/>.</returns>
        public int Compare( T x, T y )
        {
            if ( default( T ) != null )
                return this.comparison( x, y );

            if ( x == null )
                return y == null ? 0 : -1;
            else if ( y == null )
                return 1;

            return this.comparison( x, y );
        }

        /// <summary>
        /// Compares the equality of two objects.
        /// </summary>
        /// <param name="x">The object of type <typeparamref name="T"/> that is the basis of the comparison.</param>
        /// <param name="y">The object of type <typeparamref name="T"/> to compare against.</param>
        /// <returns>True if the two objects are equal; otherwise, false.</returns>
        public bool Equals( T x, T y )
        {
            return this.comparison( x, y ) == 0;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object of type <typeparamref name="T"/> to get a hash code for.</param>
        /// <returns>A hash code.</returns>
        /// <remarks>This method returns the default implementation of <see cref="M:Object.GetHashCode"/>.</remarks>
        public int GetHashCode( T obj )
        {
            return this.getHashCode( obj );
        }

        int IComparer.Compare( object x, object y )
        {
            return this.Compare( (T) x, (T) y );
        }

        bool IEqualityComparer.Equals( object x, object y )
        {
            return this.Equals( (T) x, (T) y );
        }

        int IEqualityComparer.GetHashCode( object obj )
        {
            return this.GetHashCode( (T) obj );
        }
    }
}
