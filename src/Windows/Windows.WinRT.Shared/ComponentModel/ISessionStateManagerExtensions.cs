namespace More.ComponentModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides extension methods for the <see cref="ISessionStateManager"/> interface.
    /// </summary>
    public static class ISuspensionManagerExtensions
    {
        private const string NavigationStateKey = "NavigationState";

        internal static IDictionary<string, object> GetSessionState( this ISessionStateManager sessionStateManager, Frame frame )
        {
            Contract.Requires( sessionStateManager != null );
            Contract.Requires( frame != null );
            Contract.Ensures( Contract.Result<IDictionary<string, object>>() != null );

            var sessionKey = frame.Name;

            // fallback to the main session state
            if ( string.IsNullOrEmpty( sessionKey ) )
                return sessionStateManager.SessionState;

            // use session state localized to the frame (e.g. key)
            return (IDictionary<string, object>) sessionStateManager.SessionState.GetOrAdd( sessionKey, () => new Dictionary<string, object>() );
        }

        internal static void RestoreNavigationState( this ISessionStateManager sessionStateManager, Frame frame )
        {
            Contract.Requires( sessionStateManager != null );
            Contract.Requires( frame != null );
            
            object value;

            // restore navigation state if present
            if ( sessionStateManager.GetSessionState( frame ).TryGetValue( NavigationStateKey, out value ) )
                frame.SetNavigationState( (string) value );
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
                return false;

            var frame = window.Content as Frame;

            if ( frame == null )
                return false;

            sessionStateManager.SaveNavigationState( frame );
            return true;
        }
    }
}
