namespace More.Composition
{
    using System;

    /// <summary>
    /// Provides methods to compose an existing object instance.
    /// </summary>
    public interface ICompositionService
    {
        /// <summary>
        /// Composes the specified object instance.
        /// </summary>
        /// <param name="instance">The object to compose.</param>
        void Compose( object instance );
    }
}