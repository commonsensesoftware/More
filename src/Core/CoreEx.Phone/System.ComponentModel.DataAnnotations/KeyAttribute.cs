namespace System.ComponentModel.DataAnnotations
{
    using global::System;

    /// <summary>
    /// Denotes one or more properties that uniquely identify an entity.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public sealed class KeyAttribute : Attribute
    {
    }
}
