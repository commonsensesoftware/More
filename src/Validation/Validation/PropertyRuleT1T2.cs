namespace More.ComponentModel.DataAnnotations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the base implementation for a rule that validates one property against another.
    /// </summary>
    /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
    public abstract class PropertyRule<TObject, TValue> : IPerInstanceRule<TObject, TValue>
    {
        private readonly Lazy<Func<TObject, TValue>> getter;
        private readonly Lazy<string> propertyName;
        private readonly TObject propertyInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRule{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="other">The <see cref="PropertyRule{TObject,TValue}">rule</see> the new instance derives from.</param>
        /// <param name="instance">The instance associated with the rule.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected PropertyRule( PropertyRule<TObject, TValue> other, TObject instance )
        {
            Arg.NotNull( other, nameof( other ) );

            getter = other.getter;
            propertyName = other.propertyName;
            ErrorMessage = other.ErrorMessage;
            propertyInstance = instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRule{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="propertyExpression">The <see cref="Expression{T}">expression</see> representing the property to validate against.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        protected PropertyRule( Expression<Func<TObject, TValue>> propertyExpression )
            : this( propertyExpression, null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyRule{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="propertyExpression">The <see cref="Expression{T}">expression</see> representing the property to validate against.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics." )]
        protected PropertyRule( Expression<Func<TObject, TValue>> propertyExpression, string errorMessage )
        {
            Arg.NotNull( propertyExpression, nameof( propertyExpression ) );

            getter = new Lazy<Func<TObject, TValue>>( propertyExpression.Compile );
            propertyName = new Lazy<string>( () => GetPropertyName( propertyExpression ) );
            ErrorMessage = errorMessage;
        }

        private static string GetPropertyName( Expression<Func<TObject, TValue>> propertyExpression )
        {
            Contract.Requires( propertyExpression != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var expression = propertyExpression.Body as MemberExpression;

            if ( expression == null || !( expression.Member is PropertyInfo ) )
            {
                var message = ValidationMessage.InvalidPropertyExpression.FormatDefault( propertyExpression, typeof( TObject ) );
                throw new ArgumentException( message, "propertyExpression" );
            }

            return expression.Member.Name;
        }

        /// <summary>
        /// Gets the error message associated with the rule.
        /// </summary>
        /// <value>The error message associated with the rule or <c>null</c>.</value>
        protected string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Evaluates the rule against the specified properties.
        /// </summary>
        /// <param name="sourceProperty">The <see cref="Property{T}">property</see> being validated.</param>
        /// <param name="targetProperty">The <see cref="Property{T}">property</see> being validated against.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        protected abstract IValidationResult Evaluate( Property<TValue> sourceProperty, Property<TValue> targetProperty );

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public IValidationResult Evaluate( Property<TValue> item )
        {
            if ( item == null || propertyInstance == null )
                return ValidationResult.Success;

            var other = new Property<TValue>( propertyName.Value, getter.Value( propertyInstance ) );
            return Evaluate( item, other );
        }

        /// <summary>
        /// Returns a specialized version of the rule for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to get the rule for.</param>
        /// <returns>A specialized <see cref="IRule{TInput,TOutput}">rule</see> for the provided <paramref name="instance"/>.</returns>
        public abstract IRule<Property<TValue>, IValidationResult> GetPerInstance( TObject instance );
    }
}
