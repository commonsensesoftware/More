namespace System.Composition
{
    using System;

    /// <summary>
    /// The delegate signature that allows composite instances to be resolved.
    /// </summary>
    /// <param name="type">The <see cref="Type">type</see> of instance to resolve.</param>
    /// <param name="key">The key of the instance to resolve.</param>
    /// <param name="instance">The resolved instance or <c>null</c> if resolution fails.</param>
    /// <returns>True if the requested instance was resolved; otherwise, false.</returns>
    public delegate bool CompositeResolver( Type type, string key, out object instance );
}