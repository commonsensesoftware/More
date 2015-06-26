namespace More.ComponentModel
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents the default strategy used to take the snapshot of an editable object.
    /// </summary>
    internal sealed class SnapshotStrategy : IEditSnapshotStrategy
    {
        private readonly static Lazy<SnapshotStrategy> defaultInstance = new Lazy<SnapshotStrategy>( () => new SnapshotStrategy() );

        private SnapshotStrategy()
        {
        }

        internal static IEditSnapshotStrategy Default
        {
            get
            {
                Contract.Ensures( Contract.Result<IEditSnapshotStrategy>() != null );
                return defaultInstance.Value;
            }
        }

        public object GetSnapshot( object value )
        {
            // REF: http://blogs.msdn.com/b/brada/archive/2004/05/03/125427.aspx
            // System.ICloneable has been removed in the CoreCLR and isn't portable

            // since we cannot clone or serialize the value, echo the value
            return value;
        }
    }
}
