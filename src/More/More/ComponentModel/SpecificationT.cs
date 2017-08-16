namespace More.ComponentModel
{
    using System;

    /// <summary>
    /// Represents a specification that executes a user-defined <see cref="Func{T1,TResult}">function</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of item to evaluate.</typeparam>
    /// <example>This example demonstrates how to create a specification that uses a lamda expression for the rule evaluation process.
    /// <code lang="C#"><![CDATA[
    /// using System.ComponentModel;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// 
    /// var checkedOut = new Specification<Book>( book => book.IsCheckedOut );
    /// var overdue = new Specification<Book>( book => book.ReturnDate >= DateTime.Today );
    /// var noticeSent = new Specification<Book>( book => book.NoticeMailed );
    /// var lateNoticeRequired = checkedOut.And( overdue ).And( noticeSent.Not() );
    /// var mailingList = repository.GetBooks().Where( lateNoticeRequired.IsSatisfiedBy );
    /// ]]></code>
    /// </example>
    public sealed class Specification<T> : SpecificationBase<T>
    {
        readonly Func<T, bool> evaluate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{T}"/> class.
        /// </summary>
        /// <param name="evaluate">The <see cref="Func{T1,TResult}">function</see> representing the evaluation of the specification.</param>
        public Specification( Func<T, bool> evaluate )
        {
            Arg.NotNull( evaluate, nameof( evaluate ) );
            this.evaluate = evaluate;
        }

        /// <summary>
        /// Determines whether the specified item satisfies the specification.
        /// </summary>
        /// <param name="item">The <typeparamref name="T">item</typeparamref> to evaluate.</param>
        /// <returns>True if the <paramref name="item"/> satisfies the specification; otherwise, false.</returns>
        public override bool IsSatisfiedBy( T item ) => evaluate( item );
    }
}