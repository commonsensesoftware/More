namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Navigation;
    
    internal sealed class NavigationEventArgsAdapter : INavigationEventArgs
    {
        private readonly object source;

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

        public object SourceEventArgs
        {
            get
            {
                return source;
            }
        }

        public bool IsSuccess
        {
            get;
            private set;
        }

        public Uri Uri
        {
            get;
            private set;
        }
    }
}
