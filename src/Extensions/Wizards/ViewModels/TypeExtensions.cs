namespace More.VisualStudio.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    internal static class TypeExtensions
    {
        private static readonly Type Root = typeof( object );

        internal static bool IsRestricted( this Type type, ICollection<string> restrictedBaseTypeNames )
        {
            Contract.Requires( type != null );
            Contract.Requires( restrictedBaseTypeNames != null );

            if ( restrictedBaseTypeNames.Count == 0 )
                return false;

            var current = type.BaseType;

            while ( current != null && current != Root )
            {
                if ( restrictedBaseTypeNames.Contains( current.FullName ) || restrictedBaseTypeNames.Contains( current.Name ) )
                    return true;

                current = current.BaseType;
            }

            return false;
        }
    }
}
