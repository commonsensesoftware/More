namespace More.Windows.Controls
{
    using global::Windows.UI.Xaml.Navigation;
    using System;
    using System.Diagnostics.Contracts;

    sealed class NavigationEventArgsAdapter : INavigationEventArgs
    {
        readonly object source;

        internal NavigationEventArgsAdapter( NavigationEventArgs source )
        {
            Contract.Requires( source != null );

            this.source = source;
            Uri = source.Uri;
            IsSuccess = true;
        }

        internal NavigationEventArgsAdapter( NavigationFailedEventArgs source )
        {
            Contract.Requires( source != null );

            this.source = source;
            IsSuccess = false;
        }

        public object SourceEventArgs => source;

        public bool IsSuccess { get; }

        public Uri Uri { get; }
    }
}