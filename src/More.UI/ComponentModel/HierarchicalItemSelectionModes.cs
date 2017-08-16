namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the possible hierarchical item selection modes.
    /// </summary>
    [Flags]
    [SuppressMessage( "Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "The zero value means all items, not all options.  Synchronize is the only combinable option." )]
    public enum HierarchicalItemSelectionModes
    {
        /// <summary>
        /// Indicates that all items are selected.
        /// </summary>
        All,

        /// <summary>
        /// Indicates that only leaf items are selected.
        /// </summary>
        Leaf,

        /// <summary>
        /// Indicates that the selected state of the parent and children of the current item are synchronized.
        /// </summary>
        Synchronize
    }
}