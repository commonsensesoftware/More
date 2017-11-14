namespace More.ComponentModel
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [DebuggerDisplay( "Name = {FullName}" )]
    struct TypeName
    {
        internal readonly string Name;
        internal readonly string FullName;
        internal readonly string AssemblyName;
        internal readonly bool HasNamespace;
        internal readonly bool IsAssemblyQualified;

        internal TypeName( string typeName )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );

            var parts = Parse( typeName );

            FullName = parts.Item1;
            Name = parts.Item2;
            AssemblyName = parts.Item3;
            HasNamespace = parts.Item4;
            IsAssemblyQualified = !string.IsNullOrEmpty( AssemblyName );
        }

        internal TypeName( Type type )
        {
            Contract.Requires( type != null );

            Name = type.Name;
            FullName = type.FullName;
            AssemblyName = type.GetTypeInfo().Assembly.GetName().Name;
            HasNamespace = true;
            IsAssemblyQualified = true;
        }

        static Tuple<string, string, string, bool> Parse( string typeName )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );
            Contract.Ensures( Contract.Result<Tuple<string, string, string, bool>>() != null );

            var match = Regex.Match( typeName, @"^(([^,]+\.)?([^\x20,]+))(\x20*,\x20*([^\x20,]+))?", RegexOptions.Singleline );

            if ( match.Success )
            {
                return Tuple.Create( match.Groups[1].Value, match.Groups[3].Value, match.Groups[5].Value, match.Groups[2].Success );
            }

            return Tuple.Create( string.Empty, string.Empty, string.Empty, false );
        }

        internal bool IsMatch( TypeName other )
        {
            var comparer = StringComparer.Ordinal;

            // if this type name doesn't have a namespace, then we can only match on name.  if we do have a namespace, then match on full name
            if ( ( !HasNamespace && comparer.Equals( Name, other.Name ) ) || comparer.Equals( FullName, other.FullName ) )
            {
                // if either type name doesn't have an assembly name, then treat them equally because they can't be compared
                if ( !IsAssemblyQualified || !other.IsAssemblyQualified || comparer.Equals( AssemblyName, other.AssemblyName ) )
                {
                    return true;
                }
            }

            return false;
        }
    }
}