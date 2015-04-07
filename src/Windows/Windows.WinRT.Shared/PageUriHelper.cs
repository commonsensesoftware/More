namespace More
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;

    internal static class PageUriHelper
    {
        internal static bool TryGetPageFromUri( Uri uri, out Type pageType )
        {
            Contract.Requires( uri != null );

            pageType = null;
            string typeName;

            if ( uri == null )
                return false;

            if ( !uri.IsAbsoluteUri )
            {
                // relative uri
                typeName = uri.OriginalString;
            }
            else if ( uri.Scheme == "urn" )
            {
                // parse based on urn:// scheme, which may have a scheme separator
                if ( uri.HostNameType == UriHostNameType.Unknown )
                    typeName = uri.LocalPath;
                else if ( uri.OriginalString.Length > 6 )
                    typeName = uri.OriginalString.Substring( 6 );
                else
                    return false; // unexpected urn:
            }
            else
            {
                // unsupported uri
                return false;
            }

            // trim any leading or trailing slashes
            typeName = typeName.Trim( '/' );

            ITypeResolutionService service;

            // use the registered type resolution service, if one is found; otherwise, failover to default type resolution
            if ( ServiceProvider.Current.TryGetService( out service ) )
                pageType = service.GetType( typeName, false );
            else
                pageType = Type.GetType( typeName, false );

            return pageType != null;
        }
    }
}
