namespace More.VisualStudio.Editors
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal sealed class UsingDirectiveComparer : IComparer<UsingDirectiveSyntax>
    {
        private static readonly Lazy<IComparer<UsingDirectiveSyntax>> instance = new Lazy<IComparer<UsingDirectiveSyntax>>( () => new UsingDirectiveComparer() );

        private UsingDirectiveComparer()
        {
        }

        internal static IComparer<UsingDirectiveSyntax> Instance
        {
            get
            {
                Contract.Ensures( Contract.Result<IComparer<UsingDirectiveSyntax>>() != null );
                return instance.Value;
            }
        }

        private static int CompareAlias( NameEqualsSyntax x, NameEqualsSyntax y )
        {
            if ( x == null )
                return y == null ? 0 : -1;

            if ( y == null )
                return 1;

            return StringComparer.Ordinal.Compare( x.Name.Identifier.Text, y.Name.Identifier.Text );
        }

        private static IEnumerable<string> GetNameParts( NameSyntax name ) => name.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select( n => n.Identifier.Text );

        private static int CompareParts( IEnumerable<string> x, IEnumerable<string> y )
        {
            Contract.Requires( x != null );
            Contract.Requires( y != null );

            using ( var i1 = x.GetEnumerator() )
            {
                using ( var i2 = y.GetEnumerator() )
                {
                    // i1 and i2 are both empty or i2 is longer
                    if ( !i1.MoveNext() )
                        return i2.MoveNext() ? -1 : 0;

                    // i1 is longer
                    if ( !i2.MoveNext() )
                        return 1;

                    var comparer = StringComparer.Ordinal;
                    var result = comparer.Compare( i1.Current, i2.Current );

                    // loop while the current segment isn't equal
                    while ( result == 0 )
                    {
                        // compare the current segment. stop when the current segment isn't equal or
                        // the end of one or both sequences is reached
                        if ( i1.MoveNext() )
                            result = i2.MoveNext() ? comparer.Compare( i1.Current, i2.Current ) : 1;
                        else if ( i2.MoveNext() )
                            result = -1;
                        else
                            break;
                    }

                    return result;
                }
            }
        }

        public int Compare( UsingDirectiveSyntax x, UsingDirectiveSyntax y )
        {
            if ( x == null )
                return y == null ? 0 : -1;

            if ( y == null )
                return 1;

            // compare aliases first; they should be at the end
            var result = CompareAlias( x.Alias, y.Alias );

            if ( result != 0 )
                return result;

            var name1 = GetNameParts( x.Name );
            var name2 = GetNameParts( y.Name );

            // compare by identifier parts
            return CompareParts( name1, name2 );
        }
    }
}
