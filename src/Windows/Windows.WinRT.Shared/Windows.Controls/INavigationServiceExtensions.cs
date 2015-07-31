namespace More.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="INavigationService"/> interface.
    /// </summary>
    [CLSCompliant( false )]
    public static class INavigationServiceExtensions
    {
        /// <summary>
        /// Causes the service to load content that is specified by data type.
        /// </summary>
        /// <param name="navigationService">The extended <see cref="INavigationService">navigation service</see>.</param>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool Navigate( this INavigationService navigationService, Type sourcePageType )
        {
            Arg.NotNull( navigationService, nameof( navigationService ) );
            return navigationService.Navigate( sourcePageType, null );
        }

        /// <summary>
        /// Causes the service to load content that is specified by URI.
        /// </summary>
        /// <param name="navigationService">The extended <see cref="INavigationService">navigation service</see>.</param>
        /// <param name="sourcePage">The <see cref="Uri">URI</see> of the content to load.</param>
        /// <returns>True if the service can navigate according to its settings; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool Navigate( INavigationService navigationService, Uri sourcePage )
        {
            Arg.NotNull( navigationService, nameof( navigationService ) );
            return navigationService.Navigate( sourcePage, null );
        }
    }
}
