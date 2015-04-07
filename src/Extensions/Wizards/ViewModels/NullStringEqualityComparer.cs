namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a specialized string comparer where null strings are not equal.
    /// </summary>
    internal sealed class NullStringEqualityComparer : IEqualityComparer<string>
    {
        private static readonly NullStringEqualityComparer instance = new NullStringEqualityComparer();
        private readonly IEqualityComparer<string> defaultComparer = EqualityComparer<string>.Default;

        private NullStringEqualityComparer()
        {
        }

        internal static IEqualityComparer<string> Default
        {
            get
            {
                Contract.Ensures( instance != null );
                return instance;
            }
        }

        public bool Equals( string x, string y )
        {
            if ( x == null && y == null )
                return false;

            return this.defaultComparer.Equals( x, y );
        }

        public int GetHashCode( string obj )
        {
            return this.defaultComparer.GetHashCode( obj );
        }
    }
}
