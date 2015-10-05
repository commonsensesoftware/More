namespace More.Composition
{
    using System;

    /// <summary>
    /// Represents well-known sharing boundary names.
    /// </summary>
    public static class Boundary
    {
        /// <summary>
        /// Gets the boundary name for instances shared using the Per-Request model.
        /// </summary>
        /// <remarks>This sharing boundary is typically used for web applications.</remarks>
        public const string PerRequest = nameof( PerRequest );
    }
}