namespace More.Windows.Controls
{
    using System;
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
                return this.source;
            }
        }

        public bool Cancel
        {
            get
            {
                return this.source.Cancel;
            }
            set
            {
                this.source.Cancel = value;
            }
        }

        public Uri Uri
        {
            get;
            private set;
        }
    }
}
