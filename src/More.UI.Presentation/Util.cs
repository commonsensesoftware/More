namespace More
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Text.RegularExpressions;

    static partial class Util
    {
        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not used by all platforms." )]
        internal static string HeaderTextFromPropertyName( string propertyName )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            const string PascalCasePattern = "[A-Z]{2}|[A-Z][a-z0-9]+";
            var header = Regex.Replace( propertyName, PascalCasePattern, "$0 ", RegexOptions.Singleline );

            return header.TrimEnd();
        }
    }
}