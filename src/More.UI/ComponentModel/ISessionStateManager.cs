namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behavior of a session state manager.
    /// </summary>
    [ContractClass( typeof( ISessionStateManagerContract ) )]
    public interface ISessionStateManager
    {
        /// <summary>
        /// Gets a dictionary of key/value pairs representing the current session state.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}">dictionary</see> containing the curren session state.</value>
        IDictionary<string, object> SessionState { get; }

        /// <summary>
        /// Adds a well-known session state type.
        /// </summary>
        /// <param name="sessionStateType">The well-known session state type to register.</param>
        /// <remarks>All intrinsic primitive types are well-known; however, custom session state objects
        /// may require type information in order to be serialized.</remarks>
        void AddKnownType( Type sessionStateType );

        /// <summary>
        /// Saves the current session state asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        Task SaveAsync();

        /// <summary>
        /// Restores any previously saved session state asynchronously.
        /// </summary>
        /// <param name="sessionKey">The key which identifies the type of session.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The <paramref name="sessionKey">session key</paramref> can be used to disambiguate
        /// between multiple application launch scenarios.</remarks>
        Task RestoreAsync( string sessionKey );
    }
}