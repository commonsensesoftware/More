namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <content>
    /// Provides the shared implementation of a private value comparer.
    /// </content>
    public abstract partial class ObservableObject
    {
        /// <summary>
        /// Represents the default value comparer.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value being compared.</typeparam>
        sealed class ValueComparer<TValue> : IEqualityComparer<TValue>
        {
            ValueComparer() { }

            internal static ValueComparer<TValue> Default { get; } = new ValueComparer<TValue>();

            public bool Equals( TValue x, TValue y )
            {
                var equatable = x as IEquatable<TValue>;

                if ( default( TValue ) != null )
                {
                    if ( equatable != null )
                    {
                        return equatable.Equals( y );
                    }
                    else
                    {
                        return x.Equals( y );
                    }
                }

                if ( x == null && y != null )
                {
                    return false;
                }
                else if ( x != null && y == null )
                {
                    return false;
                }

                if ( x != null && y != null )
                {
                    if ( typeof( TValue ) == typeof( string ) )
                    {
                        return StringComparer.Ordinal.Equals( x.ToString(), y.ToString() );
                    }
                    else if ( equatable != null )
                    {
                        return equatable.Equals( y );
                    }
                    else
                    {
                        return x.Equals( y );
                    }
                }

                return true;
            }

            public int GetHashCode( TValue obj ) => obj?.GetHashCode() ?? 0;
        }
    }
}