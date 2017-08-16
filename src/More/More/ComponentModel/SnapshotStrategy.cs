namespace More.ComponentModel
{
    using System;

    /// <summary>
    /// Represents the default strategy used to take the snapshot of an editable object.
    /// </summary>
    sealed class SnapshotStrategy : IEditSnapshotStrategy
    {
        SnapshotStrategy() { }

        internal static IEditSnapshotStrategy Default { get; } = new SnapshotStrategy();

        /// <summary>
        /// Create a snapshot of the specified object.
        /// </summary>
        /// <param name="value">The value to create snapshot of.</param>
        /// <returns>The object snapshot.</returns>
        /// <remarks>Since we cannot clone or serialize the value, echo the value.
        /// REF: http://blogs.msdn.com/b/brada/archive/2004/05/03/125427.aspx
        /// System.ICloneable has been removed in the CoreCLR and isn't portable
        /// </remarks>
        public object GetSnapshot( object value ) => value;
    }
}