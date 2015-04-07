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
            this.Dispose( false );
        }

        internal PrintTaskOptionDetailsWrapper( PrintTaskOptionDetails details )
        {
            Contract.Requires( details != null );
            this.details = details;
            this.details.OptionChanged += this.OnOptionChanged;
        }

        private void Dispose( bool disposing )
        {
            if ( this.disposed )
                return;

            this.disposed = true;

            if ( !disposing )
                return;

            if ( this.details != null )
            {
                this.details.OptionChanged -= this.OnOptionChanged;
                this.details = null;
            }

            if ( this.listCallbacks != null )
                this.listCallbacks.Clear();

            if ( this.textCallbacks != null )
                this.textCallbacks.Clear();
        }

        private void OnOptionChanged( PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs e )
        {
            var optionId = e.OptionId.ToString();
            Action<IPrintCustomOptionDetails> textOptionCallback;

            // try text options first
            if ( this.textCallbacks.TryGetValue( optionId, out textOptionCallback ) )
            {
                textOptionCallback( (IPrintCustomOptionDetails) sender.Options[optionId] );
                return;
            }

            Action<IPrintItemListOptionDetails> listOptionCallback;

            // then try list options
            if ( this.listCallbacks.TryGetValue( optionId, out listOptionCallback ) )
                listOptionCallback( (IPrintItemListOptionDetails) sender.Options[optionId] );
        }

        public IList<string> DisplayedOptions
        {
            get
            {
                return this.details.DisplayedOptions;
            }
        }

        public IReadOnlyDictionary<string, IPrintOptionDetails> Options
        {
            get
            {
                return this.details.Options;
            }
        }

        public IPrintItemListOptionDetails CreateItemListOption( string optionId, string displayName, Action<IPrintItemListOptionDetails> optionChanged )
        {
            var option = this.details.CreateItemListOption( optionId, displayName );

            if ( optionChanged != null )
                this.listCallbacks[optionId] = optionChanged;

            return option;
        }

        public IPrintCustomOptionDetails CreateTextOption( string optionId, string displayName, Action<IPrintCustomOptionDetails> optionChanged )
        {
            var option = this.details.CreateTextOption( optionId, displayName );

            if ( optionChanged != null )
                this.textCallbacks[optionId] = optionChanged;

            return option;
        }

        public PrintPageDescription GetPageDescription( uint jobPageNumber )
        {
            return this.details.GetPageDescription( jobPageNumber );
        }

        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }
    }
}
