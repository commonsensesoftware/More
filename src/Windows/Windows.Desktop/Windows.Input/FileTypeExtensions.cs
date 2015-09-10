namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal static class FileTypeExtensions
    {
        private const string FileFilterFormat = "{0} ({1})|{1}";

        private static IEnumerable<string> FixUpAsterisks( this IEnumerable<string> extensions )
        {
            Contract.Requires( extensions != null );
            Contract.Ensures( Contract.Result<IEnumerable<string>>() != null );

            foreach ( var extension in extensions.Where( e => !string.IsNullOrEmpty( e ) ) )
            {
                if ( extension[0] == '*' )
                    yield return extension;

                yield return extension.Insert( 0, "*" );
            }
        }

        internal static string ToFileFilter( this IEnumerable<FileType> fileTypes )
        {
            Contract.Requires( fileTypes != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var filters = from fileType in fileTypes
                          let exts = fileType.Extensions.FixUpAsterisks().ToArray()
                          where exts.Length > 0
                          let filter = FileFilterFormat.FormatInvariant( fileType.Name, string.Join( ", ", exts ), string.Join( ";", exts ) )
                          select filter;

            return string.Join( "|", filters );
        }
    }
}
