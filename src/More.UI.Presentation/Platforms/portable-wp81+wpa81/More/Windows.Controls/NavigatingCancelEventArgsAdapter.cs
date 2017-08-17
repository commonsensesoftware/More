namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Navigation;

    sealed class NavigatingCancelEventArgsAdapter : INavigationStartingEventArgs
    {
        readonly NavigatingCancelEventArgs source;

        internal NavigatingCancelEventArgsAdapter( NavigatingCancelEventArgs source ) => this.source = source;

        public object SourceEventArgs => source;

        public bool Cancel
        {
            get => source.Cancel;
            set => source.Cancel = value;
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required to implement INavigationStartingEventArgs." )]
        public Uri Uri { get; }
    }
}