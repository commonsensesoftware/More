namespace More.Composition
{
    using System;
    using System.Composition;

    /// <summary>
    /// Represents generic export metadata.
    /// </summary>
    [MetadataAttribute]
    public sealed class ExportMetadata
    {
        /// <summary>
        /// Gets or sets the exported type.
        /// </summary>
        /// <value>The exported <see cref="Type">type</see>.</value>
        public Type ExportedType
        {
            get;
            set;
        }
    }
}
