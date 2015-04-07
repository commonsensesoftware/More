namespace More.Windows.Printing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.Graphics.Printing;
    using global::Windows.Graphics.Printing.OptionDetails;

    /// <summary>
    /// Provides extension methods for the <see cref="IPrintOptionDetailsFactory"/> interface.
    /// </summary>
    [CLSCompliant( false )]
    public static class IPrintOptionDetailsFactoryExtensions
    {
        /// <summary>
        /// Creates a custom list of items that allow the user to choose the page format.
        /// </summary>
        /// <param name="factory">The extended <see cref="IPrintOptionDetailsFactory"/> object.</param>
        /// <param name="optionId">The identifier for the custom item.</param>
        /// <param name="displayName">The display name for the custom item.</param>
        /// <returns>The <see cref="IPrintItemListOptionDetails">list of custom items</see> created.</returns>
        public static IPrintItemListOptionDetails CreateItemListOption( this IPrintOptionDetailsFactory factory, string optionId, string displayName )
        {
            Contract.Requires<ArgumentNullException>( factory != null, "factory" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( optionId ), "optionId" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( displayName ), "displayName" );
            Contract.Ensures( Contract.Result<IPrintItemListOptionDetails>() != null );
            return factory.CreateItemListOption( optionId, displayName, null );
        }

        /// <summary>
        /// Creates a user text input option.
        /// </summary>
        /// <param name="factory">The extended <see cref="IPrintOptionDetailsFactory"/> object.</param>
        /// <param name="optionId">The identifier of the print task option.</param>
        /// <param name="displayName">The display name of the print task option.</param>
        /// <returns>The <see cref="IPrintCustomOptionDetails">custom text option</see> created.</returns>
        public static IPrintCustomOptionDetails CreateTextOption( this IPrintOptionDetailsFactory factory, string optionId, string displayName )
        {
            Contract.Requires<ArgumentNullException>( factory != null, "factory" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( optionId ), "optionId" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( displayName ), "displayName" );
            Contract.Ensures( Contract.Result<IPrintCustomOptionDetails>() != null );
            return factory.CreateTextOption( optionId, displayName, null );
        }
    }
}
