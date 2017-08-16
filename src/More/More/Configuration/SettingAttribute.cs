namespace More.Configuration
{
    using System;
    using static System.AttributeTargets;

    /// <summary>
    /// Represents the metadata for an application setting.
    /// </summary>
    [AttributeUsage( Property | Parameter )]
    public sealed class SettingAttribute : Attribute, ISetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute" /> class.
        /// </summary>
        public SettingAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAttribute" /> class.
        /// </summary>
        /// <param name="key">The setting key.</param>
        public SettingAttribute( string key ) => Key = key;

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <value>The setting key.</value>
        public string Key { get; }

        /// <summary>
        /// Gets or sets the default value of the setting.
        /// </summary>
        /// <value>The setting default value.</value>
        public object DefaultValue { get; set; }
    }
}