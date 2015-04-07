namespace More.Windows.Printing
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Printing;

    internal sealed class PrintAreaWrapper : IPrintArea
    {
        private readonly Panel canvas;
        private readonly Lazy<PrintDocument> printDocument = new Lazy<PrintDocument>( () => new PrintDocument() );

        internal PrintAreaWrapper( Panel canvas )
        {
            Contract.Requires( canvas != null );
            this.canvas = canvas;
        }

        public PrintDocument PrintDocument
        {
            get
            {
                return this.printDocument.Value;
            }
        }

        public void Clear()
        {
            this.canvas.Children.Clear();
        }

        public void Add( object content )
        {
            var page = content as UIElement;

            if ( page == null )
                return;

            this.canvas.Children.Add( page );
            this.canvas.InvalidateMeasure();
            this.canvas.UpdateLayout();
        }

        public void Refresh()
        {
            this.canvas.InvalidateArrange();
            this.canvas.InvalidateMeasure();
            this.canvas.UpdateLayout();
        }
    }
}
