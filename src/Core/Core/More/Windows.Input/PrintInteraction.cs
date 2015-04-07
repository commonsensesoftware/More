namespace More.Windows.Input
{
    using global::System;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an interaction request to print a document.
    /// </summary>
    public class PrintInteraction : Interaction
    {
        private bool printPreview;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintInteraction"/> class.
        /// </summary>
        public PrintInteraction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintInteraction"/> class.
        /// </summary>
        /// <param name="title">The title associated with the interaction.</param>
        public PrintInteraction( string title )
            : base( title )
        {
            Contract.Requires<ArgumentNullException>( title != null, "title" );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the request is for a print preview.
        /// </summary>
        /// <value>True if a preview of the print job should be shown; otherwise, false.</value>
        public bool PrintPreview
        {
            get
            {
                return this.printPreview;
            }
            set
            {
                this.SetProperty( ref this.printPreview, value );
            }
        }
    }
}
