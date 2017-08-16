namespace More.Windows.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.Graphics.Printing;
    using global::Windows.Graphics.Printing.OptionDetails;

    /// <summary>
    /// Provides the code contract definition for the <see cref="IPrintOptionDetailsFactory"/> class.
    /// </summary>
    [ContractClassFor( typeof( IPrintOptionDetailsFactory ) )]
    internal abstract class IPrintOptionDetailsFactoryContract : IPrintOptionDetailsFactory
    {
        IList<string> IPrintOptionDetailsFactory.DisplayedOptions
        {
            get
            {
                Contract.Ensures( Contract.Result<IList<string>>() != null );
                return null;
            }
        }

        IReadOnlyDictionary<string, IPrintOptionDetails> IPrintOptionDetailsFactory.Options
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyDictionary<string, IPrintOptionDetails>>() != null );
                return null;
            }
        }

        IPrintItemListOptionDetails IPrintOptionDetailsFactory.CreateItemListOption( string optionId, string displayName, Action<IPrintItemListOptionDetails> optionChanged )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( optionId ), nameof( optionId ) );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( displayName ), nameof( displayName ) );
            Contract.Ensures( Contract.Result<IPrintItemListOptionDetails>() != null );
            return null;
        }

        IPrintCustomOptionDetails IPrintOptionDetailsFactory.CreateTextOption( string optionId, string displayName, Action<IPrintCustomOptionDetails> optionChanged )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( optionId ), nameof( optionId ) );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( displayName ), nameof( displayName ) );
            Contract.Ensures( Contract.Result<IPrintCustomOptionDetails>() != null );
            return null;
        }

        PrintPageDescription IPrintOptionDetailsFactory.GetPageDescription( uint jobPageNumber ) => default( PrintPageDescription );
    }
}