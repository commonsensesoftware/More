namespace More
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Represents a typed <see cref="WeakDelegate">weak delegate</see>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Delegate">delegate</see> to make weak.</typeparam>
    /// <remarks>The .NET Framework does not allow specifying a <see cref="Delegate">delegate</see> as
    /// a type constraint.  The initialization of an instance of this class will throw an
    /// <see cref="InvalidOperationException">exception</see> if <typeparamref name="T"/> is not
    /// a <see cref="Delegate">delegate</see>.</remarks>
    [SuppressMessage( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This is an appropriate name for a weakly referenced delegate." )]
    public class WeakDelegate<T> : WeakDelegate where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakDelegate{T}"/> class.
        /// </summary>
        /// <param name="strongDelegate">The strong <typeparamref name="T">delegate</typeparamref> to make weak.</param>
        public WeakDelegate( T strongDelegate )
            : base( strongDelegate as Delegate )
        {
        }

        /// <summary>
        /// Returns a value indicating whether the current weak delegate is a match (equivalent) for the specified strong delegate.
        /// </summary>
        /// <param name="strongDelegate">The <typeparamref name="T">delegate</typeparamref> to evaluate.</param>
        /// <returns>True if the current weak delegate is a match for the specified strong delegate; otherwise, false.</returns>
        public virtual bool IsMatch( T strongDelegate )
        {
            var d = strongDelegate as Delegate;
            return this.IsMatch( d );
        }

        /// <summary>
        /// Creates and returns a typed, strongly referenced delegate.
        /// </summary>
        /// <returns>The <see cref="Delegate">delegate</see> of type <typeparamref name="T"/> created or
        /// null if the delegate cannot be created.</returns>
        public virtual T CreateTypedDelegate()
        {
            return this.CreateDelegate() as T;
        }
    }
}
