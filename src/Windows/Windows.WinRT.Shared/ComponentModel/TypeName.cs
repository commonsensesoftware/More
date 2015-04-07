namespace More.ComponentModel
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Text.RegularExpressions;

    [DebuggerDisplay( "Name = {FullName}" )]
    internal struct TypeName
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

            this.FullName = parts.Item1;
            this.Name = parts.Item2;
            this.AssemblyName = parts.Item3;
            this.HasNamespace = parts.Item4;
            this.IsAssemblyQualified = !string.IsNullOrEmpty( this.AssemblyName );
        }

        internal TypeName( Type type )
        {
            Contract.Requires( type != null );

            this.Name = type.Name;
            this.FullName = type.FullName;
            this.AssemblyName = type.GetTypeInfo().Assembly.GetName().Name;
            this.HasNamespace = true;
            this.IsAssemblyQualified = true;
        }

        private static Tuple<string, string, string, bool> Parse( string typeName )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );
            Contract.Ensures( Contract.Result<Tuple<string, string, string, bool>>() != null );

            var match = Regex.Match( typeName, @"^(([^,]+\.)?([^\x20,]+))(\x20*,\x20*([^\x20,]+))?", RegexOptions.Singleline );

            if ( match.Success )
                return new Tuple<string, string, string, bool>( match.Groups[1].Value, match.Groups[3].Value, match.Groups[5].Value, match.Groups[2].Success );

            return new Tuple<string, string, string, bool>( string.Empty, string.Empty, string.Empty, false );
        }

        internal bool IsMatch( TypeName other )
        {
            var comparer = StringComparer.Ordinal;

            // if this type name doesn't have a namespace, then we can only match on name.  if we do have a namespace, then match on full name
            if ( ( !this.HasNamespace && comparer.Equals( this.Name, other.Name ) ) || comparer.Equals( this.FullName, other.FullName ) )
            {
                // if either type name doesn't have an assembly name, then treat them equally because they can't be compared
                if ( !this.IsAssemblyQualified || !other.IsAssemblyQualified || comparer.Equals( this.AssemblyName, other.AssemblyName ) )
                    return true;
            }

            return false;
        }
    }
}
