namespace More.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a collection of <see cref="IRule{TInput,TOutput}">rules</see> that can be used
    /// as a single, composite <see cref="IRule{TInput,TOutput}">rule</see>.
    /// </summary>
    /// <typeparam name="TInput">The <see cref="Type">type</see> to evaluate.</typeparam>
    /// <typeparam name="TOutput">The <see cref="Type">type</see> result of the evaluation.</typeparam>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This type represents a composite rule that is a collection of other rules." )]
    public abstract class CompositeRule<TInput, TOutput> : IRule<TInput, TOutput>, ICollection<IRule<TInput, TOutput>>
    {
        private readonly IList<IRule<TInput, TOutput>> rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeRule{TInput, TOutput}"/> class.
        /// </summary>
        protected CompositeRule()
            : this( new List<IRule<TInput, TOutput>>() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeRule{TInput, TOutput}"/> class.
        /// </summary>
        /// <param name="rules">The initial <see cref="IEnumerator{T}">rules</see> in the set.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generic support." )]
        protected CompositeRule( IEnumerable<IRule<TInput, TOutput>> rules )
        {
            Arg.NotNull( rules, "rules" );
            this.rules = rules.ToList();
        }

        /// <summary>
        /// Gets the list of nested rules in the current composite rule.
        /// </summary>
        /// <value>A <see cref="IList{T}">list</see> of nested
        /// <see cref="IRule{TInput,TOutput}">rules</see>.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        protected IList<IRule<TInput, TOutput>> NestedRules
        {
            get
            {
                Contract.Ensures( this.rules != null );
                return this.rules;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="CompositeRule{TInput,TOutput}"/>.
        /// </summary>
        protected virtual void ClearItems()
        {
            Contract.Ensures( this.Count == 0 ); 
            this.rules.Clear();
        }

        /// <summary>
        /// Inserts an item into the <see cref="CompositeRule{TInput,TOutput}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The <see cref="IRule{TInput,TOutput}">item</see> to insert.</param>
        protected virtual void InsertItem( int index, IRule<TInput, TOutput> item )
        {
            Arg.NotNull( item, "item" );
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) + 1 );
            Arg.GreaterThanOrEqualTo( index, 0, "index" );
            Arg.LessThanOrEqualTo( index, this.Count, "index" );
            this.rules.Add( item );
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected virtual void RemoveItem( int index )
        {
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) - 1 );
            Arg.GreaterThanOrEqualTo( index, 0, "index" );
            Arg.LessThanOrEqualTo( index, this.Count, "index" );
            this.rules.RemoveAt( index );
        }

        /// <summary>
        /// Adds a new rule to the composite.
        /// </summary>
        /// <param name="item">The <see cref="IRule{T, T}"/> to add.</param>
        public void Add( IRule<TInput, TOutput> item )
        {
            this.InsertItem( this.rules.Count, item );
        }

        /// <summary>
        /// Removes all rules from the composite.
        /// </summary>
        public void Clear()
        {
            this.ClearItems();
        }

        /// <summary>
        /// Determines whether the rules set contains the specified rule using the defaul equality comparison.
        /// </summary>
        /// <param name="item">The <see cref="IRule{T, T}"/> to locate.</param>
        /// <returns><see langkeyword="true">true</see> if the rule set contains the rule; otherwise, <see langkeyword="false">false</see>.</returns>
        public bool Contains( IRule<TInput, TOutput> item )
        {
            return this.rules.Contains( item );
        }

        /// <summary>
        /// Copies all of the rules in the rule set to the specified array.
        /// </summary>
        /// <param name="array">The array of type <see cref="IRule{T, K}"/> to copy the rules to.</param>
        /// <param name="arrayIndex">The zero-based index where copying begins.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract" )]
        public void CopyTo( IRule<TInput, TOutput>[] array, int arrayIndex )
        {
            this.rules.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// Gets a count of the rules in the rule set.
        /// </summary>
        /// <value>The count of rules in the rule set.</value>
        public int Count
        {
            get
            {
                return this.rules.Count;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This property will always return false." )]

        bool ICollection<IRule<TInput, TOutput>>.IsReadOnly
        {
            get
            {
                return this.rules.IsReadOnly;
            }
        }

        /// <summary>
        /// Removes the specified rule from the composite.
        /// </summary>
        /// <param name="item">The <see cref="IRule{T, K}"/> to remove.</param>
        /// <returns>True if the rule is removed; otherwise, false.</returns>
        public bool Remove( IRule<TInput, TOutput> item )
        {
            var index = this.rules.IndexOf( item );

            if ( index < 0 )
                return false;

            var count = this.rules.Count;
            this.RemoveItem( index );

            return count > this.rules.Count;
        }

        /// <summary>
        /// Returns an enumerator for the current instance.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object.</returns>
        public virtual IEnumerator<IRule<TInput, TOutput>> GetEnumerator()
        {
            return this.rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Evaluates the specified item and returns the result of the evaluation.
        /// </summary>
        /// <param name="item">The <typeparamref name="TInput">item</typeparamref> to evaluate.</param>
        /// <returns>The <typeparamref name="TOutput">result</typeparamref> of the evaluation.</returns>
        public abstract TOutput Evaluate( TInput item );
    }
}
