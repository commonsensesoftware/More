namespace More.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks; 

    /// <summary>
    /// Defines the behavior of a configuration setting locator.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of configuration setting.</typeparam>
    [ContractClass( typeof( ISettingLocatorContract<> ) )]
    public interface ISettingLocator<T>
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
        /// <value>An <see cref="ISettingLocator{T}"/> object or null if this locator is the last locator in the chain.</value>
        ISettingLocator<T> NextLocator
        {
            get;
        }

        /// <summary>
        /// Clears any configuration settings cached by the locator.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Locates a configuration setting with the specified key.
        /// </summary>
        /// <param name="key">The key for the configuration setting to locate.</param>        
        /// <returns>The located <typeparamref name="T">value</typeparamref>.</returns>
       /// <remarks>Implementors should treat the <paramref name="key"/> values in an ordinal case-insensitive fashion. It is suggested that string
        /// comparisons are performed via the <see cref="T:StringComparison.OrdinalIgnoreCase">ordinal, ignore case</see> method.</remarks>
        T Locate( string key );

        /// <summary>
        /// Locates a configuration setting with the specified key and environment.
        /// </summary>
        /// <param name="key">The key for the configuration setting to locate.</param>
        /// <param name="environment">One of the <see cref="DeploymentEnvironment"/> values.</param>
        /// <returns>The located <typeparamref name="T">value</typeparamref>.</returns>
        /// <remarks>Implementors should treat <paramref name="key"/> values in an ordinal case-insensitive fashion. It is suggested that string
        /// comparisons are performed via the <see cref="T:StringComparison.OrdinalIgnoreCase">ordinal, ignore case</see> method.</remarks>
        T Locate( string key, DeploymentEnvironment environment );
    }
}
