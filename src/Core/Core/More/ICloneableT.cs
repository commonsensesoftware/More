namespace More
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;

    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value as an existing instance.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to clone.</typeparam>
    [ContractClass( typeof( ICloneableContract<> ) )]
    public interface ICloneable<in T>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <typeparam name="TClone">The <see cref="Type">type</see> of clone to create.</typeparam>
        /// <returns>A new object of type <typeparamref name="TClone"/> that is a copy of this instance.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design. Avoids common coding errors." )]
        TClone Clone<TClone>() where TClone : T;
    }
}
