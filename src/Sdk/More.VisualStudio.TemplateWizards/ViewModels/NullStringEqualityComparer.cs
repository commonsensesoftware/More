namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    sealed class NullStringEqualityComparer : IEqualityComparer<string>
    {
        static readonly NullStringEqualityComparer instance = new NullStringEqualityComparer();
        readonly IEqualityComparer<string> defaultComparer = EqualityComparer<string>.Default;

        NullStringEqualityComparer() { }

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
            {
                return false;
            }

            return defaultComparer.Equals( x, y );
        }

        public int GetHashCode( string obj ) => defaultComparer.GetHashCode( obj );
    }
}