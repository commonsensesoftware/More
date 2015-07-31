namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Navigation;

    internal sealed class NavigatingCancelEventArgsAdapter : INavigationStartingEventArgs
    {
        private readonly NavigatingCancelEventArgs source;

        internal NavigatingCancelEventArgsAdapter( NavigatingCancelEventArgs source )
        {
            Contract.Requires( source != null );
            this.source = source;
        }

        public object SourceEventArgs
        {
            get
            {
                return source;
            }
        }

        public bool Cancel
        {
            get
            {
                return source.Cancel;
            }
            set
            {
                source.Cancel = value;
            }
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement INavigationStartingEventArgs." )]
        public Uri Uri
        {
            get;
            private set;
        }
    }
}
