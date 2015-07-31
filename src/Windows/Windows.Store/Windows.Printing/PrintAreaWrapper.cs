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
                return printDocument.Value;
            }
        }

        public void Clear()
        {
            canvas.Children.Clear();
        }

        public void Add( object content )
        {
            Arg.NotNull( content, nameof( content ) );

            var page = content as UIElement;

            if ( page == null )
                return;

            canvas.Children.Add( page );
            canvas.InvalidateMeasure();
            canvas.UpdateLayout();
        }

        public void Refresh()
        {
            canvas.InvalidateArrange();
            canvas.InvalidateMeasure();
            canvas.UpdateLayout();
        }
    }
}
