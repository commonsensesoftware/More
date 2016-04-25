namespace More.Configuration
{
    using System;

    /// <summary>
    /// Defines the behavior of a setting.
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <value>The setting key.</value>
        string Key { get; }

        /// <summary>
        /// Gets the default value of the setting.
        /// </summary>
        /// <value>The setting default value.</value>
        object DefaultValue { get; }
    }
}
