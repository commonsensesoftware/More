namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    static class FileTypeExtensions
    {
        static bool IsValid( this string extension )
        {
            if ( string.IsNullOrEmpty( extension ) )
            {
                return false;
            }

            // no filter should be applied for WinRT when using asterisk
            if ( extension == ".*" || extension == "*.*" || extension == "*" )
            {
                return false;
            }

            return true;
        }

        internal static IEnumerable<string> FixUpExtensions( this IEnumerable<FileType> fileTypes )
        {
            Contract.Requires( fileTypes != null );
            Contract.Ensures( Contract.Result<IEnumerable<string>>() != null );

            // if defined as "*.txt", it should become ".txt" for WinRT
            return from fileType in fileTypes
                   from extension in fileType.Extensions
                   where extension.IsValid()
                   select extension.TrimStart( '*' );
        }

        internal static IDictionary<string, IList<string>> ToDictionary( this IEnumerable<FileType> fileTypes )
        {
            Contract.Requires( fileTypes != null );
            Contract.Ensures( Contract.Result<IDictionary<string, IList<string>>>() != null );

            return ( from fileType in fileTypes
                     let exts = fileType.Extensions.Where( IsValid ).Select( e => e.TrimStart( '*' ) ).ToArray()
                     select new { Name = fileType.Name, Extensions = exts } ).ToDictionary( i => i.Name, i => (IList<string>) i.Extensions );
        }
    }
}