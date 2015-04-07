namespace More.Configuration
{
    using global::System;
    using global::System.Diagnostics.Contracts;
    using global::System.Threading.Tasks;

    /// <content>
    /// Provides the code contract definition for the <see cref="IConfigurationSettingLocator{T}"/> interface.
    /// </content>
    [ContractClassFor( typeof( IConfigurationSettingLocator<> ) )]
    internal abstract class IConfigurationSettingLocatorContract<T> : IConfigurationSettingLocator<T>
    {
        DeploymentEnvironment IConfigurationSettingLocator<T>.DefaultEnvironment
        {
            get
            {
                return default( DeploymentEnvironment );
            }
        }

        IConfigurationSettingLocator<T> IConfigurationSettingLocator<T>.NextLocator
        {
            get
            {
                return default( IConfigurationSettingLocator<T> );
            }
        }

        void IConfigurationSettingLocator<T>.ClearCache()
        {
        }

        Task<T> IConfigurationSettingLocator<T>.LocateAsync( string key )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
            return null;
        }

        Task<T> IConfigurationSettingLocator<T>.LocateAsync( string key, DeploymentEnvironment environment )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
            return null;
        }
    }
}
