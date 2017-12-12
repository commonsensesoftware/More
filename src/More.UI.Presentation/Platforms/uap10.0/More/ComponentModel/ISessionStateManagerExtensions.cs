namespace More.ComponentModel
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="ISessionStateManager"/> interface.
    /// </summary>
    public static class ISessionStateManagerExtensions
    {
        const string NavigationStateKey = "NavigationState";

        internal static IDictionary<string, object> GetSessionState( this ISessionStateManager sessionStateManager, Frame frame )
        {
            Contract.Requires( sessionStateManager != null );
            Contract.Requires( frame != null );
            Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );

            var sessionKey = frame.Name;

            if ( string.IsNullOrEmpty( sessionKey ) )
            {
                return sessionStateManager.SessionState;
            }

            return (IDictionary<string, object>) sessionStateManager.SessionState.GetOrAdd( sessionKey, () => new Dictionary<string, object>() );
        }

        internal static void RestoreNavigationState( this ISessionStateManager sessionStateManager, Frame frame )
        {
            Contract.Requires( sessionStateManager != null );
            Contract.Requires( frame != null );

            if ( sessionStateManager.GetSessionState( frame ).TryGetValue( NavigationStateKey, out var value ) )
            {
                frame.SetNavigationState( (string) value );
            }
        }

        internal static void SaveNavigationState( this ISessionStateManager sessionStateManager, Frame frame )
        {
            Contract.Requires( sessionStateManager != null );
            Contract.Requires( frame != null );
            sessionStateManager.GetSessionState( frame )[NavigationStateKey] = frame.GetNavigationState();
        }

        /// <summary>
        /// Attempts to to save the current navigation state to the session state manager.
        /// </summary>
        /// <param name="sessionStateManager">The extended <see cref="ISessionStateManager"/> object.</param>
        /// <returns>True if the navigation state was successfully saved; otherwise, false.</returns>
        /// <remarks>In order for this method to succeed, the <see cref="Window.Content">content</see> of the
        /// <see cref="Window.Current">current window</see> must be a <see cref="Frame">frame</see> control.</remarks>
        public static bool TrySaveNavigationState( this ISessionStateManager sessionStateManager )
        {
            Arg.NotNull( sessionStateManager, nameof( sessionStateManager ) );

            var window = Window.Current;

            if ( window == null )
            {
                return false;
            }

            if ( window.Content is Frame frame )
            {
                sessionStateManager.SaveNavigationState( frame );
                return true;
            }

            return false;
        }
    }
}