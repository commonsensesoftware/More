namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Reflection;

    /// <content>
    /// Provides the shared implementation of a private value comparer.
    /// </content>
    public abstract partial class ObservableObject
    {
        /// <summary>
        /// Represents the default value comparer.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of value being compared.</typeparam>
        private sealed class ValueComparer<TValue> : IEqualityComparer<TValue>
        {
            internal static readonly ValueComparer<TValue> Default = new ValueComparer<TValue>();

            public bool Equals( TValue x, TValue y )
            {
                var equatable = x as IEquatable<TValue>;

                // avoid boxing operations for value types
                if ( typeof( TValue ).GetTypeInfo().IsValueType )
                {
                    if ( equatable != null )
                        return equatable.Equals( y );
                    else
                        return x.Equals( y );
                }

                if ( x == null && y != null )
                    return false;
                else if ( x != null && y == null )
                    return false;

                if ( x != null && y != null )
                {
                    // special handling for strings
                    if ( typeof( TValue ) == typeof( string ) )
                        return StringComparer.Ordinal.Equals( x.ToString(), y.ToString() );
                    else if ( equatable != null )
                        return equatable.Equals( y );
                    else
                        return x.Equals( y );
                }

                // x and y are null
                return true;
            }

            public int GetHashCode( TValue obj )
            {
                return obj == null ? 0 : obj.GetHashCode();
            }
        }
    }
}
