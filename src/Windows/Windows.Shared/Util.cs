namespace More
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text.RegularExpressions;

    internal static partial class Util
    {
        internal static string HeaderTextFromPropertyName( string propertyName )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            // use standard Pascal casing rules
            var header = Regex.Replace( propertyName, "[A-Z]{2}|[A-Z][a-z0-9]+", "$0 ", RegexOptions.Singleline );

            return header.TrimEnd();
        }
    }
}
