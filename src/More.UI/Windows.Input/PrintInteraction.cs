namespace More.Windows.Input
{
    using System;

    /// <summary>
    /// Represents an interaction request to print a document.
    /// </summary>
    public class PrintInteraction : Interaction
    {
        bool printPreview;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintInteraction"/> class.
        /// </summary>
        public PrintInteraction() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public PrintInteraction( string title ) : base( title ) { }

        /// <summary>
        /// Gets or sets a value indicating whether the request is for a print preview.
        /// </summary>
        /// <value>True if a preview of the print job should be shown; otherwise, false.</value>
        public bool PrintPreview
        {
            get => printPreview;
            set => SetProperty( ref printPreview, value );
        }
    }
}