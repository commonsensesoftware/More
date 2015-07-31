namespace More.Windows.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the possible widths for the Settings contract flyout.
    /// </summary>
    [SuppressMessage( "Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "The enum maps to specific values. Zero is not a valid value." )]
    public enum SettingsFlyoutWidth
    {
        /// <summary>
        /// Indicates the narrow flyout width (346 pixels).
        /// </summary>
        Narrow = 346,

        /// <summary>
        /// Indicates the wide flyout width (646 pixels).
        /// </summary>
        Wide = 646
    }
}
