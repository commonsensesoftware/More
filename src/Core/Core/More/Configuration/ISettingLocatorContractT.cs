namespace More.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <content>
    /// Provides the code contract definition for the <see cref="ISettingLocator{T}"/> interface.
    /// </content>
    [ContractClassFor( typeof( ISettingLocator<> ) )]
    internal abstract class ISettingLocatorContract<T> : ISettingLocator<T>
    {
        DeploymentEnvironment ISettingLocator<T>.DefaultEnvironment
        {
            get
            {
                return default( DeploymentEnvironment );
            }
        }

        ISettingLocator<T> ISettingLocator<T>.NextLocator
        {
            get
            {
                return default( ISettingLocator<T> );
            }
        }

        void ISettingLocator<T>.ClearCache()
        {
        }

        T ISettingLocator<T>.Locate( string key )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
            return default( T );
        }

        T ISettingLocator<T>.Locate( string key, DeploymentEnvironment environment )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( key ), "key" );
            return default( T );
        }
    }
}
