namespace More.Windows.Printing
{
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml.Printing;

    /// <summary>
    /// Provides the code contract definition for the <see cref="IPrintArea"/> class.
    /// </summary>
    [ContractClassFor( typeof( IPrintArea ) )]
    internal abstract class IPrintAreaContract : IPrintArea
    {
        PrintDocument IPrintArea.PrintDocument
        {
            get
            {
                Contract.Ensures( Contract.Result<PrintDocument>() != null );
                return null;
            }
        }

        void IPrintArea.Clear()
        {
        }

        void IPrintArea.Add( object content )
        {
            Contract.Requires<ArgumentNullException>( content != null, "content" );
        }

        void IPrintArea.Refresh()
        {
        }
    }
}
