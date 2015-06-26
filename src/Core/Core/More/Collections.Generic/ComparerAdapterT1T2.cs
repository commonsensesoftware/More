namespace More.Collections.Generic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a comparer that adapts to the behavior of another comparer.
    /// </summary>
    /// <typeparam name="TItem">The <see cref="Type">type</see> of item to adapt to.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of value to compare.</typeparam>
    /// <example>This example demonstrates a comparer that compares Person objects by last name.
    /// <code lang="C#"><![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// 
    /// var people = new List<Person>();
    /// people.Add( new Person(){ FirstName = "Bob", LastName = "Smith" };
    /// people.Add( new Person(){ FirstName = "John", LastName = "Doe" };
    /// people.Add( new Person(){ FirstName = "Bill", LastName = "Mei" };
    /// 
    /// var comparer = new ComparerAdapter<Person, string>( StringComparer.OrdinalIgnoreCase, person => person.LastName );
    /// people.Sort( comparer );
    /// ]]></code>
    /// </example>
    public class ComparerAdapter<TItem, TValue> : IComparer<TItem>, IComparer, IEqualityComparer<TItem>, IEqualityComparer
    {
        private readonly DynamicComparer<TItem> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAdapter{TItem,TValue}"/> class.
        /// </summary>
        /// <param name="selector">The <see cref="Func{T1,TResult}"/> used to select a value for comparison.</param>
        /// <remarks>This constructor uses <see cref="T:Comparer{T}"/> as the adapted <see cref="IComparer{T}">comparer</see>
        /// and <see cref="M:EqualityComparer{T}.GetHashCode"/> as the default hash code provider.</remarks>
        public ComparerAdapter( Func<TItem, TValue> selector )
            : this( Comparer<TValue>.Default, selector, EqualityComparer<TValue>.Default.GetHashCode )
        {
            Arg.NotNull( selector, "selector" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAdapter{TItem,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{TItem}"/> used to compare values.</param>
        /// <param name="selector">The <see cref="Func{T1,TResult}"/> used to select a value for comparison.</param>
        /// <remarks>This constructor uses <see cref="M:EqualityComparer{T}.GetHashCode"/> as the default hash code provider.</remarks>
        public ComparerAdapter( IComparer<TValue> comparer, Func<TItem, TValue> selector )
            : this( comparer, selector, EqualityComparer<TValue>.Default.GetHashCode )
        {
            Arg.NotNull( comparer, "comparer" );
            Arg.NotNull( selector, "selector" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAdapter{TItem,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{TItem}"/> used to compare values.</param>
        /// <param name="selector">The <see cref="Func{T1,TResult}"/> used to select a value for comparison.</param>
        /// <param name="getHashCode">The <see cref="Func{T1,TResult}"/> used to get the hash code for a compared value.</param>
        public ComparerAdapter( IComparer<TValue> comparer, Func<TItem, TValue> selector, Func<TValue, int> getHashCode )
        {
            Arg.NotNull( comparer, "comparer" );
            Arg.NotNull( selector, "selector" );
            Arg.NotNull( getHashCode, "getHashCode" );
            this.comparer = new DynamicComparer<TItem>( ( x, y ) => comparer.Compare( selector( x ), selector( y ) ), o => getHashCode( selector( o ) ) );
        }

        /// <summary>
        /// Gets the underlying comparer.
        /// </summary>
        /// <value>An <see cref="IComparer{T}"/> object.</value>
        protected virtual IComparer<TItem> Comparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IComparer<TItem>>() != null );
                return this.comparer;
            }
        }

        /// <summary>
        /// Gets the underlying equality comparer.
        /// </summary>
        /// <value>An <see cref="IComparer{T}"/> object.</value>
        protected virtual IEqualityComparer<TItem> EqualityComparer
        {
            get
            {
                Contract.Ensures( Contract.Result<IEqualityComparer<TItem>>() != null );
                return this.comparer;
            }
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="x">The object of type <typeparamref name="TItem"/> that is the basis of the comparison.</param>
        /// <param name="y">The object of type <typeparamref name="TItem"/> to compare against.</param>
        /// <returns>Zero if the two objects are equal, one if <paramref name="x"/> is greater than <paramref name="y"/>, or
        /// negative one if <paramref name="x"/> is less than <paramref name="y"/>.</returns>
        public int Compare( TItem x, TItem y )
        {
            if ( default( TItem ) != null )
                return this.Comparer.Compare( x, y );

            if ( x == null )
                return y == null ? 0 : -1;
            else if ( y == null )
                return 1;

            return this.Comparer.Compare( x, y );
        }

        /// <summary>
        /// Compares the equality of two objects.
        /// </summary>
        /// <param name="x">The object of type <typeparamref name="TItem"/> that is the basis of the comparison.</param>
        /// <param name="y">The object of type <typeparamref name="TItem"/> to compare against.</param>
        /// <returns>True if the two objects are equal; otherwise, false.</returns>
        public virtual bool Equals( TItem x, TItem y )
        {
            if ( default( TItem ) != null )
                return this.EqualityComparer.Equals( x, y );

            if ( x == null )
                return y == null;
            else if ( y == null )
                return false;

            return this.EqualityComparer.Equals( x, y );
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object of type <typeparamref name="TItem"/> to get a hash code for.</param>
        /// <returns>A hash code.</returns>
        /// <remarks>This method returns the default implementation of <see cref="M:Object.GetHashCode"/>.</remarks>
        public virtual int GetHashCode( TItem obj )
        {
            return obj == null ? 0 : this.EqualityComparer.GetHashCode( obj );
        }

        int IComparer.Compare( object x, object y )
        {
            return this.Compare( (TItem) x, (TItem) y );
        }

        bool IEqualityComparer.Equals( object x, object y )
        {
            return this.Equals( (TItem) x, (TItem) y );
        }

        int IEqualityComparer.GetHashCode( object obj )
        {
            return this.GetHashCode( (TItem) obj );
        }
    }
}
