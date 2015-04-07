namespace More
{
    using More.Windows.Controls;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;
    using global::Windows.UI.ViewManagement;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    internal static partial class Util
    {
        internal static CultureInfo GetCultureFromLanguage( string language )
        {
            return string.IsNullOrEmpty( language ) ? null : new CultureInfo( language );
        }

        internal static TExpected GetTypedOldValue<TExpected>( this DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // HACK: overcomes a WinRT bug/limitation that may prevent data binding to custom dependency properties
            // that use interface types.

            if ( typeof( TExpected ).GetTypeInfo().IsValueType || e.OldValue == null || e.OldValue is TExpected )
                return (TExpected) e.OldValue;

            throw new InvalidCastException();
        }

        internal static TExpected GetTypedNewValue<TExpected>( this DependencyPropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            // HACK: overcomes a WinRT bug/limitation that may prevent data binding to custom dependency properties
            // that use interface types.

            if ( typeof( TExpected ).GetTypeInfo().IsValueType || e.NewValue == null || e.NewValue is TExpected )
                return (TExpected) e.NewValue;

            throw new InvalidCastException();
        }

        internal static bool TryResolveNavigationService( object sourceObject, out INavigationService service )
        {
            Contract.Ensures( ( Contract.Result<bool>() && Contract.ValueAtReturn( out service ) != null ) || ( !Contract.Result<bool>() && Contract.ValueAtReturn( out service ) == null ) );
            service = null;

            var frame = sourceObject as Frame;

            if ( frame == null )
            {
                var page = sourceObject as Page;

                // use page's frame
                if ( page != null )
                    frame = page.Frame;
            }

            if ( frame != null )
            {
                // adapt to frame
                service = new FrameNavigationAdapter( frame );
                return true;
            }

            var webView = sourceObject as WebView;

            if ( webView != null )
            {
                // adapt to web view
                service = new WebViewNavigationAdapter( webView );
                return true;
            }

            // use source object if possible
            if ( ( service = sourceObject as INavigationService ) != null )
                return true;

            // use service provider to find any other registered service
            return ServiceProvider.Current.TryGetService( out service );
        }
    }
}
