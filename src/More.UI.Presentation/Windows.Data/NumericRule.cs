namespace More.Windows.Data
{
    using More.ComponentModel;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
#if UAP10_0
    using global::Windows.UI.Xaml.Markup;
#else
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Represents the base class for a numeric rule.
    /// </summary>
#if UAP10_0
    [ContentProperty( Name = "Rules" )]
#else
    [ContentProperty( "Rules" )]
#endif
    public abstract class NumericRule : IRule<decimal?, bool>
    {
        readonly Lazy<ObservableCollection<IRule<decimal?, bool>>> rules =
            new Lazy<ObservableCollection<IRule<decimal?, bool>>>( () => new ObservableCollection<IRule<decimal?, bool>>() );

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRule"/> class.
        /// </summary>
        protected NumericRule() { }

        /// <summary>
        /// Gets a collection of subordinate rules.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> object.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required to support generics." )]
        public virtual Collection<IRule<decimal?, bool>> Rules
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<IRule<decimal?, bool>>>() != null );
                return rules.Value;
            }
        }

        /// <summary>
        /// Evaluates the rule.
        /// </summary>
        /// <param name="item">The <see cref="T:System.Nullable{T}">number</see> to evalute.</param>
        /// <returns>True if the rule is satisified; otherwise, false.</returns>
        public virtual bool Evaluate( decimal? item ) => Rules.Any() && Rules.All( rule => rule.Evaluate( item ) );
    }
}