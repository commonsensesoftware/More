namespace More.Composition
{
    using System;
    using System.Composition;

    /// <summary>
    /// Represents the metadata that marks a part as constrained to being shared, but only per HTTP request.
    /// </summary>
    [CLSCompliant( false )]
    [AttributeUsage( AttributeTargets.Class, Inherited = false )]
    public sealed class SharedPerRequestAttribute : SharedAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedPerRequestAttribute"/> class.
        /// </summary>
        public SharedPerRequestAttribute() : base( Boundary.PerRequest ) { }
    }
}