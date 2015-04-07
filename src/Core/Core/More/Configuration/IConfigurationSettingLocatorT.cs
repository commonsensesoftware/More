namespace More.Configuration
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading.Tasks; 

    /// <summary>
    /// Defines the behavior of a configuration setting locator.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of configuration setting.</typeparam>
    [ContractClass( typeof( IConfigurationSettingLocatorContract<> ) )]
    public interface IConfigurationSettingLocator<T>
    {
        /// <summary>
        /// Gets the default deployment environment used by the locator.
        /// </summary>
        /// <value>One of the <see cref="DeploymentEnvironment"/> values.</value>
        DeploymentEnvironment DefaultEnvironment
        {
            get;
        }

        /// <summary>
        /// Gets the next locator for the current instance.
        /// </summary>
        /// <value>An <see cref="IConfigurationSettingLocator{T}"/> object or null if this locator is the last locator in the chain.</value>
        IConfigurationSettingLocator<T> NextLocator
        {
            get;
        }

        /// <summary>
        /// Clears any configuration settings cached by the locator.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Locates a configuration setting with the specified key and environment asychronously.
        /// </summary>
        /// <returns>A <see cref="Task{T}">task</see> containing the located <typeparamref name="T">value</typeparamref>.</returns>
        /// <remarks>Implementors should treat the <paramref name="key"/> values in an ordinal case-insensitive fashion. It is suggested that string
        /// comparisons are performed via the <see cref="T:StringComparison.OrdinalIgnoreCase">ordinal, ignore case</see> method.</remarks>
        /// <param name="key">The key for the configuration setting to locate.</param>
        /// <returns>A <typeparamref name="T"/> object.</returns>
        Task<T> LocateAsync( string key );

        /// <summary>
        /// Locates a configuration setting with the specified key and environment asychronously.
        /// </summary>
        /// <param name="key">The key for the configuration setting to locate.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing the located <typeparamref name="T">value</typeparamref>.</returns>
        /// <remarks>Implementors should treat <paramref name="key"/> values in an ordinal case-insensitive fashion. It is suggested that string
        /// comparisons are performed via the <see cref="T:StringComparison.OrdinalIgnoreCase">ordinal, ignore case</see> method.</remarks>
        Task<T> LocateAsync( string key, DeploymentEnvironment environment );
    }
}
