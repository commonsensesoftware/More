namespace More.Windows.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.Graphics.Printing;
    using global::Windows.Graphics.Printing.OptionDetails;

    /// <summary>
    /// Defines the behavior of a print option details factory.
    /// </summary>
    [CLSCompliant( false )]
    [ContractClass( typeof( IPrintOptionDetailsFactoryContract ) )]
    public interface IPrintOptionDetailsFactory
    {
        /// <summary>
        /// Gets the list of print task options that are currently displayed.
        /// </summary>
        /// <value>The <see cref="IList{T}">list</see> of print task options that are currently displayed.</value>
        IList<string> DisplayedOptions { get; }

        /// <summary>
        /// Gets the read-only dictionary of options for the advanced print task.
        /// </summary>
        /// <value>The <see cref="IReadOnlyDictionary{TKey,TValue}">read-only dictionary</see> of
        /// <see cref="IPrintOptionDetails">options</see> for the advanced print task.</value>
        IReadOnlyDictionary<string, IPrintOptionDetails> Options { get; }

        /// <summary>
        /// Creates a custom list of items that allow the user to choose the page format.
        /// </summary>
        /// <param name="optionId">The identifier for the custom item.</param>
        /// <param name="displayName">The display name for the custom item.</param>
        /// <param name="optionChanged">The <see cref="Action{T}">action</see> to invoke when an option changes.</param>
        /// <returns>The <see cref="IPrintItemListOptionDetails">list of custom items</see> created.</returns>
        IPrintItemListOptionDetails CreateItemListOption( string optionId, string displayName, Action<IPrintItemListOptionDetails> optionChanged );

        /// <summary>
        /// Creates a user text input option.
        /// </summary>
        /// <param name="optionId">The identifier of the print task option.</param>
        /// <param name="displayName">The display name of the print task option.</param>
        /// <param name="optionChanged">The <see cref="Action{T}">action</see> to invoke when an option changes.</param>
        /// <returns>The <see cref="IPrintCustomOptionDetails">custom text option</see> created.</returns>
        IPrintCustomOptionDetails CreateTextOption( string optionId, string displayName, Action<IPrintCustomOptionDetails> optionChanged );

        /// <summary>
        /// Returns a page description for the referenced page number.
        /// </summary>
        /// <param name="jobPageNumber">The requested page number.</param>
        /// <returns>A <see cref="PrintPageDescription"/> object.</returns>
        PrintPageDescription GetPageDescription( uint jobPageNumber );
    }
}