namespace More.Windows.Printing
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Printing;

    /// <summary>
    /// Defines the behavior of a print area.
    /// </summary>
    [CLSCompliant( false )]
    [ContractClass( typeof( IPrintAreaContract ) )]
    public interface IPrintArea
    {
        /// <summary>
        /// Gets the print document associated with the print area.
        /// </summary>
        /// <value>A <see cref="PrintDocument"/> object.</value>
        PrintDocument PrintDocument
        {
            get;
        }

        /// <summary>
        /// Clears all the content from the print area.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds new content to be printed.
        /// </summary>
        /// <param name="content">The content to be print. This is typically a print preview page.</param>
        void Add( object content );

        /// <summary>
        /// Refreshes the print area.
        /// </summary>
        /// <remarks>This method can be used to invalid and/or update the print layout.</remarks>
        void Refresh();
    }
}
