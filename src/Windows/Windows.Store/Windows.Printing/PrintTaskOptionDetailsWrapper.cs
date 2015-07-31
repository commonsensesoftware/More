namespace More.Windows.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;    
    using global::Windows.Graphics.Printing;
    using global::Windows.Graphics.Printing.OptionDetails;

    internal sealed class PrintTaskOptionDetailsWrapper : IPrintOptionDetailsFactory, IDisposable
    {
        private readonly Dictionary<string, Action<IPrintItemListOptionDetails>> listCallbacks = new Dictionary<string, Action<IPrintItemListOptionDetails>>();
        private readonly Dictionary<string, Action<IPrintCustomOptionDetails>> textCallbacks = new Dictionary<string, Action<IPrintCustomOptionDetails>>();
        private bool disposed;
        private PrintTaskOptionDetails details;

        ~PrintTaskOptionDetailsWrapper()
        {
            Dispose( false );
        }

        internal PrintTaskOptionDetailsWrapper( PrintTaskOptionDetails details )
        {
            Contract.Requires( details != null );
            this.details = details;
            this.details.OptionChanged += OnOptionChanged;
        }

        private void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            disposed = true;

            if ( !disposing )
                return;

            if ( details != null )
            {
                details.OptionChanged -= OnOptionChanged;
                details = null;
            }

            if ( listCallbacks != null )
                listCallbacks.Clear();

            if ( textCallbacks != null )
                textCallbacks.Clear();
        }

        private void OnOptionChanged( PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs e )
        {
            var optionId = e.OptionId.ToString();
            Action<IPrintCustomOptionDetails> textOptionCallback;

            // try text options first
            if ( textCallbacks.TryGetValue( optionId, out textOptionCallback ) )
            {
                textOptionCallback( (IPrintCustomOptionDetails) sender.Options[optionId] );
                return;
            }

            Action<IPrintItemListOptionDetails> listOptionCallback;

            // then try list options
            if ( listCallbacks.TryGetValue( optionId, out listOptionCallback ) )
                listOptionCallback( (IPrintItemListOptionDetails) sender.Options[optionId] );
        }

        public IList<string> DisplayedOptions
        {
            get
            {
                return details.DisplayedOptions;
            }
        }

        public IReadOnlyDictionary<string, IPrintOptionDetails> Options
        {
            get
            {
                return details.Options;
            }
        }

        public IPrintItemListOptionDetails CreateItemListOption( string optionId, string displayName, Action<IPrintItemListOptionDetails> optionChanged )
        {
            Arg.NotNullOrEmpty( optionId, nameof( optionId ) );
            Arg.NotNullOrEmpty( displayName, nameof( displayName ) );

            var option = details.CreateItemListOption( optionId, displayName );

            if ( optionChanged != null )
                listCallbacks[optionId] = optionChanged;

            return option;
        }

        public IPrintCustomOptionDetails CreateTextOption( string optionId, string displayName, Action<IPrintCustomOptionDetails> optionChanged )
        {
            Arg.NotNullOrEmpty( optionId, nameof( optionId ) );
            Arg.NotNullOrEmpty( displayName, nameof( displayName ) );

            var option = details.CreateTextOption( optionId, displayName );

            if ( optionChanged != null )
                textCallbacks[optionId] = optionChanged;

            return option;
        }

        public PrintPageDescription GetPageDescription( uint jobPageNumber ) => details.GetPageDescription( jobPageNumber );

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
