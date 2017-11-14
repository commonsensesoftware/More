namespace More.VisualStudio.Editors
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    sealed class UsingDirectiveComparer : IComparer<UsingDirectiveSyntax>
    {
        static readonly Lazy<IComparer<UsingDirectiveSyntax>> instance = new Lazy<IComparer<UsingDirectiveSyntax>>( () => new UsingDirectiveComparer() );

        UsingDirectiveComparer() { }

        internal static IComparer<UsingDirectiveSyntax> Instance
        {
            get
            {
                Contract.Ensures( Contract.Result<IComparer<UsingDirectiveSyntax>>() != null );
                return instance.Value;
            }
        }

        static int CompareAlias( NameEqualsSyntax x, NameEqualsSyntax y )
        {
            if ( x == null )
            {
                return y == null ? 0 : -1;
            }

            if ( y == null )
            {
                return 1;
            }

            return StringComparer.Ordinal.Compare( x.Name.Identifier.Text, y.Name.Identifier.Text );
        }

        static IEnumerable<string> GetNameParts( NameSyntax name ) => name.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select( n => n.Identifier.Text );

        static int CompareParts( IEnumerable<string> x, IEnumerable<string> y )
        {
            Contract.Requires( x != null );
            Contract.Requires( y != null );

            using ( var i1 = x.GetEnumerator() )
            {
                using ( var i2 = y.GetEnumerator() )
                {
                    if ( !i1.MoveNext() )
                    {
                        return i2.MoveNext() ? -1 : 0;
                    }

                    if ( !i2.MoveNext() )
                    {
                        return 1;
                    }

                    var comparer = StringComparer.Ordinal;
                    var result = comparer.Compare( i1.Current, i2.Current );

                    while ( result == 0 )
                    {
                        if ( i1.MoveNext() )
                        {
                            result = i2.MoveNext() ? comparer.Compare( i1.Current, i2.Current ) : 1;
                        }
                        else if ( i2.MoveNext() )
                        {
                            result = -1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    return result;
                }
            }
        }

        public int Compare( UsingDirectiveSyntax x, UsingDirectiveSyntax y )
        {
            if ( x == null )
            {
                return y == null ? 0 : -1;
            }

            if ( y == null )
            {
                return 1;
            }

            var result = CompareAlias( x.Alias, y.Alias );

            if ( result != 0 )
            {
                return result;
            }

            var name1 = GetNameParts( x.Name );
            var name2 = GetNameParts( y.Name );

            return CompareParts( name1, name2 );
        }
    }
}