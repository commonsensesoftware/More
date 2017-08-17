namespace More
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;

    static class PageUriHelper
    {
        internal static bool TryGetPageFromUri( Uri uri, out Type pageType )
        {
            Contract.Requires( uri != null );

            pageType = null;
            string typeName;

            if ( uri == null )
            {
                return false;
            }

            if ( !uri.IsAbsoluteUri )
            {
                typeName = uri.OriginalString;
            }
            else if ( uri.Scheme == "urn" )
            {
                // parse based on urn:// scheme, which may have a scheme separator
                if ( uri.HostNameType == UriHostNameType.Unknown )
                {
                    typeName = uri.LocalPath;
                }
                else if ( uri.OriginalString.Length > 6 )
                {
                    typeName = uri.OriginalString.Substring( 6 );
                }
                else
                {
                    return false; // unexpected urn:
                }
            }
            else
            {
                // unsupported uri
                return false;
            }

            typeName = typeName.Trim( '/' );

            if ( ServiceProvider.Current.TryGetService( out ITypeResolutionService service ) )
            {
                pageType = service.GetType( typeName, false );
            }
            else
            {
                pageType = Type.GetType( typeName, false );
            }

            return pageType != null;
        }
    }
}